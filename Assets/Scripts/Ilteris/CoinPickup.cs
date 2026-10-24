using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    // Detect: something enters the coin's trigger
    void OnTriggerEnter(Collider other)
    {
        // Who? only the player has PlayerMove
        if (other.GetComponent<PlayerMove>() == null) return;

        // Act: count it, then the coin disappears
        LevelManager.Instance.AddCoin();
        gameObject.SetActive(false);
    }
}
