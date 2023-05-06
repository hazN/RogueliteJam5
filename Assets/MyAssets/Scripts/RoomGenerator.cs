using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class RoomGenerator : MonoBehaviour
{
    // Prefabs
    [SerializeField] private GameObject[] floorPrefabs;

    [SerializeField] private GameObject[] wallPrefabs;
    [SerializeField] private GameObject[] mushroomPrefabs;
    [SerializeField] private GameObject[] tablePrefabs;
    [SerializeField] private GameObject[] lootPrefabs;
    [SerializeField] private GameObject[] coinPrefabs;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject holePrefab;
    [SerializeField] private GameObject roofPrefab;
    [SerializeField] private TextMeshProUGUI roomTextNumber;
    public int roomNumber = 0;
    private Door.RoomType roomType = Door.RoomType.Basic;
    private int roomLevel = 1;
    private GameObject[] doors;
    private GameObject entranceDoor;

    // Store roomcontent so its easy to destroy later
    private GameObject roomContent;

    // Room generation settings
    [SerializeField] private float tileSize = 4.9f;
    [SerializeField] private int roomWidth;
    [SerializeField] private int roomHeight;
    [SerializeField] private  float holeProbability = 0.1f;

    // Navmesh
    [SerializeField] private NavMeshSurface surface;

    [SerializeField] private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
    }

    private void InstantiateFloorTiles(Vector3 position)
    {
        List<Vector2Int> holePositions = new List<Vector2Int>();
        // Generate hole positions
        for (int i = 0; i < roomWidth * roomHeight * holeProbability; i++)
        {
            int x = Random.Range(1, roomWidth - 1);
            int y = Random.Range(1, roomHeight - 1);

            // Check if the hole is not under a door
            if (x == roomWidth / 2 && y == -1) continue;
            if (x == roomWidth / 2 && y == 0) continue;

            Vector2Int holePosition = new Vector2Int(x, y);

            if (!holePositions.Contains(holePosition))
            {
                // Make sure holes are not placed next to doors
                bool placeHole = true;
                // Find and loop through all doors
                doors = GameObject.FindGameObjectsWithTag("Door");
                foreach (GameObject door in doors)
                {
                    // If the hole is next to a door, don't place it
                    if (Vector3.Distance(door.transform.position, new Vector3(holePosition.x, 0f, holePosition.y)) <tileSize)
                    {
                        placeHole = false;
                        break;
                    }
                }
                if (placeHole)
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
                Vector3 tilePosition = position + new Vector3(x *tileSize, 0, y *tileSize);
                if (holePositions.Contains(new Vector2Int(x, y)))
                {
                    // Hole
                }
                else
                {
                    GameObject floorPrefab = floorPrefabs[Random.Range(0, floorPrefabs.Length)];
                    Instantiate(floorPrefab, tilePosition, Quaternion.identity, roomContent.transform);
                }
            }
        }

        //// Instantiate roof tiles
        //for (int x = 0; x < roomWidth; x++)
        //{
        //    for (int y = 0; y < roomHeight; y++)
        //    {
        //        Vector3 tilePosition = position + new Vector3(x *tileSize,tileSize, y *tileSize);
        //        GameObject roofTile = Instantiate(roofPrefab, tilePosition, Quaternion.identity, roomContent.transform);
        //    }
        //}
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
                        x *tileSize + ((x == roomWidth) ? -5f : 0),
                        h *tileSize,
                        y *tileSize + ((y == roomHeight) ? 0f :tileSize));

                    GameObject prefabToInstantiate = wallPrefabs[Random.Range(0, wallPrefabs.Length)];
                    Quaternion prefabRotation = Quaternion.identity;

                    if (h == 0 && !doorPlaced)
                    {
                        // Place the entrance door (inactive) at the center of the first wall
                        if (x == roomWidth / 2 && y == -1)
                        {
                            prefabToInstantiate = doorPrefab;
                            prefabToInstantiate.transform.position += transform.right * 2.5f;
                            prefabToInstantiate.GetComponent<Door>().isActive = false;
                            prefabToInstantiate.layer = 0;
                            doorPlaced = true;
                        }
                        // Place a door on the other walls
                        else if (doorWalls.Contains(GetWallNumber(x, y, roomWidth, roomHeight)))
                        {
                            prefabToInstantiate = doorPrefab;
                            prefabToInstantiate.transform.position += transform.right * 2.5f;
                            prefabToInstantiate.GetComponent<Door>().isActive = true;
                            prefabToInstantiate.layer = 11;
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

                    GameObject wall = Instantiate(prefabToInstantiate, wallPosition, prefabRotation, roomContent.transform);
                    if (doorPlaced && h == 0)
                    {
                        wall.transform.position += wall.transform.right * -2.5f;
                        if (wall.layer == 0)
                        {
                            entranceDoor = wall;
                            entranceDoor.name = "EntranceDoor";
                        }
                    }
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

    public void GenerateRoom(Door.RoomType _roomType, int _roomLevel, Vector3 _position)
    {
        roomNumber++;
        roomTextNumber.text = roomNumber.ToString();
        player.gameObject.SetActive(false);
        if (roomContent)
            Destroy(roomContent);
        roomContent = new GameObject("RoomContent");
        roomContent.transform.parent = GameObject.Find("DungeonManager").transform;

        // Randomize width based on room level
        int[] minSizes = { 3, 4, 6 };
        int[] maxSizes = { 4, 6, 10 };
        int minSize = minSizes[roomLevel - 1];
        int maxSize = maxSizes[roomLevel - 1];
        roomWidth = Random.Range(minSize, maxSize + 1);
        roomHeight = Random.Range(minSize, maxSize + 1);

        InstantiateFloorTiles(_position);
        InstantiateWalls(_position);

        // Instantiate room content based on the selected room type
        roomLevel = _roomLevel;
        roomType = _roomType;
        InstantiateRoomContent(_position, roomType, roomLevel);

        // Rebuild navmesh to include content like tables
        surface.BuildNavMesh();
        // Spawn enemies
        InstantiateEnemies(_position);
        player.gameObject.SetActive(true);

        // Move player to entrance of the room
        doors = GameObject.FindGameObjectsWithTag("Door");
        GameObject playerArmature = GameObject.Find("PlayerArmature");
        // Need to disable character controller as it was interfering with changing the transform
        CharacterController cc = playerArmature.GetComponent<CharacterController>();
        cc.enabled = false;
        playerArmature.transform.position = new Vector3(entranceDoor.transform.position.x, 1f, entranceDoor.transform.position.z);
        playerArmature.transform.rotation = entranceDoor.transform.rotation;
        playerArmature.transform.position += playerArmature.transform.forward * 2.5f;
        cc.enabled = true;
        foreach (GameObject door in doors)
        {
            door.GetComponent<Door>().RoomCompleted();
        }
    }

    private void InstantiateRoomContent(Vector3 position, Door.RoomType roomType, int roomLevel)
    {
        // Generate navmesh to use to find valid points to place stuff
        //surface.BuildNavMesh();

        int mushroomCount = Random.Range(roomWidth + roomHeight, roomWidth * roomHeight);
        int coinCount = Random.Range(roomWidth + roomHeight, roomWidth * roomHeight);
        int tableCount = Random.Range(roomWidth + roomHeight, roomWidth * roomHeight) / 2;

        switch (roomType)
        {
            case Door.RoomType.Basic:
                // Default amount of mushrooms, coins and loot
                break;

            case Door.RoomType.Health:
                // More mushrooms, less coins and loot
                mushroomCount = Mathf.RoundToInt(mushroomCount * 2.5f);
                coinCount = Mathf.RoundToInt(coinCount * 0.5f);
                tableCount = Mathf.RoundToInt(tableCount * 0.8f);
                break;

            case Door.RoomType.Gold:
                // More coins, less mushrooms and loot
                mushroomCount = Mathf.RoundToInt(mushroomCount * 0.5f);
                coinCount = Mathf.RoundToInt(coinCount * 2.5f);
                tableCount = Mathf.RoundToInt(tableCount * 0.8f);
                break;

            case Door.RoomType.Loot:
                // More loot, less mushrooms and coins
                mushroomCount = Mathf.RoundToInt(mushroomCount * 0.8f);
                coinCount = Mathf.RoundToInt(coinCount * 0.8f);
                tableCount = Mathf.RoundToInt(tableCount * 2.5f);
                break;
        }

        // Instantiate mushrooms
        for (int i = 0; i < mushroomCount; i++)
        {
            GameObject mushroomPrefab = mushroomPrefabs[Random.Range(0, mushroomPrefabs.Length)];

            // Generate a random position within the room bounds
            Vector3 mushroomPosition = position + new Vector3(Random.Range(-roomWidth / 2f, roomWidth / 2f) * 10f,
                0f,
                Random.Range(-roomHeight / 2f, roomHeight / 2f) * 10f);

            // Sample to a valid point on the navsurface
            NavMeshHit hit;
            if (NavMesh.SamplePosition(mushroomPosition, out hit,tileSize, NavMesh.AllAreas))
            {
                mushroomPosition = hit.position;// + new Vector3(0f, 0f, 0f);
                Instantiate(mushroomPrefab, mushroomPosition, Quaternion.identity, roomContent.transform);
            }
        }

        // Instantiate coins
        for (int i = 0; i < coinCount; i++)
        {
            GameObject coinPrefab = coinPrefabs[Random.Range(0, coinPrefabs.Length)];

            // Generate a random position within the room bounds
            Vector3 coinPosition = position + new Vector3(
                Random.Range(-roomWidth / 2f, roomWidth / 2f) * 10f,
                0f,
                Random.Range(-roomHeight / 2f, roomHeight / 2f) * 10f);

            // Sample to a valid point on the navsurface
            NavMeshHit hit;
            if (NavMesh.SamplePosition(coinPosition, out hit,tileSize, NavMesh.AllAreas))
            {
                coinPosition = hit.position + new Vector3(0f, 0f, 0f);
                Instantiate(coinPrefab, coinPosition, Quaternion.identity, roomContent.transform);
            }
        }

        // Instantiate tables and loot
        for (int i = 0; i < tableCount; i++)
        {
            GameObject tablePrefab = tablePrefabs[Random.Range(0, tablePrefabs.Length)];

            // Determine which edge of the room to place the table on
            float x = 0f, z = 0f;
            int edge = Random.Range(0, 4);
            switch (edge)
            {
                case 0: // Top edge
                    z = roomHeight / 2f * 10f;
                    x = Random.Range(-roomWidth / 2.1f, roomWidth / 2.1f) * 10f;
                    break;

                case 1: // Right edge
                    x = roomWidth / 2f * 10f;
                    z = Random.Range(-roomHeight / 2.1f, roomHeight / 2.1f) * 10f;
                    break;

                case 2: // Bottom edge
                    z = -roomHeight / 2f * 10f;
                    x = Random.Range(-roomWidth / 2.1f, roomWidth / 2.1f) * 10f;
                    break;

                case 3: // Left edge
                    x = -roomWidth / 2f * 10f;
                    z = Random.Range(-roomHeight / 2.1f, roomHeight / 2.1f) * 10f;
                    break;
            }

            // Calculate table position and rotation
            Vector3 tablePosition = position + new Vector3(x, 0f, z);
            Quaternion tableRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            // Sample a valid position on the navmesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(tablePosition, out hit,tileSize, NavMesh.AllAreas))
            {
                tablePosition = hit.position + new Vector3(0f, -0.1f, 0f);
                GameObject table = Instantiate(tablePrefab, tablePosition, tableRotation, roomContent.transform);

                // Spawn loot on the table
                if (Random.value < 0.8f)
                {
                    // Get the table's bounds
                    Bounds tableBounds = table.GetComponent<MeshRenderer>().bounds;
                    Vector3 lootPosition = new Vector3(table.transform.position.x, table.transform.position.y + tableBounds.extents.y * 2f, table.transform.position.z);
                    // Instantiate the loot at the position and randomize rotation
                    GameObject lootPrefab = lootPrefabs[Random.Range(0, lootPrefabs.Length)];
                    Quaternion lootRotation = Quaternion.Euler(-90f, Random.Range(0f, 360f), 0f);
                    lootPrefab.GetComponent<Rigidbody>().isKinematic = true;
                    Instantiate(lootPrefab, lootPosition, lootRotation, table.transform);
                }
            }
        }
    }

    private void InstantiateEnemies(Vector3 position)
    {
        int enemyCount = 0;
        switch (roomType)
        {
            default:
                {
                    // Default amount of enemies
                    enemyCount = Random.Range(roomWidth + roomHeight, roomWidth * roomHeight);
                    break;
                }
        }

        // Probability to spawn each enemy type
        float level1Prob = 0.7f - (0.05f * roomNumber);
        float level2Prob = 0.25f + (0.025f * roomNumber);
        float level3Prob = 0.05f + (0.025f * roomNumber);

        // Normalize probabilities
        float sum = level1Prob + level2Prob + level3Prob;
        level1Prob /= sum;
        level2Prob /= sum;
        level3Prob /= sum;

        // Instantiate the enemies
        for (int i = 0; i < enemyCount; i++)
        {
            // Generate a random position within the room bounds
            Vector3 enemyPosition = position + new Vector3(Random.Range(-roomWidth / 2.1f, roomWidth / 2.1f) * 10f,
                0f,
                Random.Range(-roomHeight / 2.1f, roomHeight / 2.1f) * 10f);

            // Sample to a valid point on the navsurface
            NavMeshHit hit;
            if (NavMesh.SamplePosition(enemyPosition, out hit,tileSize, NavMesh.AllAreas))
            {
                // Decide which enemy to instantiate based on the probabilities
                float rand = Random.value;
                GameObject enemy = null;
                if (rand < level1Prob)
                {
                    enemy = Instantiate(enemyPrefabs[0]);
                }
                else if (rand < level1Prob + level2Prob)
                {
                    enemy = Instantiate(enemyPrefabs[1]);
                }
                else
                {
                    enemy = Instantiate(enemyPrefabs[2]);
                }
                enemy.transform.position = enemyPosition;
                enemy.transform.parent = roomContent.transform;
                // Increase damage based on level too
                float damageFactor = 1.0f + (0.01f * roomNumber);
                enemy.GetComponentInChildren<Weapon>().damage *= damageFactor;
            }
        }
    }
}