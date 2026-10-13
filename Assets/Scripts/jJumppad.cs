using UnityEngine;

public class jJumppad : MonoBehaviour
{
    [SerializeField] private float jumpForce = 12f;

    void OnTriggerEnter(Collider other)
    {
        // Who? only the object tagged "Player"
        if (!other.CompareTag("Player")) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) return;

        // Act: push the courier up
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
