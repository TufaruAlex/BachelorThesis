using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Pathfinding;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class GenerateDungeon : MonoBehaviour
{
    public Tile[] wallTiles;

    public Tile[] floorTiles;

    public GameObject bat;
    
    public GameObject player;
    
    private Individual _bestIndividual;

    // Start is called before the first frame update
    void Start()
    {
        var generations = RunGeneticAlgorithm();

        if (generations < 200)
        {
            Debug.Log(_bestIndividual.ToString());
            
            foreach (var node in _bestIndividual.Nodes)
            {
                Debug.Log(node.ToString());
            }
            
            ConstructDungeon();
        }
        else
        {
            while (generations >= 200)
            {
                generations = RunGeneticAlgorithm();
            }
            
            Debug.Log(_bestIndividual.ToString());
            
            foreach (var node in _bestIndividual.Nodes)
            {
                Debug.Log(node.ToString());
            }
            
            ConstructDungeon();
        }
    }

    private int RunGeneticAlgorithm ()
    {
        var ga = new GeneticAlgorithm(new System.Random(), 100, 0.05);
        while (ga.Generation < 200)
        {
            if (ga.BestIndividual.Fitness > 0.9)
            {
                _bestIndividual = ga.BestIndividual;
                _bestIndividual.ConstructLevel();
                break;
            }
            ga.NewGeneration();
        }

        return ga.Generation;
    }

    private void ConstructDungeon()
    {
        var rows = _bestIndividual.Level[0].Tiles.GetLength(0);
        var columns = _bestIndividual.Level[0].Tiles.GetLength(1);
        var noRooms = _bestIndividual.Level.Length;
        
        var grid = new GameObject("DungeonGrid").AddComponent<Grid>();

        var wallTilemap = new GameObject("WallTileMap").AddComponent<Tilemap>();
        wallTilemap.AddComponent<TilemapRenderer>();

        var pathTilemap = new GameObject("PathTileMap").AddComponent<Tilemap>();
        pathTilemap.AddComponent<TilemapRenderer>();
        pathTilemap.AddComponent<TilemapCollider2D>();
        var groundLayer = LayerMask.NameToLayer("Ground");
        pathTilemap.gameObject.layer = groundLayer;

        var enemyTileMap = new GameObject("EnemyTileMap").AddComponent<Tilemap>();
        enemyTileMap.AddComponent<TilemapRenderer>();
        
        grid.transform.SetParent(gameObject.transform);

        wallTilemap.transform.SetParent(grid.transform);
        pathTilemap.transform.SetParent(grid.transform);
        enemyTileMap.transform.SetParent(grid.transform);

        for (var i = 0; i < noRooms; i++)
        {
            var room = _bestIndividual.Level[i];
            for (var j = 0; j < rows; j++)
            {
                for (var k = 0; k < columns; k++)
                {
                    var rowIndex = (4 - i / 4) * rows - j;
                    var columnIndex = i % 4 * columns + k;
                    switch (room.Tiles[j, k])
                    {
                        case 0:
                        {
                            var tile = wallTiles[Random.Range(0, wallTiles.Length)];
                            wallTilemap.SetTile(new Vector3Int(columnIndex, rowIndex, 0), tile);
                            break;
                        }
                        case 1:
                        {
                            var tile = floorTiles[Random.Range(0, floorTiles.Length)];
                            pathTilemap.SetTile(new Vector3Int(columnIndex, rowIndex, 0), tile);
                            break;
                        }
                        case 2:
                        {
                            var tile = wallTiles[Random.Range(0, wallTiles.Length)];
                            wallTilemap.SetTile(new Vector3Int(columnIndex, rowIndex, 0), tile);
                            Instantiate(bat, new Vector3(columnIndex, rowIndex, 0), Quaternion.identity);
                            break;
                        }
                    }
                }
            }
        }
        
        PlacePlayer();
        Invoke(nameof(Scan), 0.1f);
        SetEnemyTargets();
    }

    private void Scan()
    {
        AstarPath.active.Scan();
    }

    private void PlacePlayer()
    {
        var rows = _bestIndividual.Level[0].Tiles.GetLength(0);
        var columns = _bestIndividual.Level[0].Tiles.GetLength(1);
        
        var startRoom = _bestIndividual.Genes[0];
        var randomStartPosition = Random.Range(1, 9);
        if (_bestIndividual.Level[startRoom].Tiles[rows - 1, randomStartPosition] == 1)
        {
            var rowIndex = (4 - (startRoom / 4 + 1)) * rows + 4;
            var columnIndex = startRoom % 4 * columns + randomStartPosition;
            Instantiate(player, new Vector3Int(columnIndex, rowIndex, 0), Quaternion.identity);
        }
        else
        {
            while (_bestIndividual.Level[startRoom].Tiles[rows - 1, randomStartPosition] != 1)
            {
                randomStartPosition = Random.Range(0, 10);
            }
            var rowIndex = (4 - (startRoom / 4 + 1)) * rows + 2;
            var columnIndex = startRoom % 4 * columns + randomStartPosition;
            Instantiate(player, new Vector3Int(columnIndex, rowIndex, 0), Quaternion.identity);
        }
    }

    private static void SetEnemyTargets()
    {
        var allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        var player = GameObject.FindGameObjectWithTag("Player");
        
        foreach (var enemy in allEnemies)
        {
            enemy.GetComponent<AIDestinationSetter>().target = player.transform;
            enemy.GetComponent<PathFindController>().playerTransform = player.transform;
        }
    }
}
