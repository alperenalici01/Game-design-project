using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class jJumppad : MonoBehaviour
{
    [FormerlySerializedAs("jumpForce")]
    [SerializeField] private float launchSpeed = 12f; // meters per second, not force
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private bool usePadUp = true;
    [SerializeField] private bool preserveHorizontalVelocity = true;

    private readonly HashSet<Collider> contacts = new HashSet<Collider>();
    private float nextLaunchAt;

    // Works when the pad is a normal solid collider.
    private void OnCollisionEnter(Collision collision)
    {
        TryLaunch(collision.collider);
    }

    // Works when the pad collider has "Is Trigger" enabled.
    private void OnTriggerEnter(Collider other)
    {
        contacts.Add(other);
        TryLaunch(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // Handles the case where the player starts already inside the trigger.
        if (contacts.Add(other))
        {
            TryLaunch(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        contacts.Remove(other);
    }

    private void TryLaunch(Collider other)
    {
        if (other == null) return;
        if (Time.time < nextLaunchAt) return;

        // Finds the Rigidbody even when the collider is a child object.
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;

        if (!IsPlayer(other, rb)) return;

        Vector3 up = (usePadUp ? transform.up : Vector3.up).normalized;
        Vector3 currentVelocity = GetVelocity(rb);

        Vector3 horizontalVelocity = preserveHorizontalVelocity
            ? Vector3.ProjectOnPlane(currentVelocity, up)
            : Vector3.zero;

        SetVelocity(rb, horizontalVelocity + up * launchSpeed);

        nextLaunchAt = Time.time + cooldown;
    }

    private static bool IsPlayer(Collider other, Rigidbody rb)
    {
        if (other.CompareTag("Player") ||
            rb.CompareTag("Player") ||
            rb.transform.root.CompareTag("Player"))
        {
            return true;
        }

        // Also works if the Player object is still tagged "Untagged".
        return other.GetComponentInParent<PlayerMove>() != null
            || other.GetComponentInParent<PlayerJump>() != null;
    }

    private static Vector3 GetVelocity(Rigidbody rb)
    {
#if UNITY_6000_0_OR_NEWER
        return rb.linearVelocity;
#else
        return rb.velocity;
#endif
    }

    private static void SetVelocity(Rigidbody rb, Vector3 velocity)
    {
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = velocity;
#else
        rb.velocity = velocity;
#endif
    }
}
