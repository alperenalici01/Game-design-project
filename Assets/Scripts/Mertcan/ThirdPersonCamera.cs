using UnityEngine;
using UnityEngine.InputSystem;

// Run after Mertcan platform/respawn support (-100), using the final player position.
[DefaultExecutionOrder(200)]
[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField, Min(0.1f)] private float distance = 6f;
    [SerializeField] private float lookHeight = 1f;
    [SerializeField] private float initialYaw = 270f;
    [SerializeField] private float initialPitch = 20f;
    [SerializeField] private float minPitch = 10f;
    [SerializeField] private float maxPitch = 60f;
    [SerializeField, Min(0f)] private float sensitivity = 0.08f;
    [SerializeField, Min(0.01f)] private float collisionRadius = 0.35f;
    [SerializeField, Min(0f)] private float collisionPadding = 0.05f;
    [SerializeField] private LayerMask collisionMask = ~0;
    [SerializeField, Min(0.01f)] private float returnSpeed = 8f;

    private Rigidbody targetBody;
    private Camera view;
    private float yaw;
    private float pitch;
    private float currentDistance;
    private bool orbiting;
    private bool waitForRelease;

    private void OnEnable()
    {
        view = GetComponent<Camera>();
        targetBody = target != null ? target.GetComponent<Rigidbody>() : null;
        yaw = initialYaw;
        pitch = Mathf.Clamp(initialPitch, minPitch, maxPitch);
        currentDistance = distance;
        waitForRelease = true;
    }

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("ThirdPersonCamera: Target must be the Mario physics root.", this);
            enabled = false;
            return;
        }
        UpdateCamera();
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (!Application.isFocused || mouse == null)
        {
            ReleaseMouse();
            waitForRelease = true;
            return;
        }
        // A held button on focus return must not capture the cursor unexpectedly.
        if (waitForRelease)
        {
            if (!mouse.rightButton.isPressed) waitForRelease = false;
            return;
        }
        if (!mouse.rightButton.isPressed)
        {
            ReleaseMouse();
            return;
        }
        if (!orbiting)
        {
            if (!mouse.rightButton.wasPressedThisFrame) return;
            orbiting = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return; // Discard any cursor-warp delta on the capture frame.
        }

        // Mouse delta is already a per-frame displacement: do not multiply by deltaTime.
        Vector2 delta = mouse.delta.ReadValue();
        yaw = Mathf.Repeat(yaw + delta.x * sensitivity, 360f);
        pitch = Mathf.Clamp(pitch - delta.y * sensitivity, minPitch, maxPitch);
        // PlayerMove uses this camera's yaw; no rotation damping or automatic recentering.
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void LateUpdate()
    {
        if (target != null) UpdateCamera();
    }

    private bool IsObstacle(Collider candidate)
    {
        return candidate != null && !candidate.isTrigger &&
            !candidate.transform.IsChildOf(target) &&
            (targetBody == null || candidate.attachedRigidbody != targetBody);
    }

    public void SnapToRespawn(float heading, float downwardPitch)
    {
        yaw = heading;
        pitch = Mathf.Clamp(downwardPitch, minPitch, maxPitch);
        currentDistance = distance;
        if (target != null && view != null) UpdateCamera(true);
    }

    private void UpdateCamera(bool snap = false)
    {
        Vector3 pivot = target.position + Vector3.up * lookHeight;
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 back = rotation * Vector3.back;

        // Protect the near-plane corners as well as the camera's center.
        float halfHeight = view.nearClipPlane * Mathf.Tan(view.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float radius = Mathf.Max(collisionRadius, Mathf.Sqrt(
            view.nearClipPlane * view.nearClipPlane +
            halfHeight * halfHeight * (1f + view.aspect * view.aspect)));
        float wantedDistance = distance;

        // SphereCast omits colliders overlapping its origin; handle those explicitly.
        foreach (Collider overlap in Physics.OverlapSphere(pivot, radius, collisionMask,
                     QueryTriggerInteraction.Ignore))
        {
            if (!IsObstacle(overlap)) continue;
            wantedDistance = 0f;
            break;
        }
        if (wantedDistance > 0f)
        {
            // Inspect every hit: ignoring the player must not hide a wall behind it.
            foreach (RaycastHit hit in Physics.SphereCastAll(pivot, radius, back, distance,
                         collisionMask, QueryTriggerInteraction.Ignore))
            {
                if (IsObstacle(hit.collider))
                    wantedDistance = Mathf.Min(wantedDistance,
                        Mathf.Max(0f, hit.distance - collisionPadding));
            }
        }

        // Immediate contraction; bounded easing out never crosses the current obstruction.
        currentDistance = snap || wantedDistance < currentDistance
            ? wantedDistance
            : Mathf.Lerp(currentDistance, wantedDistance,
                1f - Mathf.Exp(-returnSpeed * Time.deltaTime));
        transform.SetPositionAndRotation(pivot + back * currentDistance, rotation);
    }

    private void ReleaseMouse()
    {
        if (!orbiting) return;
        orbiting = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnApplicationFocus(bool focused)
    {
        if (focused) return;
        ReleaseMouse();
        waitForRelease = true;
    }

    private void OnApplicationPause(bool paused)
    {
        if (!paused) return;
        ReleaseMouse();
        waitForRelease = true;
    }

    private void OnDisable()
    {
        ReleaseMouse();
    }

    private void OnValidate()
    {
        minPitch = Mathf.Clamp(minPitch, 0f, 89f);
        maxPitch = Mathf.Clamp(maxPitch, minPitch, 89f);
        initialPitch = Mathf.Clamp(initialPitch, minPitch, maxPitch);
    }
}
