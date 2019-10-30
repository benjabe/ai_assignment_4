using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance { get; protected set; }

    // the size of the world (worldSize x worldSize tiles)
    [SerializeField] private int _worldSize = 8;

    // The graph that connects all non-tunnel tiles
    public TileGraph TileGraph { get; protected set; }

    // Invoked when the terrain updates
    public Action OnTerrainUpdate;

    // The default tile type to be created on world gen
    [SerializeField] private List<TileType> _defaultTileTypes = null;

    // The prefab to instantiate when creating tiles
    [SerializeField] private GameObject tilePrefab = null;

    // The parent transform for regular dungeon tiles
    [SerializeField] private Transform _tileParent = null;

    // reference to main camera so we can place tiles with mouse
    private Camera mainCamera;

    // maps position in world to an instantiated tile
    public Dictionary<Vector2Int, Tile> Tiles { get; protected set; }

    // True if terrain has been updated since previous frame
    private bool _terrainUpdated = false;


    public int WorldSize
    {
        get { return _worldSize; }
        protected set { _worldSize = value; }
    }


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            mainCamera = Camera.main;

            // Initiate world with only walls
            Initialize();
        }
        else
        {
            Debug.LogWarning("Tried to awake world while one already exists.");
        }
    }

    private void Update()
    {
        if (_terrainUpdated)
        {
            OnTerrainUpdate?.Invoke();
            _terrainUpdated = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Initialize();
        }
    }

    private void Initialize()
    {
        Tiles = new Dictionary<Vector2Int, Tile>();
        for (int x = 0; x < WorldSize; x++)
        {
            for (int y = 0; y < WorldSize; y++)
            {
                // Main world space
                Vector2 position = new Vector2(x, y);
                Vector2Int positionInt = new Vector2Int(x, y);
                Tiles[positionInt] = Instantiate(
                    tilePrefab,
                    position,
                    Quaternion.identity,
                    _tileParent
                ).GetComponent<Tile>();
                int tileTypeIndex =
                    UnityEngine.Random.Range(0, _defaultTileTypes.Count);
                ChangeTileType(
                    positionInt,
                    _defaultTileTypes[tileTypeIndex]
                );
            }
        }
        // Get a list of tiles to populate out tile graph
        List<Tile> tileList = new List<Tile>();
        foreach (Vector2Int position in Tiles.Keys)
        {
            tileList.Add(Tiles[position]);
        }
        TileGraph = new TileGraph(tileList);
        OnTerrainUpdate?.Invoke();
    }

    //Tells a particular tile to alter its tiletype
    public void ChangeTileType(Vector2Int position, TileType tileType)
    {
        if (position.x < 0 || position.y < 0 ||
            position.x >= WorldSize || position.y >= WorldSize)
        {
            return;
        }

        if (Tiles[position] != null)
        {
            if (Tiles[position].TileType != tileType)
            {
                Tiles[position].SetTileType(tileType);
                _terrainUpdated = true;
            }
        }
        else
        {
            Debug.LogWarning("No tile exists at " + position);
        }
    }

    public Vector2Int ScreenToWorldCoordinate(Vector2 screenCoordinate)
    {
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenCoordinate);
        Vector2Int coordinate = new Vector2Int(
            (int)Mathf.Round(worldPoint.x),
            (int)Mathf.Round(worldPoint.y)
        );
        return coordinate;
    }

    public Tile GetTileAtMousePosition()
    {
        Vector2Int worldPoint = MouseWorldPosition();

        if (worldPoint.x < 0 || worldPoint.y < 0 ||
            worldPoint.x >= WorldSize || worldPoint.y >= WorldSize)
        {
            return null;
        }
        return Tiles[worldPoint];
    }

    public Vector2Int MouseWorldPosition()
    {
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int coordinate = new Vector2Int(
            (int)Mathf.Round(worldPoint.x),
            (int)Mathf.Round(worldPoint.y)
        );
        return coordinate;
    }

    public Tile GetRandomTile()
    {
        Vector2Int position = new Vector2Int(
            UnityEngine.Random.Range(0, WorldSize),
            UnityEngine.Random.Range(0, WorldSize)
        );
        return Tiles[position];
    }

    ////returns tile at a given position in the map
    //public Tile getTileAt(int x, int y)
    //{
    //    Vector2Int pos = new Vector2Int(x, y);
    //    return Tiles[pos].;
    //
    //}

    public Vector2 MouseWorldPositionVector2()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    public Vector2Int ClampToWorldCoordinates(Vector2Int position)
    {
        int x = (int)Mathf.Clamp(position.x, 0, WorldSize - 1);
        int y = (int)Mathf.Clamp(position.y, 0, WorldSize - 1);

        return new Vector2Int(x, y);
    }

    public bool InBounds(Vector2Int position)
    {
        return
            position.x >= 0 &&
            position.x < WorldSize &&
            position.y >= 0 &&
            position.y < WorldSize;
    }
}
