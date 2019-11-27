using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RtsCameraController : MonoBehaviour
{
    // Speed of camera
    [SerializeField] private float _cameraSpeed = 10;

    // Height above ground
    private float height = 10;

    // Current Camera Velocity
    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        // Camera Movement on Axes
        HandleMovement();

        // Camera Rotation
        HandleRotation();

        // Camera Zoom
        HandleZoom();
    }

    private void HandleMovement()
    {
        velocity /= 1.35f;
        velocity.x += Input.GetAxis("Horizontal") * _cameraSpeed * Time.deltaTime;
        velocity.z += Input.GetAxis("Vertical") * _cameraSpeed * Time.deltaTime;
        GetComponent<Transform>().Translate(velocity, Space.Self);
    }

    private void HandleRotation()
    {
        if (!Input.GetMouseButton(2)) { return; }

        // Use delta X as rotational input
        float deltaX = Input.GetAxis("Mouse X") * 2f;

        // Find rotation pivot (1.55 is constant based on camera angle)
        // Since angle is constantly 40 degrees downward, and the height is the height of a triangle
        // Then 1.55 is the ratio of the hypotenuse where we intersect with the ground plane
        // Therefore we can rotate around with this as the pivot as per a nice RTS feeling
        Vector3 pivot = transform.position + (transform.forward * height * 1.55f);
        transform.RotateAround(pivot, Vector3.up, deltaX);
    }

    private void HandleZoom()
    {
        height = Mathf.Clamp(height + Input.mouseScrollDelta.y, 2, 15);
        Vector3 finalPosition = GetComponent<Transform>().position;
        finalPosition.y = height;
        transform.position = finalPosition;
    }
}
