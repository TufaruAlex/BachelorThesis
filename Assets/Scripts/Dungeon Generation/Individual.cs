using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cinemachine;

namespace DefaultNamespace
{
    public class Individual
    {
        // array of size 6 which contains: the positions of the start room, boss room and lever room, the length of the
        //  path between the start and lever room, the number of items and the number of enemies of the level
        public int[] Genes { get; private set; }

        // TODO change comment
        public List<Node> Nodes { get; private set; }

        // the fitness value of the individual
        public double Fitness { get; private set; }

        // the level represented as a list of rooms
        public Room[] Level { get; private set; }

        private readonly Random _random;

        public Individual(Random random, bool shouldInitGenes = true)
        {
            Genes = new int[6];
            Nodes = new List<Node>(16);
            for (var i = 0; i < Nodes.Capacity; i++)
            {
                Nodes.Add(new Node(i));
            }

            Level = new Room[16];
            _random = random;

            if (shouldInitGenes)
            {
                GenerateRandomIndividual();
            }
        }

        public double CalculateFitness()
        {
            Fitness = 0f;
            var placementScoreStartRoom = 0d;
            if (Genes[0] <= 3)
            {
                placementScoreStartRoom = 1d;
            }

            var placementScoreBossRoom = 0d;
            if (Genes[1] <= 7)
            {
                placementScoreBossRoom = 1;
            }

            var placementScoreLeverRoom = 0;
            if (Genes[2] <= 15 && Genes[2] >= 12)
            {
                placementScoreLeverRoom = 1;
            }

            var pathFromStartToLeverScore = 0;
            if (Genes[3] > 0)
            {
                pathFromStartToLeverScore = 1;
            }

            var uniqueRoomsDistinctScore = 0;
            if (Genes[0] != Genes[1] && Genes[1] != Genes[2])
            {
                uniqueRoomsDistinctScore = 1;
            }

            var canReachBossScore = 0;
            if (CanReachBoss())
            {
                canReachBossScore = 1;
            }

            Fitness = 0.16 * placementScoreStartRoom + 0.16 * placementScoreBossRoom + 0.16 * placementScoreLeverRoom +
                      0.16 * pathFromStartToLeverScore + 0.16 * uniqueRoomsDistinctScore + 0.16 * canReachBossScore;
            return Fitness;
        }

        private bool CanReachBoss()
        {
            var startRoom = Genes[0];
            var bossRoom = Genes[1];
            var leverRoom = Genes[2];

            if (startRoom == bossRoom)
            {
                return true;
            }

            var visited = new bool[16];
            visited[startRoom] = true;
            visited[leverRoom] = true;

            return DFS(startRoom, visited, bossRoom);
        }

        private bool DFS(int v, IList<bool> visited, int bossRoom)
        {
            visited[v] = true;
            var adjacentRooms = GetAdjacentRooms(Nodes[v]);

            foreach (var room in adjacentRooms)
            {
                if (room.Item1 == bossRoom)
                {
                    return true;
                }

                if (!visited[room.Item1])
                {
                    return DFS(room.Item1, visited, bossRoom);
                }
            }

            return false;
        }

        // function that does the two-point crossover operation between this individual and another. The genes passed
        // down to the child are chosen randomly between the 2 parents
        public List<Individual> Crossover(Individual otherParent)
        {
            var children = new List<Individual>();
            var child1 = new Individual(_random, shouldInitGenes: false);
            var child2 = new Individual(_random, shouldInitGenes: false);
            var crossoverPoint1 = _random.Next(1, 3);
            var crossoverPoint2 = _random.Next(4, 6);

            for (var i = 0; i < crossoverPoint1; i++)
            {
                child1.Genes[i] = Genes[i];
                child2.Genes[i] = otherParent.Genes[i];
            }

            for (var i = crossoverPoint1; i < crossoverPoint2; i++)
            {
                child1.Genes[i] = otherParent.Genes[i];
                child2.Genes[i] = Genes[i];
            }

            for (var i = crossoverPoint2; i < Genes.Length; i++)
            {
                child1.Genes[i] = Genes[i];
                child2.Genes[i] = otherParent.Genes[i];
            }

            crossoverPoint1 = _random.Next(0, 7);
            crossoverPoint2 = _random.Next(crossoverPoint1 + 1, 15);

            for (var i = 0; i < crossoverPoint1; i++)
            {
                child1.Nodes[i] = Nodes[i];
                child2.Nodes[i] = otherParent.Nodes[i];
            }

            for (var i = crossoverPoint1; i < crossoverPoint2; i++)
            {
                child1.Nodes[i] = otherParent.Nodes[i];
                child2.Nodes[i] = Nodes[i];
            }

            for (var i = crossoverPoint2; i < Genes.Length; i++)
            {
                child1.Nodes[i] = Nodes[i];
                child2.Nodes[i] = otherParent.Nodes[i];
            }

            child1.Genes[3] = child1.ComputeDistance();
            child2.Genes[3] = child2.ComputeDistance();

            children.Add(child1);
            children.Add(child2);

            return children;
        }

