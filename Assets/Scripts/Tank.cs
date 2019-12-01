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
    /// The tanks turret and barrel
    /// </summary>
    public GameObject TurretPart;
    public GameObject BodyPart;

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
    public List<Node<Tile>> Path = null;

    /// <summary>
    /// The current plan of what to do.
    /// </summary>
    private List<GoapAction> _plan = null;

    /// <summary>
    /// The level in which the tank exists.
    /// </summary>
    public TileGraph Level = null;

    /// <summary>
    /// The tank this tank wants to kill.
    /// </summary>
    [HideInInspector] public Tank Target = null;

    /// <summary>
    /// Dictionary of the current conditions of the tank.
    /// </summary>
    public Dictionary<string, object> Conditions;

    /// <summary>
    /// An array of all the actions this agent can perform.
    /// </summary>
    private GoapAction[] _actions;


    private void Awake()
    {
        Tanks.Add(this);

        // Set up our initial conditions
        Conditions = new Dictionary<string, object>()
        {
            { "hasAmmo", true },
            { "hasTarget", false }
        };

        _actions = GetComponents<GoapAction>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        Level = TileGraph.Instance;
        CurrentTile = Level.NearestTile(transform.position);
        //Debug.Log(name + ": " + _currentTile.NodeTransform.position, this);
    }

    // Update is called once per frame
    void Update()
    {
        // Generic "roam and do nothing behaviour"
        // Replacing this with Goap Planning
        // Keeping for historical reasons
        // Find a path if we don't have one.
        /*
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
            AimAtOpponent();
            Shoot();
        }
        */

        // Do Goap Planning
        if (_plan == null)
        {
            // 1. Choose a goal
            // Default is to kill enemy
            var goal = new Dictionary<string, object>()
            {
                { "targetIsDead", true }
            };
            // If health is low the goal should be to survive

            // 2. Get a plan for how to execute the goal
            // If we get null this means no plan was possible
            // to achieve the goal or the planner failed
            Debug.Log(name + " - New plan:");
            _plan = GoapPlanner.CreatePlan(Conditions, goal, _actions);
            
            if (_plan != null)
            {
                foreach (var action in _plan)
                {
                    Debug.Log("\t" + action.GetType().Name);
                }
            }
        }
        // Execute plan
        if (_plan != null)
        {
            bool success = _plan[0].Perform(gameObject);
            // If executing the plan fails we scrap the plan.
            if (!success)
            {
                Debug.Log(name + " failed to execute " +
                    _plan[0].GetType().Name);
                _plan = null;
            }
            else if (_plan[0].IsDone())
            {
                Debug.Log(name + " executed " + _plan[0].GetType().Name);
                _plan[0].DoReset();
                _plan.RemoveAt(0);
                if (_plan.Count == 0)
                {
                    _plan = null;
                }
            }
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
    public void MoveAlongPath()
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

        // Sets the tanks rotation relative to the direction on the target by lerping
        Quaternion targetRotation = Quaternion.LookRotation(relativePosition, Vector3.up);
        BodyPart.transform.rotation = Quaternion.Lerp(BodyPart.transform.rotation, targetRotation, 0.04f);
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
    
        TurretPart.transform.rotation = Quaternion.Lerp(TurretPart.transform.rotation, rotation, 0.01f);
    }

    /// Finds a tank in line of sight.
    /// </summary>
    /// <returns>The first tank found in line of sight.</returns>
    public Tank FindTankInLineOfSight()
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
                        /*
                        Debug.DrawRay(
                            _lineOfSightTransform.position,
                            distance,
                            Color.green
                        );
                        */
                        return tank;
                    }
                    /*
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
                    */
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
        if(turret.Shoot(Target.gameObject))
        {
            Debug.Log(name + " shoots " + Target.name, this);
        }
    }
}
