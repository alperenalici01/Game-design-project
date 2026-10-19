using UnityEngine;

public class KillZone : MonoBehaviour
{
    // Detect: something enters this trigger (spikes, turtle, fall zone)
    void OnTriggerEnter(Collider other)
    {
        // Who? only the player
        if (other.GetComponent<PlayerMove>() == null) return;

        // Act: the level starts again
        LevelManager.Instance.Restart();
    }
}
