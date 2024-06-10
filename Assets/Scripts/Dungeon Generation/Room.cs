using System;
using System.Collections.Generic;
using System.Text;

namespace DefaultNamespace
{
    public class Room
    {
        // 10x8 matrix that represents a room
        public int[,] Tiles { get; private set; }

        // boolean that is true if the room is a unique one (e.g. boss room, lever room) and false if 
        public bool IsUnique { get; private set; }
        public Node Node { get; private set; }
        private const int Rows = 8;
        private const int Cols = 10;

        public Room(Node node, bool isUnique = false)
        {
            Tiles = new int[8, 10];
            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Cols; j++)
                {
                    Tiles[i, j] = 1;
                }
            }

            IsUnique = isUnique;
            Node = node;
        }

        public void ConstructRoom()
        {
            if (IsUnique)
            {
                for (var i = 1; i < Rows - 1; i++)
                {
                    for (var j = 1; j < Cols - 1; j++)
                    {
                        Tiles[i, j] = 0;
                    }
                }
            }
            else
            {
                var random = new Random();
                var digger = (3, 5);
                Tiles[digger.Item1, digger.Item2] = 0;
                var moveCount = 0;
                while (moveCount < 25)
                {
                    var d = random.NextDouble();
                    if (d < 0.25 && IsInBounds(digger.Item1 + 1, digger.Item2) &&
                        Tiles[digger.Item1 + 1, digger.Item2] == 1)
                    {
                        digger.Item1++;
                        Tiles[digger.Item1, digger.Item2] = 0;
                        moveCount++;
                    }
                    else if (d is >= 0.25 and < 0.5 && IsInBounds(digger.Item1 - 1, digger.Item2) &&
                             Tiles[digger.Item1 - 1, digger.Item2] == 1)
                    {
                        digger.Item1--;
                        Tiles[digger.Item1, digger.Item2] = 0;
                        moveCount++;
                    }
                    else if (d is >= 0.5 and < 0.75 && IsInBounds(digger.Item1, digger.Item2 + 1) &&
                             Tiles[digger.Item1, digger.Item2 + 1] == 1)
                    {
                        digger.Item2++;
                        Tiles[digger.Item1, digger.Item2] = 0;
                        moveCount++;
                    }
                    else if (IsInBounds(digger.Item1, digger.Item2 - 1) && Tiles[digger.Item1, digger.Item2 - 1] == 1)
                    {
                        digger.Item2--;
                        Tiles[digger.Item1, digger.Item2] = 0;
                        moveCount++;
                    }
                    else if (IsSurrounded(digger))
                    {
                        while (IsSurrounded(digger))
                        {
                            d = random.NextDouble();
                            if (d < 0.25 && IsInBounds(digger.Item1 + 1, digger.Item2))
                            {
                                digger.Item1++;
                            }
                            else if (d is >= 0.25 and < 0.5 && IsInBounds(digger.Item1 - 1, digger.Item2))
                            {
                                digger.Item1--;
                            }
                            else if (d is >= 0.5 and < 0.75 && IsInBounds(digger.Item1, digger.Item2 + 1))
                            {
                                digger.Item2++;
                            }
                            else if (IsInBounds(digger.Item1, digger.Item2 - 1))
                            {
                                digger.Item2--;
                            }
                        }
                    }
                }

                RemoveFloatingBlocks();
            }
        }

        private bool IsSurrounded((int, int) digger)
        {
            return (digger.Item1 == 7 || Tiles[digger.Item1 + 1, digger.Item2] == 0) &&
                   (digger.Item1 == 0 || Tiles[digger.Item1 - 1, digger.Item2] == 0) &&
                   (digger.Item2 == 9 || Tiles[digger.Item1, digger.Item2 + 1] == 0) &&
                   (digger.Item2 == 0 || Tiles[digger.Item1, digger.Item2 - 1] == 0);
        }

        private void RemoveFloatingBlocks()
        {
            for (var i = 1; i < Rows - 1; i++)
            {
                for (var j = 1; j < Cols - 1; j++)
                {
                    if (IsSurrounded((i, j)) && Tiles[i, j] == 1)
                    {
                        Tiles[i, j] = 0;
                    }
                }
            }
        }

        public void PlaceEnemies(int noItems, int noEnemies)
        {
            var random = new Random();
            while (noEnemies > 0)
            {
                var i = random.Next(0, 8);
                var j = random.Next(0, 10);
                if (Tiles[i, j] == 0)
                {
                    Tiles[i, j] = 2;
                    noEnemies--;
                }
            }
        }

        public void MakeConnections(List<(int, Room)> neighbours)
        {
            var up = false;
            var down = false;
            var left = false;
            var right = false;

            foreach (var neighbour in neighbours)
            {
                var direction = "";
                switch (neighbour.Item1 - Node.Id)
                {
                    case 1:
                        right = true;
                        direction = "right";
                        break;
                    case -1:
                        direction = "left";
                        left = true;
                        break;
                    case -4:
                        direction = "up";
                        up = true;
                        break;
                    case 4:
                        direction = "down";
                        down = true;
                        break;
                }

                MakeConnection(direction);
            }

            if (!up)
            {
                for (var i = 0; i < Cols; i++)
                {
                    Tiles[0, i] = 1;
                }
            }

            if (!down)
            {
                for (var i = 0; i < Cols; i++)
                {
                    Tiles[Rows - 1, i] = 1;
                }
            }

            if (!left)
            {
                for (var i = 0; i < Rows; i++)
                {
                    Tiles[i, 0] = 1;
                }
            }

            if (!right)
            {
                for (var i = 0; i < Rows; i++)
                {
                    Tiles[i, Cols - 1] = 1;
                }
            }
        }

        private void MakeConnection(string direction)
        {
            int i;
            switch (direction)
            {
                case "up":
                    i = 0;
                    while (i < Rows - 1 && (Tiles[i, 4] == 1 || Tiles[i, 5] == 1))
                    {
                        Tiles[i, 4] = 0;
                        Tiles[i, 5] = 0;
                        i++;
                    }

                    break;
                case "down":
                    i = Rows - 1;
                    while (i > 0 && (Tiles[i, 4] == 1 || Tiles[i, 5] == 1))
                    {
                        Tiles[i, 4] = 0;
                        Tiles[i, 5] = 0;
                        i--;
                    }

                    break;
                case "left":
                    i = 0;
                    while (i < Cols - 1 && (Tiles[4, i] == 1 || Tiles[5, i] == 1))
                    {
                        Tiles[4, i] = 0;
                        Tiles[5, i] = 0;
                        i++;
                    }

                    break;
                case "right":
                    i = Cols - 1;
                    while (i > 0 && (Tiles[4, i] == 1 || Tiles[5, i] == 1))
                    {
                        Tiles[4, i] = 0;
                        Tiles[5, i] = 0;
                        i--;
                    }

                    break;
            }
        }

        /*private void MakeConnection(Room neighbour, string direction)
        {
            bool neighbourHasPath;
            var nodeHasPath = IsPath(direction);
            
            switch (direction)
            {
                case "up":
                    neighbourHasPath = neighbour.IsPath("down");
                    if (neighbourHasPath && nodeHasPath)
                    {
                        var foundPath = false;
                        for (var i = 1; i < Cols - 1; i++)
                        {
                            if (Tiles[0, i] == 0 && neighbour.Tiles[Rows - 1, i] == 0)
                            {
                                foundPath = true;
                                break;
                            }
                        }

                        if (!foundPath)
                        {
                            for (var i = 1; i < Cols - 1; i++)
                            {
                                if (Tiles[0, i] == 0)
                                {
                                    for (var j = 1; j < Cols - 1; j++)
                                    {
                                        if (neighbour.Tiles[Rows - 1, j] == 0)
                                        {
                                            var dif = j - i;
                                            switch (dif)
                                            {
                                                case < 0:
                                                {
                                                    for (var k = j; k <= i; k++)
                                                    {
                                                        Tiles[0, k] = 0;
                                                        neighbour.Tiles[Rows - 1, k] = 0;
                                                    }

                                                    break;
                                                }
                                                case > 0:
                                                {
                                                    for (var k = i; k <= j; k++)
                                                    {
                                                        Tiles[0, k] = 0;
                                                        neighbour.Tiles[Rows - 1, k] = 0;
                                                    }

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (nodeHasPath)
                    {
                        var minDistanceFromMiddle = (-1, -1);
                        for (var i = 1; i < Cols - 1; i++)
                        {
                            if (Tiles[0, i] == 0)
                            {
                                var distanceFromMiddle = Math.Abs(5 - i);
                                if (minDistanceFromMiddle.Item2 < 0 || distanceFromMiddle < minDistanceFromMiddle.Item2)
                                {
                                    minDistanceFromMiddle.Item1 = i;
                                    minDistanceFromMiddle.Item2 = distanceFromMiddle;
                                }
                            }
                        }

                        if (minDistanceFromMiddle.Item1 <= 5)
                        {
                            for (var i = minDistanceFromMiddle.Item1; i <= 5; i++)
                            {
                                Tiles[0, i] = 0;
                                neighbour.Tiles[Rows - 1, i] = 0;
                            }
                        }
                        else
                        {
                            for (var i = 5; i <= minDistanceFromMiddle.Item1; i++)
                            {
                                Tiles[0, i] = 0;
                                neighbour.Tiles[Rows - 1, i] = 0;
                            }
                        }

                        var j = Rows - 1;
                        while (j > 1 && (neighbour.Tiles[j, 4] == 1 || neighbour.Tiles[j, 5] == 1))
                        {
                            neighbour.Tiles[j, 4] = 0;
                            neighbour.Tiles[j, 5] = 0;
                            j--;
                        }
                    }
                    else if (neighbourHasPath)
                    {
                        var minDistanceFromMiddle = (-1, -1);
                        for (var i = 1; i < Cols - 1; i++)
                        {
                            if (neighbour.Tiles[Rows - 1, i] == 0)
                            {
                                var distanceFromMiddle = Math.Abs(5 - i);
                                if (minDistanceFromMiddle.Item2 < 0 || distanceFromMiddle < minDistanceFromMiddle.Item2)
                                {
                                    minDistanceFromMiddle.Item1 = i;
                                    minDistanceFromMiddle.Item2 = distanceFromMiddle;
                                }
                            }
                        }
                        
                        if (minDistanceFromMiddle.Item1 <= 5)
                        {
                            for (var i = minDistanceFromMiddle.Item1; i <= 5; i++)
                            {
                                neighbour.Tiles[Rows - 1, i] = 0;
                                Tiles[0, i] = 0;
                            }
                        }
                        else
                        {
                            for (var i = 5; i <= minDistanceFromMiddle.Item1; i++)
                            {
                                neighbour.Tiles[Rows - 1, i] = 0;
                                Tiles[0, i] = 0;
                            }
                        }

                        var j = 0;
                        while (j < Rows - 1 && (Tiles[j, 4] == 1 || Tiles[j, 5] == 1))
                        {
                            Tiles[j, 4] = 0;
                            Tiles[j, 5] = 0;
                            j++;
                        }
                    }
                    else
                    {
                        var i = 0;
                        while (i < Rows - 1 && (Tiles[i, 4] == 1 || Tiles[i, 5] == 1))
                        {
                            Tiles[i, 4] = 0;
                            Tiles[i, 5] = 0;
                            i++;
                        }
                    }
                    break;
                case "down":
                    neighbourHasPath = neighbour.IsPath("up");
                    if (neighbourHasPath && nodeHasPath)
                    {
                        var foundPath = false;
                        for (var i = 1; i < Cols - 1; i++)
                        {
                            if (Tiles[Rows - 1, i] == 0 && neighbour.Tiles[0, i] == 0)
                            {
                                foundPath = true;
                                break;
                            }
                        }

                        if (!foundPath)
                        {
                            for (var i = 1; i < Cols - 1; i++)
                            {
                                if (Tiles[Rows - 1, i] == 0)
                                {
                                    for (var j = 1; j < Cols - 1; j++)
                                    {
                                        if (neighbour.Tiles[0, j] == 0)
                                        {
                                            var dif = j - i;
                                            switch (dif)
                                            {
                                                case < 0:
                                                    for (var k = j; k <= i; k++)
                                                    {
                                                        Tiles[Rows - 1, k] = 0;
                                                        neighbour.Tiles[0, k] = 0;
                                                    }
                                                    break;
                                                case > 0:
                                                    for (var k = i; k <= j; k++)
                                                    {
                                                        Tiles[Rows - 1, k] = 0;
                                                        neighbour.Tiles[0, k] = 0;
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (nodeHasPath)
                    {
                        var minDistanceFromMiddle = (-1, -1);
                        for (var i = 1; i < Cols - 1; i++)
                        {
                            if (Tiles[Rows - 1, i] == 0)
                            {
                                var distanceFromMiddle = Math.Abs(5 - i);
                                if (minDistanceFromMiddle.Item2 < 0 || distanceFromMiddle < minDistanceFromMiddle.Item2)
                                {
                                    minDistanceFromMiddle.Item1 = i;
                                    minDistanceFromMiddle.Item2 = distanceFromMiddle;
                                }
                            }
                        }

                        if (minDistanceFromMiddle.Item1 <= 5)
                        {
                            for (var i = minDistanceFromMiddle.Item1; i <= 5; i++)
                            {
                                Tiles[Rows - 1, i] = 0;
                                neighbour.Tiles[0, i] = 0;
                            }
                        }
                        else
                        {
                            for (var i = 5; i <= minDistanceFromMiddle.Item1; i++)
                            {
                                Tiles[Rows - 1, i] = 0;
                                neighbour.Tiles[0, i] = 0;
                            }
                        }

                        var j = 0;
                        while (j < Rows - 1 && (neighbour.Tiles[j, 4] == 1 || neighbour.Tiles[j, 5] == 1))
                        {
                            neighbour.Tiles[j, 4] = 0;
                            neighbour.Tiles[j, 5] = 0;
                            j++;
                        }
                    }
                    else if (neighbourHasPath)
                    {
                        var minDistanceFromMiddle = (-1, -1);
                        for (var i = 1; i < Cols - 1; i++)
                        {
                            if (neighbour.Tiles[0, i] == 0)
                            {
                                var distanceFromMiddle = Math.Abs(5 - i);
                                if (minDistanceFromMiddle.Item2 < 0 || distanceFromMiddle < minDistanceFromMiddle.Item2)
                                {
                                    minDistanceFromMiddle.Item1 = i;
                                    minDistanceFromMiddle.Item2 = distanceFromMiddle;
                                }
                            }
                        }

                        if (minDistanceFromMiddle.Item1 <= 5)
                        {
                            for (var i = minDistanceFromMiddle.Item1; i <= 5; i++)
                            {
                                neighbour.Tiles[0, i] = 0;
                                Tiles[Rows - 1, i] = 0;
                            }
                        }
                        else
                        {
                            for (var i = 5; i <= minDistanceFromMiddle.Item1; i++)
                            {
                                neighbour.Tiles[0, i] = 0;
                                Tiles[Rows - 1, i] = 0;
                            }
                        }

                        var j = Rows - 1;
                        while (j > 1 && (Tiles[j, 4] == 1 || Tiles[j, 5] == 1))
                        {
                            Tiles[j, 4] = 0;
                            Tiles[j, 5] = 0;
                            j--;
                        }
                    }
                    else
                    {
                        var i = Rows - 1;
                        while (i > 0 && (Tiles[i, 4] == 1 || Tiles[i, 5] == 1))
                        {
                            Tiles[i, 4] = 0;
                            Tiles[i, 5] = 0;
                            i--;
                        }
                    }
                    break;
                case "left":
                    neighbourHasPath = neighbour.IsPath("right");
                    if (neighbourHasPath && nodeHasPath)
                    {
                        var foundPath = false;
                        for (var i = 1; i < Rows - 1; i++)
                        {
                            if (Tiles[i, 0] == 0 && neighbour.Tiles[i, Cols - 1] == 0)
                            {
                                foundPath = true;
                                break;
                            }
                        }

                        if (!foundPath)
                        {
                            for (var i = 1; i < Rows - 1; i++)
                            {
                                if (Tiles[i, 0] == 0)
                                {
                                    for (var j = 1; j < Rows - 1; j++)
                                    {
                                        if (neighbour.Tiles[j, Cols - 1] == 0)
                                        {
                                            var dif = j - i;
                                            switch (dif)
                                            {
                                                case < 0:
                                                    for (var k = j; k <= i; k++)
                                                    {
                                                        Tiles[k, 0] = 0;
                                                        neighbour.Tiles[k, Cols - 1] = 0;
                                                    }
                                                    break;
                                                case > 0:
                                                    for (var k = j; k <= i; k++)
                                                    {
                                                        Tiles[k, 0] = 0;
                                                        neighbour.Tiles[k, Cols - 1] = 0;
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (nodeHasPath)
                    {
                        var minDistanceFromMiddle = (-1, -1);
                        for (var i = 1; i < Rows - 1; i++)
                        {
                            if (Tiles[i, 0] == 0)
                            {
                                var distanceFromMiddle = Math.Abs(4 - i);
                                if (minDistanceFromMiddle.Item2 < 0 || distanceFromMiddle < minDistanceFromMiddle.Item2)
                                {
                                    minDistanceFromMiddle.Item1 = i;
                                    minDistanceFromMiddle.Item2 = distanceFromMiddle;
                                }
                            }
                        }

                        if (minDistanceFromMiddle.Item1 <= 4)
                        {
                            for (var i = minDistanceFromMiddle.Item1; i <= 4; i++)
                            {
                                Tiles[i, 0] = 0;
                                neighbour.Tiles[i, Cols - 1] = 0;
                            }
                        }
                        else
                        {
                            for (var i = 4; i <= minDistanceFromMiddle.Item1; i++)
                            {
                                Tiles[i, 0] = 0;
                                neighbour.Tiles[i, Cols - 1] = 0;
                            }
                        }

                        var j = Cols - 1;
                        while (j > 1 && (neighbour.Tiles[4, j] == 1 || neighbour.Tiles[5, j] == 1))
                        {
                            neighbour.Tiles[4, j] = 0;
                            neighbour.Tiles[5, j] = 0;
                            j--;
                        }
                    }
                    else if (neighbourHasPath)
                    {
                        var minDistanceFromMiddle = (-1, -1);
                        for (var i = 1; i < Rows - 1; i++)
                        {
                            if (neighbour.Tiles[i, Cols - 1] == 0)
                            {
                                var distanceFromMiddle = Math.Abs(4 - i);
                                if (minDistanceFromMiddle.Item2 < 0 || distanceFromMiddle < minDistanceFromMiddle.Item2)
                                {
                                    minDistanceFromMiddle.Item1 = i;
                                    minDistanceFromMiddle.Item2 = distanceFromMiddle;
                                }
                            }
                        }

                        if (minDistanceFromMiddle.Item1 <= 4)
                        {
                            for (var i = minDistanceFromMiddle.Item1; i <= 4; i++)
                            {
                                neighbour.Tiles[i, Cols - 1] = 0;
                                Tiles[i, 0] = 0;
                            }
                        }
                        else
                        {
                            for (var i = 4; i <= minDistanceFromMiddle.Item1; i++)
                            {
                                neighbour.Tiles[i, Cols - 1] = 0;
                                Tiles[i, 0] = 0;
                            }
                        }

                        var j = 0;
                        while (j < Cols - 1 && (Tiles[4, j] == 1 || Tiles[5, j] == 1))
                        {
                            Tiles[4, j] = 0;
                            Tiles[5, j] = 0;
                            j++;
                        }
                    }
                    else
                    {
                        var i = 0;
                        while (i < Cols - 1 && (Tiles[4, i] == 1 || Tiles[5, i] == 1))
                        {
                            Tiles[4, i] = 0;
                            Tiles[5, i] = 0;
                            i++;
                        }
                    }
                    break;
                case "right":
                    neighbourHasPath = neighbour.IsPath("left");
                    if (neighbourHasPath && nodeHasPath)
                    {
                        var foundPath = false;
                        for (var i = 1; i < Rows - 1; i++)
                        {
                            if (Tiles[i, Cols - 1] == 0 && neighbour.Tiles[i, 0] == 0)
                            {
                                foundPath = true;
                                break;
                            }
                        }

                        if (!foundPath)
                        {
                            for (var i = 1; i < Rows - 1; i++)
                            {
                                if (Tiles[i, Cols - 1] == 0)
                                {
                                    for (var j = 1; j < Rows - 1; j++)
                                    {
                                        if (neighbour.Tiles[j, 0] == 0)
                                        {
                                            var dif = j - i;
                                            switch (dif)
                                            {
                                                case < 0:
                                                    for (var k = j; k <= i; k++)
                                                    {
                                                        Tiles[k, Cols - 1] = 0;
                                                        neighbour.Tiles[k, 0] = 0;
                                                    }
                                                    break;
                                                case > 0:
                                                    for (var k = j; k <= i; k++)
                                                    {
                                                        Tiles[k, Cols - 1] = 0;
                                                        neighbour.Tiles[k, 0] = 0;
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (nodeHasPath)
                    {
                        var minDistanceFromMiddle = (-1, -1);
                        for (var i = 1; i < Rows - 1; i++)
                        {
                            if (Tiles[i, Cols - 1] == 0)
                            {
                                var distanceFromMiddle = Math.Abs(4 - i);
                                if (minDistanceFromMiddle.Item2 < 0 || distanceFromMiddle < minDistanceFromMiddle.Item2)
                                {
                                    minDistanceFromMiddle.Item1 = i;
                                    minDistanceFromMiddle.Item2 = distanceFromMiddle;
                                }
                            }
                        }

                        if (minDistanceFromMiddle.Item1 <= 4)
                        {
                            for (var i = minDistanceFromMiddle.Item1; i <= 4; i++)
                            {
                                Tiles[i, Cols - 1] = 0;
                                neighbour.Tiles[i, 0] = 0;
                            }
                        }
                        else
                        {
                            for (var i = 4; i <= minDistanceFromMiddle.Item1; i++)
                            {
                                Tiles[i, Cols - 1] = 0;
                                neighbour.Tiles[i, 0] = 0;
                            }
                        }

                        var j = 0;
                        while (j < Cols - 1 && (neighbour.Tiles[4, j] == 1 || neighbour.Tiles[5, j] == 1))
                        {
                            neighbour.Tiles[4, j] = 0;
                            neighbour.Tiles[5, j] = 0;
                            j++;
                        }
                    }
                    else if (neighbourHasPath)
                    {
                        var minDistanceFromMiddle = (-1, -1);
                        for (var i = 1; i < Rows - 1; i++)
                        {
                            if (neighbour.Tiles[i, 0] == 0)
                            {
                                var distanceFromMiddle = Math.Abs(4 - i);
                                if (minDistanceFromMiddle.Item2 < 0 || distanceFromMiddle < minDistanceFromMiddle.Item2)
                                {
                                    minDistanceFromMiddle.Item1 = i;
                                    minDistanceFromMiddle.Item2 = distanceFromMiddle;
                                }
                            }
                        }

                        if (minDistanceFromMiddle.Item1 <= 4)
                        {
                            for (var i = minDistanceFromMiddle.Item1; i <= 4; i++)
                            {
                                neighbour.Tiles[i, 0] = 0;
                                Tiles[i, Cols - 1] = 0;
                            }
                        }
                        else
                        {
                            for (var i = 4; i <= minDistanceFromMiddle.Item1; i++)
                            {
                                neighbour.Tiles[i, 0] = 0;
                                Tiles[i, Cols - 1] = 0;
                            }
                        }

                        var j = Cols - 1;
                        while (j > 1 && (Tiles[4, j] == 1 || Tiles[5, j] == 1))
                        {
                            Tiles[4, j] = 0;
                            Tiles[5, j] = 0;
                            j--;
                        }
                    }
                    else
                    {
                        var i = Cols - 1;
                        while (i > 0 && (Tiles[4, i] == 1 || Tiles[5, i] == 1))
                        {
                            Tiles[4, i] = 0;
                            Tiles[5, i] = 0;
                            i--;
                        }
                    }
                    break;
            }
        }*/

        private bool IsPath(string direction)
        {
            var entryPoints = GetEntrancePoints(direction);
            var exitPoints = GetExitPoints();

            if (entryPoints.Count == 0)
            {
                return false;
            }

            foreach (var entryPoint in entryPoints)
            {
                if (BFS(entryPoint, exitPoints))
                {
                    return true;
                }
            }

            return false;
        }

        private List<(int, int)> GetEntrancePoints(string direction)
        {
            var entrancePoints = new List<(int, int)>();
            switch (direction)
            {
                case "up":
                    for (var i = 1; i < Cols - 1; i++)
                    {
                        if (Tiles[0, i] == 0)
                        {
                            entrancePoints.Add((0, i));
                        }
                    }

                    break;
                case "down":
                    for (var i = 1; i < Cols - 1; i++)
                    {
                        if (Tiles[Rows - 1, i] == 0)
                        {
                            entrancePoints.Add((Rows - 1, i));
                        }
                    }

                    break;
                case "left":
                    for (var i = 1; i < Rows - 1; i++)
                    {
                        if (Tiles[i, 0] == 0)
                        {
                            entrancePoints.Add((i, 0));
                        }
                    }

                    break;
                case "right":
                    for (var i = 1; i < Rows - 1; i++)
                    {
                        if (Tiles[i, Cols - 1] == 0)
                        {
                            entrancePoints.Add((i, Cols - 1));
                        }
                    }

                    break;
            }

            return entrancePoints;
        }

        private List<(int, int)> GetExitPoints()
        {
            const int midRowStart = 3;
            const int midRowEnd = 5;
            const int midColStart = 4;
            const int midColEnd = 5;
            var exitPoints = new List<(int, int)>();

            for (var i = midRowStart; i <= midRowEnd; i++)
            {
                for (var j = midColStart; j <= midColEnd; j++)
                {
                    if (Tiles[i, j] == 0)
                    {
                        exitPoints.Add((i, j));
                    }
                }
            }

            return exitPoints;
        }

        private bool BFS((int, int) start, ICollection<(int, int)> exits)
        {
            var queue = new Queue<(int, int)>();
            var visited = new bool[Rows, Cols];
            int[] rowDirection = { -1, 1, 0, 0 };
            int[] colDirection = { 0, 0, -1, 1 };

            queue.Enqueue(start);
            visited[start.Item1, start.Item2] = true;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var currentRow = current.Item1;
                var currentColumn = current.Item2;

                if (exits.Contains(current))
                {
                    return true;
                }

                for (var i = 0; i < 4; i++)
                {
                    var newRow = currentRow + rowDirection[i];
                    var newColumn = currentColumn + colDirection[i];

                    if (!IsInBounds(newRow, newColumn) || visited[newRow, newColumn] ||
                        Tiles[newRow, newColumn] == 1) continue;
                    queue.Enqueue((newRow, newColumn));
                    visited[newRow, newColumn] = true;
                }
            }

            return false;
        }

        private static bool IsInBounds(int row, int col)
        {
            return row is >= 0 and < Rows && col is >= 0 and < Cols;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < Rows; i++)
            {
                for (var j = 0; j < Cols; j++)
                {
                    stringBuilder.Append(Tiles[i, j] + " ");
                }

                stringBuilder.Append('\n');
            }

            return stringBuilder.ToString();
        }
    }
}