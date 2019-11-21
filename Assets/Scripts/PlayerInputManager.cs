using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the player's input.
/// </summary>
public class PlayerInputManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Press R to restart
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Just reloads the current scene.
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
