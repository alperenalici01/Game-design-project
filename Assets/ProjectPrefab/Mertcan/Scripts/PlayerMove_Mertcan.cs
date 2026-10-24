using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMove_Mertcan : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;

    [SerializeField] private float acceleration = 40f;

    [SerializeField] private float turnSpeed = 720f;

    [SerializeField] private Transform cameraTransform;



    private Rigidbody rb;

    private InputAction moveAction;

    private Vector3 direction; // where the input points

    private float grip = 1f; // 1 = normal ground

    private Transform currentPlatform;
    private Vector3 lastPlatformPosition;
    private float lastPlatformContact = float.NegativeInfinity;

    void Start()

    {

        rb = GetComponent<Rigidbody>();

        moveAction = InputSystem.actions.FindAction("Move");

    }

    void Update()

    {

        Vector2 input = moveAction.ReadValue<Vector2>();

        if (cameraTransform == null)
        {
            direction = Vector3.zero;
            return;
        }

        // Kameranın ileri yönünü yatay zemine aktar.
        Vector3 forward = Vector3.ProjectOnPlane(
            cameraTransform.forward, Vector3.up
        );

        // Kamera tam aşağı bakıyorsa ekranın üst yönünü kullan.
        if (forward.sqrMagnitude < 0.001f)
        {
            forward = Vector3.ProjectOnPlane(
                cameraTransform.up, Vector3.up
            );
        }

        forward.Normalize();

        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

        direction = Vector3.ClampMagnitude(
            right * input.x + forward * input.y, 1f
        );
    }

    void FixedUpdate()

    {

        // speed we want, speed we have (not up/down)

        Vector3 platformVelocity = Vector3.zero;

        if (currentPlatform != null)
        {
            bool recentContact =
                Time.fixedTime - lastPlatformContact
                <= Time.fixedDeltaTime * 1.5f;

            if (recentContact)
            {
                platformVelocity =
                    (currentPlatform.position - lastPlatformPosition)
                    / Time.fixedDeltaTime;

                lastPlatformPosition = currentPlatform.position;

                // Zıplayarak platformdan ayrılıyorsak takip etme.
                if (rb.linearVelocity.y > platformVelocity.y + 0.5f)
                {
                    currentPlatform = null;
                    platformVelocity = Vector3.zero;
                }
            }
            else
            {
                currentPlatform = null;
            }
        }

        Vector3 wanted = direction * maxSpeed;

        wanted.x += platformVelocity.x;
        wanted.z += platformVelocity.z;

        Vector3 v = rb.linearVelocity;

        Vector3 flat = new Vector3(v.x, 0f, v.z);

        // get a little closer to the wanted speed

        float step = acceleration * grip * Time.fixedDeltaTime;

        flat = Vector3.MoveTowards(flat, wanted, step);

        rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);

        // turn towards the way we are going

        if (direction == Vector3.zero) return;

        Quaternion look = Quaternion.LookRotation(direction);

        float turn = turnSpeed * Time.fixedDeltaTime;

        look = Quaternion.RotateTowards(rb.rotation, look, turn);

        rb.MoveRotation(look);

    }

    // the icy ramp calls this

    public void SetGrip(float value)

    {

        grip = value;

    }
    void OnCollisionEnter(Collision collision)
{
    CheckPlatform(collision);
}

void OnCollisionStay(Collision collision)
{
    CheckPlatform(collision);
}

private void CheckPlatform(Collision collision)
{
    Patroller platform =
        collision.collider.GetComponentInParent<Patroller>();

    if (platform == null || !platform.isActiveAndEnabled)
        return;

    for (int i = 0; i < collision.contactCount; i++)
    {
        // Platformun yalnızca üst yüzeyine basmayı algıla.
        if (collision.GetContact(i).normal.y > 0.5f)
        {
            if (currentPlatform != platform.transform)
            {
                currentPlatform = platform.transform;
                lastPlatformPosition = currentPlatform.position;
            }

            lastPlatformContact = Time.fixedTime;
            return;
        }
    }
}

}

