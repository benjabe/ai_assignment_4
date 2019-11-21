using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCamera : MonoBehaviour
{
    /// <summary>
    /// A reference to the main camera (using Camera.main is expensive).
    /// </summary>
    Camera _mainCamera = null;

    // Start is called before the first frame update
    void Awake()
    {
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTransform();
    }

    /// <summary>
    /// Updates transform values to those of the main camera.
    /// </summary>
    private void UpdateTransform()
    {
        // Set position and rotation to the same as the main camera
        transform.position = _mainCamera.transform.position;
        transform.eulerAngles = _mainCamera.transform.eulerAngles;
    }

    /// <summary>
    /// Called when the game object is enables.
    /// </summary>
    private void OnEnable()
    {
        UpdateTransform();
    }
}
