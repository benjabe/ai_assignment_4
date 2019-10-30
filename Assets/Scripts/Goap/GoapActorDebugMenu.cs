using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides debug information about a GoapActor.
/// </summary>
public class GoapActorDebugMenu : MonoBehaviour
{
    private GoapActor _goapActor = null;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectGoapActorUnderMouse();
            if (_goapActor != null)
            {
                Vector3 newPosition =
                    Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(
                    newPosition.x,
                    newPosition.y,
                    0.0f
                );
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
            else
            {
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    void SelectGoapActorUnderMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(
            Camera.main.ScreenToWorldPoint(Input.mousePosition),
            ray.direction,
            Mathf.Infinity
        );
        if (hit)
        {
            GoapActor actor = hit.collider.gameObject.GetComponent<GoapActor>();
            if (actor != null)
            {
                _goapActor = actor;
            }
            else
            {
                _goapActor = null;
            }
        }
    }
}
