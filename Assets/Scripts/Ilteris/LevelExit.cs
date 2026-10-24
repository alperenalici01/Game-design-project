using UnityEngine;

public class LevelExit : MonoBehaviour
{
    // Detect: something walks into the door
    void OnTriggerEnter(Collider other)
    {
        // Who? only the player
        if (other.GetComponent<PlayerMove>() == null) return;

        // Act: victory
        LevelManager.Instance.Win();
    }
}