        // function that mutates the individual based on the mutation rate. the fourth element is omitted because the 
        //  distance between the start room and the level room is computed based on their position
        public void Mutate(double mutationRate)
        {
            for (var i = 0; i < Genes.Length; i++)
            {
                if (_random.NextDouble() < mutationRate)
                {
                    if (i < 3)
                    {
                        Genes[i] = _random.Next(0, 15);
                    }
                    else if (i > 4)
                    {
                        Genes[i] = _random.Next();
                    }
                }
            }

            Genes[3] = ComputeDistance();

            foreach (var node in Nodes)
            {
                var adjacentNodesCopy = new Node[node.AdjacentNodes.Count];
                node.AdjacentNodes.CopyTo(adjacentNodesCopy);
                foreach (var adjacentNode in adjacentNodesCopy)
                {
                    if (_random.NextDouble() < mutationRate)
                    {
                        var randomNr = _random.Next(1, 3);
                        switch (randomNr)
                        {
                            case 1:
                                node.AdjacentNodes.Remove(adjacentNode);
                                break;
                            case 2:
                                var direction = _random.Next(1, 5);
                                switch (direction)
                                {
                                    case 1 when node.Id > 3:
                                    {
                                        if (node.Id - 4 != adjacentNode.Id)
                                        {
                                            node.AdjacentNodes.Remove(adjacentNode);
                                            node.AdjacentNodes.Add(Nodes[node.Id - 4]);
                                        }

                                        break;
                                    }
                                    case 2 when node.Id < 12:
                                    {
                                        if (node.Id + 4 != adjacentNode.Id)
                                        {
                                            node.AdjacentNodes.Remove(adjacentNode);
                                            node.AdjacentNodes.Add(Nodes[node.Id + 4]);
                                        }

                                        break;
                                    }
                                    case 3 when node.Id % 4 != 3:
                                    {
                                        if (node.Id + 1 != adjacentNode.Id)
                                        {
                                            node.AdjacentNodes.Remove(adjacentNode);
                                            node.AdjacentNodes.Add(Nodes[node.Id + 1]);
                                        }

                                        break;
                                    }
                                    case 4 when node.Id % 4 != 0:
                                    {
                                        if (node.Id - 1 != adjacentNode.Id)
                                        {
                                            node.AdjacentNodes.Remove(adjacentNode);
                                            node.AdjacentNodes.Add(Nodes[node.Id - 1]);
                                        }

                                        break;
                                    }
                                }

                                break;
                        }
                    }
                }
            }
        }

        public void ConstructLevel()
        {
            var startRoom = Genes[0];
            var connectedComponents = GetConnectedRooms();
            var mainComponent = new List<int>();
            
            for (var i = 0; i < Level.Length; i++)
            {
                Level[i] = new Room(Nodes[i], i == Genes[0] || i == Genes[1] || i == Genes[2]);
            }
            
            foreach (var component in connectedComponents)
            {
                if (component.Contains(startRoom))
                {
                    mainComponent = component;
                }
            }

            if (mainComponent.Count > 0)
            {
                foreach (var room in mainComponent)
                {
                    Level[room].ConstructRoom();
                }

                foreach (var room in mainComponent)
                {
                    Level[room].MakeConnections(GetAdjacentRooms(Level[room].Node));
                    if (!Level[room].IsUnique)
                    {
                        Level[room].PlaceEnemies(0, 3);
                    }
                }
            }
        }

        private List<List<int>> GetConnectedRooms()
        {
            var visited = new bool[16];
            var components = new List<List<int>>();

            for (var i = 0; i < 16; i++)
            {
                if (!visited[i])
                {
                    var component = new List<int>();
                    MakeComponent(i, visited, component);
                    components.Add(component);
                }
            }

            return components;
        }

        private void MakeComponent(int v, IList<bool> visited, ICollection<int> component)
        {
            visited[v] = true;
            component.Add(v);

            foreach (var adjacentRoom in GetAdjacentRooms(Nodes[v]))
            {
                if (!visited[adjacentRoom.Item1])
                {
                    MakeComponent(adjacentRoom.Item1, visited, component);
                }
            }
        }

        private List<(int, Room)> GetAdjacentRooms(Node node)
        {
            return node.AdjacentNodes.Select(adjacentNode => adjacentNode.Id).Select(id => (id, Level[id])).ToList();
        }

