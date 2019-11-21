using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles game time logic.
/// </summary>
public class GameTimer : MonoBehaviour
{
    /// <summary>
    /// The amount of time before the game ends in seconds.
    /// </summary>
    [Tooltip("The amount of time before the game ends in seconds. " +
             "If you wish to specify the time in minutes you can " +
             "type 60 * (minutes) where minutes is the amount of minutes.")]
    [SerializeField] private int _maxGameTime = 120;

    /// <summary>
    /// The text that displays how much time is left before the game ends.
    /// </summary>
    [SerializeField] private Text _countdownText = null;

    // Awake is called before start
    private void Awake()
    {
        // Check that the countdown text has been assigned
        if (_countdownText == null)
        {
            Debug.LogError(
                "GameTimer: Countdown Text is null. " +
                "Please drag to countdown text UI element to the " +
                name + " GameObject.",
                this
            );
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update the timer display
        int totalSecondsRemaining = _maxGameTime - (int)Time.time;
        int minutes = totalSecondsRemaining / 60;
        int seconds = totalSecondsRemaining - minutes * 60;
        _countdownText.text =
            ((minutes < 10) ? "0" : "") + minutes + ":" +
            ((seconds < 10) ? "0" : "") + seconds;
    }
}
