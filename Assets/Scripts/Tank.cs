using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A tank.
/// </summary>
public class Tank : MonoBehaviour
{
    /// <summary>
    /// The amount of units the tank can move in one second.
    /// </summary>
    [Tooltip("The amount of units the tank can move in one second.")]
    [SerializeField] private float _moveSpeed = 1.0f;
    /// <summary>
    /// The tile on which the tank currently sits.
    /// </summary>
    private Tile _currentTile = null;

    /// <summary>
    /// The current path to where the tank wants to be.
    /// </summary>
    private List<Node<Tile>> _path = null;

    /// <summary>
    /// The level in which the tank exists.
    /// </summary>
    private TileGraph _level = null;

    // Start is called before the first frame update
    private void Start()
    {
        _level = TileGraph.Instance;
        _currentTile = _level.NearestTile(transform.position);
        Debug.Log(name + ": " + _currentTile.NodeTransform.position, this);
    }

    // Update is called once per frame
    void Update()
    {
        // Find a path if we don't have one.
        if (_path == null || _path.Count <= 0)
        {
            _path = _level.ConstructPath(
                _currentTile,
                _level.Tiles[Random.Range(0, _level.Tiles.Count)]
            );
            Debug.Log(name + ": New path:", this);
            foreach (Node<Tile> node in _path)
            {
                Debug.Log(node.Data.NodeTransform.position, this);
            }
        }

        // If we have a path, move along it.
        if (_path != null && _path.Count > 0)
        {
            MoveAlongPath();
        }
    }

    /// <summary>
    /// Moves the tank along the current path.
    /// </summary>
    private void MoveAlongPath()
    {
        // Take the first tile in the path list and
        // get the direction to it from the current position.
        Vector3 targetPosition = _path[0].Data.NodeTransform.position;
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
            _currentTile = _path[0].Data;
            _path.RemoveAt(0);
        }
        else
        {
            // We're not overshooting, just move regularly
            transform.position += toMove;
        }
    }
}
