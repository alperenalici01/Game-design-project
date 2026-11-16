using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class jJumppad : MonoBehaviour
{
    [FormerlySerializedAs("jumpForce")]
    [SerializeField] private float launchSpeed = 12f; 
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private bool usePadUp = true;
    [SerializeField] private bool preserveHorizontalVelocity = true;

    [SerializeField] private float normalMass = 1f;          
    [SerializeField] private float reductionPerMass = 0.2f;  
    [SerializeField] private float minMultiplier = 0.6f;      

    private readonly HashSet<Collider> contacts = new HashSet<Collider>();
    private float nextLaunchAt;

    
    private void OnCollisionEnter(Collision collision)
    {
        TryLaunch(collision.collider);
    }

    
    private void OnTriggerEnter(Collider other)
    {
        contacts.Add(other);
        TryLaunch(other);
    }

    private void OnTriggerStay(Collider other)
    {
        
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

        
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null || rb.isKinematic) return;

        if (!IsPlayer(other, rb)) return;

        Vector3 up = (usePadUp ? transform.up : Vector3.up).normalized;
        Vector3 currentVelocity = GetVelocity(rb);

        Vector3 horizontalVelocity = preserveHorizontalVelocity
            ? Vector3.ProjectOnPlane(currentVelocity, up)
            : Vector3.zero;

        
        float effectiveMass = rb.mass;
        JP_Special_Carry special = rb.GetComponent<JP_Special_Carry>();
        if (special != null)
        {
            float stacked = special.StackedMass(normalMass);
            if (stacked >= 0f) effectiveMass = stacked; 
        }

        float extraMass = Mathf.Max(0f, effectiveMass - normalMass);
        float multiplier = Mathf.Clamp(1f - extraMass * reductionPerMass, minMultiplier, 1f);

        Debug.Log("player mass: " + effectiveMass + " | çarpan: " + multiplier);

        rb.linearVelocity = horizontalVelocity + up * launchSpeed * multiplier;

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