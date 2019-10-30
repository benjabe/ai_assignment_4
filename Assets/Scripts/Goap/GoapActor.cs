using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A smart guy who uses GOAP (hopefully)
/// For the purpose of this project read 'GoapActor' as 'Tank'
/// </summary>
public class GoapActor : MonoBehaviour
{
    // All the GoapActors
    private static List<GoapActor> _goapActors = new List<GoapActor>();

    // The speed of the GoapActor
    [SerializeField] private float _speed = 2.0f;

    // The hit points of the GoapActor
    [SerializeField] private int _hitPoints = 5;

    // The current state of the GoapActor
    private State _state = State.Idle;

    // The GoapActions to perform to get to our desired world state
    private List<GoapAction> _actionPlan = null;

    // The current tile of the GoapActor
    private Tile _currentTile = null;

    // The path the GoapActor wants to move along
    private List<Tile> _path = null;

    // Called after Awake
    void Start()
    {
        while (_currentTile == null || !_currentTile.TileType.walkable)
        {
            _currentTile = World.Instance.GetRandomTile();
        }
        transform.position = _currentTile.transform.position;
        _goapActors.Add(this);
    }
    
    // Called every frame
    private void Update()
    {
        switch(_state)
        {
            case State.Idle:
            {
                // Just chilling

                // Chilled enough, do something
                _path = World.Instance.TileGraph.ConstructPath(
                    _currentTile,
                    World.Instance.GetRandomTile()
                );
                _state = State.Moving;
                break;
            }
            case State.Moving:
            {
                // Move towards a target position to perform our next action
                if (_path != null)
                {
                    MoveAlongPath();
                    if (_path.Count == 0)
                    {
                        _state = State.Idle;
                    }
                }
                else
                {
                    _state = State.Idle;
                }
                break;
            }
            case State.Acting:
            {
                // Do something
                break;
            }
        }
    }

    private void MoveAlongPath()
    {
        Vector3 target = _path[0].transform.position;
        Vector3 distanceToTarget = target - transform.position;
        Vector3 direction = distanceToTarget.normalized;
        Vector3 distanceToMove =
            direction * _speed * Time.deltaTime / _path[0].TileType.moveCost;
        if (distanceToMove.magnitude >= distanceToTarget.magnitude)
        {
            transform.position = target;
            _currentTile = _path[0];
            _path.RemoveAt(0);
        }
        else
        {
            transform.position += distanceToMove;
        }
    }

    // The possible states of the GoapActor
    private enum State
    {
        Idle,       // The actor is considering its goal
        Moving,     // The actor is moving to perform its next action
        Acting      // The actor is performing an action
    }

    // The team of the GoapActor
    private enum team
    {
        Blue,
        Red
    }
}
