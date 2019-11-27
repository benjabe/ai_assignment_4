using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magazine : MonoBehaviour
{
    // Max number of shells in magazine
    [SerializeField] private int _capacity = 20;

    // Time in seconds to reload magazine
    [SerializeField] private float _reloadTime = 2f;

    // True when tank is reloading
    private bool _isReloading = false;

    // Current Number of shells left in current magazine
    private int _shellsLeft = 20;

    // Start is called before the first frame update
    void Start()
    {
        _shellsLeft = 0;
    }

    // Returns true if the magazine has bullets
    bool HasBullets()
    {
        return _shellsLeft > 0;
    }

    // Returns true if tank is reloading
    public bool IsReloading()
    {
        return _isReloading;
    }

    // For a magazine, triggering shoot simply means reducing number of shells, returning true if it
    // successfully fired.
    public bool Shoot()
    {
        if(!HasBullets())
        {
            return false;
        }

        _shellsLeft--;
        return true;
    }

    // Reload the magazine is a coroutine function that sets reloading and fills up the magazine while waiting for reload time
    public IEnumerator Reload()
    {
        Debug.Log("Magazine is reloading..");

        // Set up reload state
        _isReloading = true;

        // Iterate until we are done
        for(float timeLeft = _reloadTime; timeLeft > 0; timeLeft -= 0.05f)
        {
            yield return new WaitForSeconds(0.05f);
        }

        // Reset and clean up state
        _shellsLeft = _capacity;
        _isReloading = false;

        Debug.Log("Reload completed.");
        yield break;
    }
}
