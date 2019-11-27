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

    public GameObject bulletPrefab;
    float shootCooldown;

    /// <summary>
    /// The tanks turret and barrel
    /// </summary>
    GameObject turret;
    GameObject barrel;


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
             "Basically should include any level elements. " +
             "including tanks of course.")]
    [SerializeField] private LayerMask _lineOfSightBlockingMask = 0;

    /// <summary>
    /// The transform on which line of sight calculations are based.
    /// </summary>
    [SerializeField] private Transform _lineOfSightTransform = null;
    
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
        turret = this.transform.GetChild(1).gameObject;
        barrel = this.transform.GetChild(2).gameObject;

        shootCooldown = 0;
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
            FaceMovingDirection();
            MoveAlongPath();
        }

        // Get a tank in line of sight
        _target = FindTankInLineOfSight();

        if (_target != null)
        {
            // Found a target, shoot it.
            aimAtOpponent();
            Shoot();
        }
    }

    /// <summary>
    /// Called when the gameobject is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        Tanks.Remove(this);
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
    /// Rotates the tank in the direction it is facing
    /// </summary>
    private void FaceMovingDirection()
    {
        // Gets the tanks relative position through using the target and its own position
        Vector3 targetPosition = Path[0].Data.NodeTransform.position;
        Vector3 relativePosition = targetPosition - transform.position;

        // Get tank body
        GameObject body = this.transform.GetChild(0).gameObject;

        // Sets the tanks rotation relative to the direction on the target by lerping
        Quaternion targetRotation = Quaternion.LookRotation(relativePosition, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.04f);
    }

    /// <summary>
    /// Rotates the tank turret and barrel towards its opponent
    /// </summary>
    private void AimAtOpponent()
    {
        // Get opponent tank
        Tank opponent = (Tanks[0] == this) ? Tanks[1] : Tanks[0];

        // Get this tanks position relative to opponent
        Vector3 targetPosition = opponent.transform.position;
        Vector3 relativePosition = targetPosition - transform.position;
        
        // Sets the turret/barrel rotation relative to the direction on the opponent
        Quaternion rotation = Quaternion.LookRotation(relativePosition, Vector3.up);
    
        turret.transform.rotation = rotation;
        barrel.transform.rotation = rotation;
    }

    private void TurnTurret()
    {
        Quaternion turretRotation = turret.transform.rotation;
        Quaternion barrelRotation = barrel.transform.rotation;
        turretRotation.y += 1;
        barrelRotation.y += 1;

        turret.transform.rotation = turretRotation;
        barrel.transform.rotation = barrelRotation;
    }

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

                Vector3 distance =
                    tank._lineOfSightTransform.position -
                    _lineOfSightTransform.position;

                Ray lineOfSightRay = new Ray(
                    _lineOfSightTransform.position,
                    distance
                );

                Physics.Raycast(
                    lineOfSightRay,
                    out hit,
                    _lineOfSightBlockingMask
                );

                if (hit.collider != null)
                {
                    // We hit something
                    if (hit.collider.gameObject.GetComponent<Tank>() == tank)
                    {
                        // We hit the tank
                        // Draw a ray for visualisation
                        Debug.DrawRay(
                            _lineOfSightTransform.position,
                            distance,
                            Color.green
                        );
                        return tank;
                    }
                    else
                    {
                        // We hit something else

                        // The position of the collider
                        Vector3 hitPosition = hit.collider.transform.position;

                        // Draw a ray for visualisation
                        // Draw a green line to the blocking collider
                        Debug.DrawRay(
                            _lineOfSightTransform.position,
                            hitPosition - _lineOfSightTransform.position,
                            Color.green
                        );
                        // Draw a red line the rest of the distance
                        Debug.DrawRay(
                            hitPosition,
                            tank._lineOfSightTransform.position - hitPosition,
                            Color.red
                        );
                    }
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
        var turret = GetComponentInChildren<Turret>();
        if(turret.Shoot())
        {
            Debug.Log(name + " shoots " + _target.name, this);
        }
    }
}
