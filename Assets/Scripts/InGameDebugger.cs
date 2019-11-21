using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Displays in-game debug information.
/// </summary>
public class InGameDebugger : MonoBehaviour
{
    /// <summary>
    /// Prefab for game objects that draw lines between nodes.
    /// </summary>
    [SerializeField] private GameObject _edgeLineRenderer = null;

    /// <summary>
    /// The GameObject with the Camera which renders the lines.
    /// </summary>
    [SerializeField] private GameObject _lineRendererCameraGO = null;

    /// <summary>
    /// Whether the InGameDebugger is currently displaying debug info.
    /// </summary>
    /// <value></value>
    public bool IsActive { get; protected set; } = false;


    private void Start()
    {
        // Set up some line renderers 
        List<Vector3> positions = new List<Vector3>();

        // Loop through each node in the level
        foreach (Node<Tile> node in TileGraph.Instance.Nodes.Values)
        {
            // Loop through the node's neighbours
            foreach (Edge<Tile> edge in node.Edges)
            {
                // Draw line from the node to its neighbour
                GameObject go = Instantiate(_edgeLineRenderer, transform);
                LineRenderer lineRenderer = go.GetComponent<LineRenderer>();
                lineRenderer.positionCount = 2;
                lineRenderer.SetPositions(new Vector3[] {
                    node.Data.NodeTransform.position,
                    edge.Node.Data.NodeTransform.position
                });
            }
        }

        SetActive(IsActive);
    }

    /// <summary>
    /// Enables or disables the InGameDebugger.
    /// </summary>
    /// <param name="active">
    /// True enables the debugger.
    /// False disables the debugger.
    /// </param>
    public void SetActive(bool active)
    {
        IsActive = active;

        if (IsActive)
        {
            // Display lines
            _lineRendererCameraGO.SetActive(true);
        }
        else
        {
            // Hide lines
            _lineRendererCameraGO.SetActive(false);
        }
    }
}
