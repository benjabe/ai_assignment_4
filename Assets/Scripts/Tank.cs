using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A tank.
/// </summary>
public class Tank : MonoBehaviour
{
    /// <summary>
    /// A list of all the tanks in the game.
    /// </summary>
    /// <value></value>
    public static List<Tank> Tanks { get; protected set; } = new List<Tank>();

    /// <summary>
    /// The amount of units the tank can move in one second.
    /// </summary>
    [Tooltip("The amount of units the tank can move in one second.")]
    [SerializeField] private float _moveSpeed = 1.0f;

    /// <summary>
    /// The colour of the tank.
    /// </summary>
    [Tooltip("The colour of the tank. This is used for drawing the " +
             "the tank's path and has nothing to do with the colour " +
             "of its meshes.")]
    [SerializeField] private Color _color = new Color(1.0f, 0.0f, 1.0f);
    public Color Color { get => _color; }

    /// <summary>
    /// Layers containing objects which block line of sight.
    /// </summary>
    [Tooltip("Layers containing objects which block line of sight. " +
             "Basically should include any level elements.")]
    [SerializeField] private LayerMask _lineOfSightBlockingMask = 0;
    
    /// <summary>
    /// The tile on which the tank currently sits.
    /// </summary>
    public Tile CurrentTile { get; protected set; } = null;

    /// <summary>
    /// The current path to where the tank wants to be.
    /// </summary>
    public List<Node<Tile>> Path {get; protected set; }= null;

    /// <summary>
    /// The level in which the tank exists.
    /// </summary>
    private TileGraph _level = null;

    /// <summary>
    /// The tank this tank wants to kill.
    /// </summary>
    private Tank _target = null;

    private void Awake()
    {
        Tanks.Add(this);
    }

    // Start is called before the first frame update
    private void Start()
    {
        _level = TileGraph.Instance;
        CurrentTile = _level.NearestTile(transform.position);
        //Debug.Log(name + ": " + _currentTile.NodeTransform.position, this);
    }

    // Update is called once per frame
    void Update()
    {
        // Find a path if we don't have one.
        if (Path == null || Path.Count <= 0)
        {
            Path = _level.ConstructPath(
                CurrentTile,
                _level.Tiles[Random.Range(0, _level.Tiles.Count)]
            );
        }

        // If we have a path, move along it.
        if (Path != null && Path.Count > 0)
        {
            MoveAlongPath();
        }

        // Get a tank in line of sight
        _target = FindTankInLineOfSight();

        if (_target != null)
        {
            // Found a target, shoot it.
            Shoot();
        }
    }

    /// <summary>
    /// Moves the tank along the current path.
    /// </summary>
    private void MoveAlongPath()
    {
        // Take the first tile in the path list and
        // get the direction to it from the current position.
        Vector3 targetPosition = Path[0].Data.NodeTransform.position;
        Vector3 distance = targetPosition - transform.position;
        Vector3 direction = distance.normalized;
        Vector3 toMove = direction * Time.deltaTime * _moveSpeed;

        // Check if we are overshooting/moving further than we need
        if (toMove.sqrMagnitude >= distance.sqrMagnitude)
        {
            // Overshooting, only move the remaining distance
            transform.position += distance;
            // We also reached our destination so we should
            // remove it from the current path.
            CurrentTile = Path[0].Data;
            Path.RemoveAt(0);
        }
        else
        {
            // We're not overshooting, just move regularly
            transform.position += toMove;
        }
    }

    /// <summary>
    /// Finds a tank in line of sight.
    /// </summary>
    /// <returns>The first tank found in line of sight.</returns>
    private Tank FindTankInLineOfSight()
    {
        // Check all tanks that aren't this tank.
        // Do a raycast, if it doesn't get blocked, the tank is in LOS.
        foreach (Tank tank in Tanks)
        {
            if (tank != this)
            {
                // Check if there's anything blocking line of sight
                RaycastHit hit;
                Vector3 distance = tank.transform.position - transform.position;
                Ray lineOfSightRay = new Ray(transform.position, distance);
                Physics.Raycast(
                    lineOfSightRay,
                    out hit,
                    _lineOfSightBlockingMask
                );

                if (hit.collider == null)
                {
                    // Draw a ray for visualisation
                    Debug.DrawRay(
                        transform.position,
                        distance,
                        Color.green
                    );
                    // Nothing blocks line of sight, return the tank.
                    return tank;
                }
                else
                {
                    // The position of the collider
                    Vector3 hitPosition = hit.collider.transform.position;
                    // Something *did* block the line of sight
                    // Draw a ray for visualisation
                    // Draw a green line to the blocking collider
                    Debug.DrawRay(
                        transform.position,
                        hitPosition - transform.position,
                        Color.green
                    );
                    // Draw a red line the rest of the distance
                    Debug.DrawRay(
                        hitPosition,
                        tank.transform.position - hitPosition,
                        Color.red
                    );
                }
            }
        }

        // No tanks in line of sight.
        return null;
    }

    /// <summary>
    /// Shoots a projectile.
    /// </summary>
    private void Shoot()
    {
        Debug.Log(name + " shoots " + _target.name, this);
    }
}
