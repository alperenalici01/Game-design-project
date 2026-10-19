using UnityEngine;

public class jJumppad : MonoBehaviour
{
    [SerializeField] private float jumpForce = 12f;

    private float nextLaunchAt; // cooldown, like canPickUpAt

    void OnTriggerStay(Collider other)
    {
        // Who? only the object tagged "Player"
        if (!other.CompareTag("Player")) return;
        if (Time.time < nextLaunchAt) return; // just launched

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) return;

        // Act: cancel the old movement, then push along the pad's own up
        rb.linearVelocity = Vector3.zero; // Unity 2022 and older: rb.velocity
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        nextLaunchAt = Time.time + 0.5f;
    }
}
