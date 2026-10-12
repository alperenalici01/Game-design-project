using UnityEngine;

public class Cargo : MonoBehaviour
{
    private Rigidbody rb;
    private Collider box; // solid body
    private Collider range; // pickup trigger
    private float canPickUpAt; // re-pickup time

    void Awake()
    {
    // find our own components once, keep them
        rb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();
        range = GetComponent<SphereCollider>();
    }
    // a question: "can I be picked up now?"
    public bool CanBePickedUp()
    {
        return Time.time >= canPickUpAt;
    }

    public void PickUp(Transform carryPoint)
    {
        rb.isKinematic = true; // physics stops
        box.enabled = false; // no bumping
        range.enabled = false; // no re-pickup
        // become a child of the carry point
        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
    public void Drop()
    {
        transform.SetParent(null); // free again
        box.enabled = true;
        range.enabled = true;
        rb.isKinematic = false; // physics again
        // push it back and up, away from the courier
        Vector3 push = -transform.forward + Vector3.up;
        rb.AddForce(push * 3f, ForceMode.Impulse);
        // wait 0.5 s before it can be picked up again
        canPickUpAt = Time.time + 0.5f;
        GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
    }
}
