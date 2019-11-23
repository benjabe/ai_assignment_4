﻿using System.Collections;
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
    [SerializeField] private Color _color = new Color(1.0f, 0.0f, 1.0f);
    public Color Color { get => _color; }
    
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
            /*Debug.Log(name + ": New path:", this);
            foreach (Node<Tile> node in Path)
            {
                Debug.Log(node.Data.NodeTransform.position, this);
            }*/
        }

        // If we have a path, move along it.
        if (Path != null && Path.Count > 0)
        {
            faceMovingDirection();
            MoveAlongPath();
        }

        aimAtOpponent();
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
    private void faceMovingDirection()
    {
        // Gets the tanks relative position through using the target and its own position
        Vector3 targetPosition = Path[0].Data.NodeTransform.position;
        Vector3 relativePosition = targetPosition - transform.position;

        // Get tank body
        GameObject body = this.transform.GetChild(0).gameObject;

        // Sets the tanks rotation relative to the direction on the target
        Quaternion rotation = Quaternion.LookRotation(relativePosition, Vector3.up);
        transform.rotation = rotation;
    }

    /// <summary>
    /// Rotates the tank turret and barrel towards its opponent
    /// </summary>
    private void aimAtOpponent()
    {
        // Get turrent and barrel
        // TODO - Bytte ut hardkodede index-verdier med noe som GetChild("turret") e.l.?
        GameObject turret = this.transform.GetChild(1).gameObject;
        GameObject barrel = this.transform.GetChild(2).gameObject;

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
}
