using UnityEngine;

public class WalkBob : MonoBehaviour
{
    [Header("Walking")]
    [SerializeField] private float stepsPerSecond = 4f;   // how fast the feet "step"
    [SerializeField] private float bounceHeight = 0.12f;  // little hop on every step
    [SerializeField] private float swayAngle = 6f;        // side-to-side waddle, degrees
    [SerializeField] private float speedForFullBob = 5f;  // at this speed the bob is full size

    [Header("Jumping")]
    [SerializeField] private LayerMask groundLayers;      // set it to Ground (same as PlayerJump)
    [SerializeField] private float airStretch = 0.15f;    // taller and thinner in the air
    [SerializeField] private float landSquash = 0.25f;    // flatter and wider on landing
    [SerializeField] private float shapeSpeed = 12f;      // how fast the shape changes

    private Rigidbody rb;                // the Player's Rigidbody (on our parent)
    private Vector3 startLocalPosition;
    private Quaternion startLocalRotation;
    private Vector3 startLocalScale;
    private float stepTime;
    private bool wasGrounded = true;
    private float stretch;               // >0 = tall and thin, <0 = flat and wide

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        startLocalPosition = transform.localPosition;
        startLocalRotation = transform.localRotation;
        startLocalScale = transform.localScale;
    }

    void Update()
    {
        // same ground check as PlayerJump: 1.1 m down from the Player's middle
        bool grounded = Physics.Raycast(rb.position, Vector3.down, 1.1f, groundLayers);

        // just landed: a quick squash
        if (grounded && !wasGrounded) stretch = -landSquash;
        wasGrounded = grounded;

        // in the air: stretch; on the ground: slowly back to normal
        float wantedStretch = grounded ? 0f : airStretch;
        stretch = Mathf.Lerp(stretch, wantedStretch, shapeSpeed * Time.deltaTime);

        // ---- walking bob, only on the ground ----
        Vector3 v = rb.linearVelocity;
        float speed = new Vector3(v.x, 0f, v.z).magnitude;
        float amount = grounded ? Mathf.Clamp01(speed / speedForFullBob) : 0f;

        stepTime += Time.deltaTime * stepsPerSecond * amount;
        float hop = Mathf.Abs(Mathf.Sin(stepTime * Mathf.PI)) * bounceHeight * amount;
        float sway = Mathf.Sin(stepTime * Mathf.PI) * swayAngle * amount;

        // ---- apply ----
        transform.localPosition = startLocalPosition + Vector3.up * hop;
        transform.localRotation = startLocalRotation * Quaternion.Euler(0f, 0f, sway);
        // taller means thinner, flatter means wider: the body keeps its "volume"
        transform.localScale = new Vector3(
            startLocalScale.x * (1f - stretch * 0.5f),
            startLocalScale.y * (1f + stretch),
            startLocalScale.z * (1f - stretch * 0.5f));
    }
}
