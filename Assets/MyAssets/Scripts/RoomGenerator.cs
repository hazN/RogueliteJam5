using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.AI.Navigation;
using Unity.VisualScripting;

public class RoomGenerator : MonoBehaviour
{
    // Prefabs
    public GameObject[] floorPrefabs;

    public GameObject[] wallPrefabs;
    public GameObject doorPrefab;
    public GameObject holePrefab;
    // Add other prefabs for room content

    // Room generation settings
    public int roomWidth;

    public int roomHeight;
    public float holeProbability = 0.1f;

    // Navmesh
    [SerializeField] private NavMeshSurface surface;
    [SerializeField] private GameObject player;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        GenerateInitialRoom();
    }
    private void Update()
    {
       
    }
    private void GenerateInitialRoom()
    {
        GenerateRoom(Vector3.zero);
    }

    public void GenerateRoom(Vector3 position)
    {
        player.gameObject.SetActive(false);
        InstantiateFloorTiles(position);
        InstantiateWalls(position);
        surface.BuildNavMesh();
        player.gameObject.SetActive(true);
    }

    private void InstantiateFloorTiles(Vector3 position)
    {
        List<Vector2Int> holePositions = new List<Vector2Int>();
        // Generate hole positions
        for (int i = 0; i < roomWidth * roomHeight * holeProbability; i++)
        {
            int x = Random.Range(1, roomWidth - 1);
            int y = Random.Range(1, roomHeight - 1);
            Vector2Int holePosition = new Vector2Int(x, y);

            if (!holePositions.Contains(holePosition))
            {
                holePositions.Add(holePosition);
            }
        }

        // Merge adjacent holes
        for (int i = 0; i < holePositions.Count; i++)
        {
            Vector2Int holePosition = holePositions[i];

            if (Random.value < 0.5f)
            {
                Vector2Int adjacentPosition = holePosition + new Vector2Int(1, 0);
                if (!holePositions.Contains(adjacentPosition))
                {
                    holePositions.Add(adjacentPosition);
                }
            }

            if (Random.value < 0.5f)
            {
                Vector2Int adjacentPosition = holePosition + new Vector2Int(0, 1);
                if (!holePositions.Contains(adjacentPosition))
                {
                    holePositions.Add(adjacentPosition);
                }
            }
        }

        // Instantiate floor tiles
        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                Vector3 tilePosition = position + new Vector3(x * 5f, 0, y * 5f);
                if (holePositions.Contains(new Vector2Int(x, y)))
                {
                    // Hole
                }
                else
                {
                    GameObject floorPrefab = floorPrefabs[Random.Range(0, floorPrefabs.Length)];
                    Instantiate(floorPrefab, tilePosition, Quaternion.identity);
                }
            }
        }
    }

    private void InstantiateWalls(Vector3 position)
    {
        int wallHeight = 2;
        int exitDoorCount = (Random.value < 0.6f) ? 2 : (Random.value < 0.85f) ? 3 : 4;
        int[] availableWalls = { 1, 2, 3, 4 };
        List<int> doorWalls = new List<int>();

        for (int i = 0; i < exitDoorCount; i++)
        {
            int randomIndex = Random.Range(0, availableWalls.Length);
            doorWalls.Add(availableWalls[randomIndex]);
            availableWalls = availableWalls.Where((val, idx) => idx != randomIndex).ToArray();
        }

        for (int x = -1; x <= roomWidth; x++)
        {
            for (int y = -1; y <= roomHeight; y++)
            {
                if (x >= 0 && x < roomWidth && y >= 0 && y < roomHeight) continue;

                bool doorPlaced = false;

                for (int h = 0; h < wallHeight; h++)
                {
                    Vector3 wallPosition = position + new Vector3(
                        x * 5f + ((x == roomWidth) ? -5f : 0),
                        h * 5f,
                        y * 5f + ((y == roomHeight) ? 0f : 5f));

                    GameObject prefabToInstantiate = wallPrefabs[Random.Range(0, wallPrefabs.Length)];
                    Quaternion prefabRotation = Quaternion.identity;

                    if (h == 0 && !doorPlaced)
                    {
                        // Place the entrance door (inactive) at the center of the first wall
                        if (x == roomWidth / 2 && y == -1)
                        {
                            prefabToInstantiate = doorPrefab;
                            doorPlaced = true;
                        }
                        // Place a door on the other walls
                        else if (doorWalls.Contains(GetWallNumber(x, y, roomWidth, roomHeight)))
                        {
                            prefabToInstantiate = doorPrefab;
                            doorPlaced = true;
                        }
                    }

                    if (x == -1)
                    {
                        prefabRotation = Quaternion.Euler(0, 90, 0);
                    }
                    else if (x == roomWidth)
                    {
                        prefabRotation = Quaternion.Euler(0, -90, 0);
                    }
                    else if (y == -1)
                    {
                        prefabRotation = Quaternion.Euler(0, 0, 0);
                    }
                    else if (y == roomHeight)
                    {
                        prefabRotation = Quaternion.Euler(0, 180, 0);
                        wallPosition += new Vector3(-5f, 0f, 0f);
                    }

                    GameObject wall = Instantiate(prefabToInstantiate, wallPosition, prefabRotation);

                }
            }
        }
    }

    private int GetWallNumber(int x, int y, int roomWidth, int roomHeight)
    {
        if (x == -1 && y == roomHeight / 2) return 1;
        if (x == roomWidth && y == roomHeight / 2) return 2;
        if (x == roomWidth / 2 && y == -1) return 3;
        if (x == roomWidth / 2 && y == roomHeight) return 4;

        return 0;
    }
}