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
    /// The default colour of rendered lines.
    /// </summary>
    private Color _defaultLineColor = new Color(1.0f, 0.0f, 1.0f, 1.0f);

    /// <summary>
    /// Key: The edge that the line renderer belongs to
    /// Value: The line renderer itself
    /// </summary>
    private Dictionary<Edge<Tile>, LineRenderer> _lineRenderers = null;

    /// <summary>
    /// Whether the InGameDebugger is currently displaying debug info.
    /// </summary>
    /// <value></value>
    public bool IsActive { get; protected set; } = false;

    private void Awake()
    {
        _lineRenderers = new Dictionary<Edge<Tile>, LineRenderer>();
    }

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
                // Set the colour
                //Material whiteDiffuseMat
                    //= new Material(Shader.Find("Unlit/Texture"));
                //lineRenderer.material = whiteDiffuseMat;
                lineRenderer.startColor = _defaultLineColor;
                lineRenderer.endColor = _defaultLineColor;

                // Add the line renderer to the dictionary
                _lineRenderers[edge] = lineRenderer;
            }
        }

        SetActive(IsActive);
    }

    // Update is called every frame
    private void Update()
    {
        if (IsActive)
        {
            ResetLineColors();
            UpdateTankPathLines();
        }
    }

    /// <summary>
    /// Sets all line renderers to the default colour.
    /// </summary>
    private void ResetLineColors()
    {
        foreach(LineRenderer lineRenderer in _lineRenderers.Values)
        {
            lineRenderer.startColor = _defaultLineColor;
            lineRenderer.endColor = _defaultLineColor;
        }
    }

    /// <summary>
    /// Updates linerenderer for every tank's current path.
    /// </summary>
    private void UpdateTankPathLines()
    {
        // Go through each tank and figure out which
        // edges it's path goes through so we can
        // change their colour
        foreach (Tank tank in Tank.Tanks)
        {
            if (tank.Path != null)
            {
                // For each each node in the tank's path
                foreach (Node<Tile> node in tank.Path)
                {
                    // Go through its edges
                    foreach (Edge<Tile> edge in node.Edges)
                    {
                        // If the edge's node is also in the path
                        if (tank.Path.Contains(edge.Node))
                        {
                            // Recolour the line to the tank's colour
                            _lineRenderers[edge].startColor = tank.Color;
                            _lineRenderers[edge].endColor = tank.Color;
                        }
                    }
                }
            }
        }
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
