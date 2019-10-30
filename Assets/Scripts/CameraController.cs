using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 13.5f;
    [SerializeField] private float _zoomSpeed = 2.0f;
    [SerializeField] private float _minimumOrthographicSize = 3.0f;
    [SerializeField] private float _maximumOrthographicSize = 25.0f;
    
    //[SerializeField] private bool edgeScrolling = false;

    private Camera mainCamera;

    Vector2 previousMousePosition = Vector2.zero;

    void Awake()
    {
        mainCamera = GetComponent<Camera>();
        previousMousePosition = Input.mousePosition;
    }

    void Update()
    {
        // wasd movement
        Vector2 direction = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        // middle click panning
        Vector2 mousePosition =
            mainCamera.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(2))
        {
            Vector2 mouseDelta = mousePosition - previousMousePosition;
            transform.Translate(-mouseDelta);
        }
        previousMousePosition =
            mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // zooming
        float zoomDelta = -Input.GetAxisRaw("Mouse ScrollWheel") * _zoomSpeed;
        zoomDelta *= mainCamera.orthographicSize - _minimumOrthographicSize + 1;
        mainCamera.orthographicSize += zoomDelta;
        mainCamera.orthographicSize = Mathf.Clamp(
            mainCamera.orthographicSize,
            _minimumOrthographicSize,
            _maximumOrthographicSize
        );
    }

    void OnTick()
    {
        transform.Translate(Vector2.up);
    }
}
