using UnityEngine;

[DisallowMultipleComponent]
public class PlatformRider : MonoBehaviour
{
    [Header("Ground check")]
    public float checkDistance = 0.4f;
    public LayerMask groundMask = ~0;

    [Header("Debug")]
    public bool debugLog = true;

    private Collider col;
    private CharacterController cc;
    private Rigidbody rb;

    private Transform currentPlatform;
    private Vector3 lastPlatPos;
    private Quaternion lastPlatRot;
    private string lastLoggedState = "";

    private void Awake()
    {
        col = GetComponent<Collider>();
        cc = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();

        if (col == null)
        {
            Debug.LogError($"[PlatformRider] {name} üzerinde Collider/CharacterController yok, script çalışamaz.");
            return;
        }

        if (debugLog)
        {
            Debug.Log($"[PlatformRider] Obje: {name} | Tag: {tag} | CharacterController: {cc != null} | Rigidbody: {rb != null} | Collider: {col.GetType().Name}");
        }
    }

    private void LateUpdate()
    {
        if (col == null) return;

        Transform platform = FindPlatformBelow(out string belowName);

        Log($"Altindaki collider: {belowName} | Platform: {(platform != null ? platform.name : "yok")}");

        if (platform != null && platform == currentPlatform)
        {
            Quaternion deltaRot = platform.rotation * Quaternion.Inverse(lastPlatRot);
            Vector3 rel = transform.position - lastPlatPos;
            Vector3 newPos = platform.position + deltaRot * rel;
            Vector3 delta = newPos - transform.position;

            if (cc != null) cc.Move(delta);
            else transform.position += delta;

            // Platform dönüyorsa karakter de onunla birlikte dönsün
            float yaw = deltaRot.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, yaw, 0f) * transform.rotation;
        }

        currentPlatform = platform;
        if (platform != null)
        {
            lastPlatPos = platform.position;
            lastPlatRot = platform.rotation;
        }
    }

    private Transform FindPlatformBelow(out string belowName)
    {
        belowName = "hicbir sey";

        Bounds b = col.bounds;
        Vector3 origin = new Vector3(b.center.x, b.min.y + 0.1f, b.center.z);

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 0.1f + checkDistance,
                            groundMask, QueryTriggerInteraction.Ignore))
        {
            belowName = hit.collider.name;

            RotatingPlatform rp = hit.collider.GetComponentInParent<RotatingPlatform>();
            if (rp != null) return rp.transform;
        }

        return null;
    }

    private void Log(string state)
    {
        if (!debugLog || state == lastLoggedState) return;
        lastLoggedState = state;
        Debug.Log($"[PlatformRider] {state}");
    }
}