        // copy constructor
        public Individual(Individual individual)
        {
            Genes = individual.Genes;

            Nodes = individual.Nodes;

            Fitness = individual.Fitness;

            Level = individual.Level;

            _random = individual._random;
        }

        /*
         * Function that computes the distance between the start room and the lever room. It uses Breadth First Search
         * in order to find a path between the nodes. If one can't be found, the function returns -1
         */
        private int ComputeDistance()
        {
            var startRoom = Genes[0];
            var bossRoom = Genes[1];
            var leverRoom = Genes[2];

            if (startRoom == leverRoom)
            {
                return 0;
            }

            var distance = new Dictionary<Node, int>();
            var queue = new Queue<Node>();
            var visited = new HashSet<Node>();

            queue.Enqueue(Nodes[startRoom]);
            visited.Add(Nodes[startRoom]);
            visited.Add(Nodes[bossRoom]);
            distance[Nodes[startRoom]] = 0;

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                if (currentNode == Nodes[leverRoom])
                {
                    return distance[currentNode];
                }

                foreach (var neighbor in currentNode.AdjacentNodes.Where(neighbor => !visited.Contains(neighbor)))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    distance[neighbor] = distance[currentNode] + 1;
                }
            }

            return -1;
        }

        /*
         * Function that scores the connections as in how many are valid. A connection is valid if the 2 rooms are
         * adjacent in the level grid, therefore the absolute value of the difference must be 1 or 4.
         */
        private double CheckConnections()
        {
            double score = 1;
            foreach (var node in Nodes)
            {
                foreach (var adjacentNode in node.AdjacentNodes)
                {
                    if (Math.Abs(node.Id - adjacentNode.Id) != 4 ||
                        Math.Abs(node.Id - adjacentNode.Id) != 1)
                    {
                        score -= 0.03;
                    }
                }
            }

            return score <= 0.16 ? 0f : score;
        }

        private void GenerateRandomIndividual()
        {
            for (var i = 0; i < Genes.Length; i++)
            {
                Genes[i] = i switch
                {
                    <= 2 => _random.Next(1, 16),
                    > 4 => _random.Next(),
                    _ => Genes[i]
                };
            }

            var noConnections = 0;
            while (noConnections < 14)
            {
                var randomIndex = _random.Next(0, 16);
                var node = Nodes[randomIndex];
                var direction = _random.Next(1, 5);
                switch (direction)
                {
                    case 1 when node.Id > 3:
                    {
                        if (node.AdjacentNodes.Contains(Nodes[node.Id - 4])) continue;
                        node.AdjacentNodes.Add(Nodes[node.Id - 4]);
                        if (!Nodes[node.Id - 4].AdjacentNodes.Contains(node))
                        {
                            Nodes[node.Id - 4].AdjacentNodes.Add(node);
                        }

                        noConnections++;
                        break;
                    }
                    case 2 when node.Id < 12:
                    {
                        if (node.AdjacentNodes.Contains(Nodes[node.Id + 4])) continue;
                        node.AdjacentNodes.Add(Nodes[node.Id + 4]);
                        if (!Nodes[node.Id + 4].AdjacentNodes.Contains(node))
                        {
                            Nodes[node.Id + 4].AdjacentNodes.Add(node);
                        }

                        noConnections++;
                        break;
                    }
                    case 3 when node.Id % 4 != 3:
                    {
                        if (node.AdjacentNodes.Contains(Nodes[node.Id + 1])) continue;
                        node.AdjacentNodes.Add(Nodes[node.Id + 1]);
                        if (!Nodes[node.Id + 1].AdjacentNodes.Contains(node))
                        {
                            Nodes[node.Id + 1].AdjacentNodes.Add(node);
                        }

                        noConnections++;
                        break;
                    }
                    case 4 when node.Id % 4 != 0:
                    {
                        if (node.AdjacentNodes.Contains(Nodes[node.Id - 1])) continue;
                        node.AdjacentNodes.Add(Nodes[node.Id - 1]);
                        if (!Nodes[node.Id - 1].AdjacentNodes.Contains(node))
                        {
                            Nodes[node.Id - 1].AdjacentNodes.Add(node);
                        }

                        noConnections++;
                        break;
                    }
                }
            }

            Genes[3] = ComputeDistance();
            ConstructLevel();
        }

        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append("Start room: " + Genes[0] + "\nBoss room: " + Genes[1] + "\nLever room: " + Genes[2] +
                     "\nPath length: " + Genes[3] + "\n");
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    s.Append(Level[i * 4 + j]);
                }

                s.Append('\n');
            }

            return s.ToString();
        }
    }
}