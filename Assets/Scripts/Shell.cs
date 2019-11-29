using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A bombshell, more often known as a shell.
/// </summary>
public class Shell : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.up * Time.deltaTime);
    }
}
