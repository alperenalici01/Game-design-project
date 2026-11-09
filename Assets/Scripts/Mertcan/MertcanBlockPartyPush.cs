using UnityEngine;

// Ortak PlayerMove sonrasında sadece temas edilen itici bloğun hızını korur.
// Runs after the shared PlayerMove (default order 0), and only preserves a contacting BlockParty wall's push.
[DefaultExecutionOrder(100)]
[DisallowMultipleComponent]
public class MertcanBlockPartyPush : MonoBehaviour
{
    private Rigidbody playerBody;
    private MertcanBlockPartyPiece contactBlock;
    private Collider contactCollider;
    private float lastContact = float.NegativeInfinity;

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Observe(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        Observe(collision);
    }

    private void Observe(Collision collision)
    {
        MertcanBlockPartyPiece block = collision.collider.GetComponentInParent<MertcanBlockPartyPiece>();
        if (block == null || !block.IsPushable) return;
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.x >= -0.5f) continue;
            contactBlock = block;
            contactCollider = collision.collider;
            lastContact = Time.fixedTime;
            return;
        }
    }

    private void FixedUpdate()
    {
        if (playerBody == null || contactBlock == null) return;
        if (!contactBlock.IsPushable || !contactBlock.isActiveAndEnabled || contactCollider == null ||
            Time.fixedTime - lastContact > Time.fixedDeltaTime * 1.5f ||
            contactCollider.bounds.SqrDistance(playerBody.position) > 2.25f)
        {
            ClearContact();
            return;
        }
        Vector3 velocity = playerBody.linearVelocity;
        velocity.x = Mathf.Min(velocity.x, contactBlock.VelocityX);
        // Lateral steering, jump velocity and turning remain owned by the existing controllers.
        playerBody.linearVelocity = velocity;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider == contactCollider) ClearContact();
    }

    private void OnDisable()
    {
        ClearContact();
    }
    private void ClearContact()
    {
        contactBlock = null;
        contactCollider = null;
        lastContact = float.NegativeInfinity;
    }
}
