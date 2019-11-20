using System.Collections;
using System.Collections.Generic;

public class AStar
{
    public static List<Node<T>> ConstructPath<T>(Node<T> start, Node<T> goal)
    {
        if (start == goal)
        {
            return new List<Node<T>> { start };
        }
        // List of nodes that do not need to be (re-)expanded.
        List<Node<T>> closedList = new List<Node<T>>();

        // List of discovered nodes that need to be (re-)expanded
        List<Node<T>> openList = new List<Node<T>>()
        {
            start
        };

        // Form node n cameFrom[n] returns the node
        // immediately preceding it on the cheapest known path
        Dictionary<Node<T>, Node<T>> cameFrom
            = new Dictionary<Node<T>, Node<T>>();

        // For node n, gScore[n] returns the cost of the cheapest known path.
        Dictionary<Node<T>, float> gScore = new Dictionary<Node<T>, float>();
        gScore.Add(start, 0.0f);

        // For node n, fScore[n] = gScore[n] + heuristic cost(n)
        Dictionary<Node<T>, float> fScore = new Dictionary<Node<T>, float>();
        fScore.Add(start, 0.0f);

        while (openList.Count > 0)
        {
            // The node in openSet with the lowest fScore
            Node<T> current = openList[0];

            foreach (Node<T> node in openList)
            {
                if (fScore[node] < fScore[current])
                {
                    current = node;
                }
            }

            if (current == goal)
            {
                // Reconstruct path
                List<Node<T>> path = new List<Node<T>>();
                path.Add(current);

                while (cameFrom.ContainsKey(current))
                {
                    current = cameFrom[current];
                    path.Insert(0, current);
                }

                return path;
            }

            openList.Remove(current);
            closedList.Add(current);

            foreach (Edge<T> edge in current.Edges)
            {
                Node<T> neighbor = edge.Node;
                if (closedList.Contains(neighbor))
                {
                    continue;
                }

                float tentativeGScore = gScore[current] + edge.Cost;

                if (!gScore.ContainsKey(neighbor) ||
                    tentativeGScore < gScore[neighbor])
                {
                    cameFrom.Add(neighbor, current);
                    gScore.Add(neighbor, tentativeGScore);
                    fScore.Add(neighbor, gScore[neighbor] + 0);

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }
        // Something went wrong.
        return null;
    }
}
