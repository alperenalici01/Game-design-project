using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;

    [SerializeField] private float acceleration = 40f;

    [SerializeField] private float turnSpeed = 720f;



    private Rigidbody rb;

    private InputAction moveAction;

    private Vector3 direction; // where the input points

    private float grip = 1f; // 1 = normal ground

    void Start()

    {

        rb = GetComponent<Rigidbody>();

        moveAction = InputSystem.actions.FindAction("Move");

    }

    void Update()

    {

        Vector2 input = moveAction.ReadValue<Vector2>();

        direction = new Vector3(input.x, 0f, input.y);
    }

    void FixedUpdate()
    {
        Vector3 wanted = direction * maxSpeed;
        Vector3 v = rb.linearVelocity;
        Vector3 flat = new Vector3(v.x, 0f, v.z);

        // If there is input, allow normal acceleration. If there is no input (sliding on ice), use the low grip!
        float currentAcceleration = (direction.magnitude > 0.1f) ? acceleration : (acceleration * grip);
        
        float step = currentAcceleration * Time.fixedDeltaTime;
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

}

