using UnityEngine;

// Hareket koduna karışmadan düşmeyi ve başlangıca dönüşü yönetir.
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public class MertcanRespawn : MonoBehaviour
{
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private ThirdPersonCamera followCamera;
    [SerializeField] private float fallY = -15f;
    [SerializeField] private float respawnCameraYaw = 270f;
    [SerializeField] private float respawnCameraPitch = 20f;

    private Rigidbody body;
    private PlayerMove movement;
    private PlayerJump jump;
    private MertcanPlatformCarry carry;
    private MertcanLevelCompletion completion;
    private bool pendingRespawn;
    private bool resumeControls;
    private bool movementWasEnabled;
    private bool jumpWasEnabled;
    private float ignoreContactsUntil = float.NegativeInfinity;

    public bool IsCompleted => completion != null && completion.IsCompleted;
    public bool CanCarry => !IsCompleted && !pendingRespawn;
    public bool CanReadPlatformContact => CanCarry && Time.fixedTime > ignoreContactsUntil;

    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody>();
        movement = GetComponent<PlayerMove>();
        jump = GetComponent<PlayerJump>();
        carry = GetComponent<MertcanPlatformCarry>();
        completion = GetComponent<MertcanLevelCompletion>();
        if (body == null || movement == null || jump == null || respawnPoint == null)
        {
            Debug.LogError("Mertcan oyuncusunda fizik gövdesi, hareket, zıplama ve başlangıç noktası gerekli.", this);
            enabled = false;
        }
    }

    protected virtual void Update()
    {
        if (IsCompleted || body == null) return;
        // Ortak hareket bileşenleri güncel girdiyi okumadan önce kontrolü geri aç.
        if (resumeControls)
        {
            movement.enabled = movementWasEnabled;
            jump.enabled = jumpWasEnabled;
            resumeControls = false;
        }
        if (body.position.y < fallY) RequestRespawn();
    }

    public void RequestRespawn()
    {
        if (!enabled || IsCompleted || body == null || pendingRespawn || resumeControls ||
            Time.fixedTime <= ignoreContactsUntil) return;
        pendingRespawn = true;
        movementWasEnabled = movement.enabled;
        jumpWasEnabled = jump.enabled;
        movement.enabled = false;
        jump.enabled = false;
    }

    protected virtual void LateUpdate()
    {
        if (!IsCompleted && pendingRespawn) Respawn();
    }

    private void Respawn()
    {
        if (carry != null) carry.ClearPlatform();
        movement.SetGrip(1f);
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.position = respawnPoint.position;
        body.rotation = respawnPoint.rotation;
        body.transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);
        Physics.SyncTransforms();
        if (followCamera != null)
            followCamera.SnapToRespawn(respawnCameraYaw, respawnCameraPitch);
        pendingRespawn = false;
        resumeControls = true;
        ignoreContactsUntil = Time.fixedTime + Time.fixedDeltaTime * 2f;
    }

    public void StopForCompletion()
    {
        pendingRespawn = false;
        resumeControls = false;
        if (carry != null) carry.ClearPlatform();
        movement.enabled = false;
        jump.enabled = false;
        movement.SetGrip(1f);
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.isKinematic = true;
    }

    protected virtual void OnDisable()
    {
        if (carry != null) carry.ClearPlatform();
        if (body != null && (pendingRespawn || resumeControls))
        {
            if (movement != null) movement.enabled = movementWasEnabled;
            if (jump != null) jump.enabled = jumpWasEnabled;
        }
        pendingRespawn = false;
        resumeControls = false;
    }
}
