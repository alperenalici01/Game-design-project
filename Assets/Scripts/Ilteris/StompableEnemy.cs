using UnityEngine;

public class StompableEnemy : MonoBehaviour
{
    [SerializeField] private float stompHeight = 0.3f;  // feet must be this far above our base
    [SerializeField] private float bounceSpeed = 6f;    // how high the player bounces off

    // Detect: something enters our trigger
    void OnTriggerEnter(Collider other)
    {
        // Who? only the player
        if (other.GetComponent<PlayerMove>() == null) return;

        Rigidbody playerRb = other.attachedRigidbody;
        float playerFeet = other.bounds.min.y;          // bottom of the player's capsule
        bool falling = playerRb.linearVelocity.y < 0.1f;
        bool fromAbove = playerFeet > transform.position.y + stompHeight;

        if (falling && fromAbove)
        {
            // Act 1: stomped! bounce the player up and remove this enemy
            Vector3 v = playerRb.linearVelocity;
            playerRb.linearVelocity = new Vector3(v.x, bounceSpeed, v.z);
            gameObject.SetActive(false);
        }
        else
        {
            // Act 2: touched from the side: the level starts again
            LevelManager.Instance.Restart();
        }
    }
}
