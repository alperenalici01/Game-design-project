using UnityEngine;
using UnityEngine.InputSystem;

public class TpsCamera : MonoBehaviour
{
    [SerializeField] private Transform target;                 // the Player
    [SerializeField] private float distance = 6f;
    [SerializeField] private float lookHeight = 1.5f;          // the point we orbit around (above the feet)
    [SerializeField] private float sensitivity = 0.15f;        // degrees per mouse pixel
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 70f;
    [SerializeField] private float smooth = 15f;               // higher = follows faster
    [SerializeField] private float collisionRadius = 0.3f;
    [SerializeField] private LayerMask collisionMask = ~0;     // what blocks the camera
    [SerializeField] private bool lockCursor = true;

    private float yaw;
    private float pitch = 20f;
    private float currentDistance;

    // other scripts (movement) can use this to move relative to the camera
    public float Yaw => yaw;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("TpsCamera: drag the Player into Target.", this);
            enabled = false;
            return;
        }

        yaw = transform.eulerAngles.y;
        currentDistance = distance;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // LateUpdate: after the player has moved this frame
    void LateUpdate()
    {
        // Esc frees the cursor, click locks it again
        if (lockCursor && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        bool locked = Cursor.lockState == CursorLockMode.Locked;

        if (locked && Mouse.current != null)
        {
            Vector2 look = Mouse.current.delta.ReadValue() * sensitivity;
            yaw += look.x;
            pitch = Mathf.Clamp(pitch - look.y, minPitch, maxPitch);
        }
        else if (lockCursor && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        Vector3 pivot = target.position + Vector3.up * lookHeight;
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 back = rotation * Vector3.back;

        // pull the camera in when a wall is between it and the player
        float wanted = distance;
        if (Physics.SphereCast(pivot, collisionRadius, back, out RaycastHit hit, distance,
                               collisionMask, QueryTriggerInteraction.Ignore))
        {
            wanted = Mathf.Max(hit.distance, 0.5f);
        }
        // snap in fast, ease out slowly
        currentDistance = wanted < currentDistance
            ? wanted
            : Mathf.Lerp(currentDistance, wanted, smooth * Time.deltaTime);

        transform.SetPositionAndRotation(pivot + back * currentDistance, rotation);
    }
}
