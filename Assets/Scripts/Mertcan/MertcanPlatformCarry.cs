using UnityEngine;

// Yalnızca üzerinde durulan platformun yer değiştirmesini ekler; ikinci hareket kontrolcüsü değildir.
[DefaultExecutionOrder(-90)]
[DisallowMultipleComponent]
public class MertcanPlatformCarry : MonoBehaviour
{
    private Rigidbody body;
    private MertcanRespawn respawn;
    private Patroller platform;
    private MertcanShuttlePlatform shuttlePlatform;
    private Vector3 lastPlatformPosition;
    private float lastContact = float.NegativeInfinity;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        respawn = GetComponent<MertcanRespawn>();
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
        if (body == null || (respawn != null && !respawn.CanReadPlatformContact)) return;
        MertcanShuttlePlatform local = collision.collider.GetComponentInParent<MertcanShuttlePlatform>();
        if (local == null || !local.isActiveAndEnabled) local = null;
        Patroller candidate = local == null ? collision.collider.GetComponentInParent<Patroller>() : null;
        if (local == null && (candidate == null || !candidate.isActiveAndEnabled)) return;
        Transform next = local != null ? local.transform : candidate.transform;

        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y <= 0.5f) continue;
            Transform current = shuttlePlatform != null ? shuttlePlatform.transform :
                (platform != null ? platform.transform : null);
            if (current != next)
            {
                if (current != null && Time.fixedTime - lastContact <= Time.fixedDeltaTime * 1.5f)
                {
                    Vector3 oldOffset = current.position - body.position;
                    Vector3 newOffset = next.position - body.position;
                    oldOffset.y = newOffset.y = 0f;
                    if (newOffset.sqrMagnitude >= oldOffset.sqrMagnitude) return;
                }
                // Yeni desteğe geçince önceki platformun konum örneğini bırak.
                platform = candidate;
                shuttlePlatform = local;
                lastPlatformPosition = next.position;
            }
            lastContact = Time.fixedTime;
            return;
        }
    }

    private void LateUpdate()
    {
        if (body == null || (respawn != null && !respawn.CanCarry)) return;
        Transform support = shuttlePlatform != null ? shuttlePlatform.transform :
            (platform != null ? platform.transform : null);
        if (support == null) return;
        bool supportEnabled = shuttlePlatform != null
            ? shuttlePlatform.isActiveAndEnabled : platform.isActiveAndEnabled;
        if (!supportEnabled || Time.fixedTime - lastContact > Time.fixedDeltaTime * 1.5f)
        {
            ClearPlatform();
            return;
        }
        Vector3 position = support.position;
        Vector3 displacement = position - lastPlatformPosition;
        float platformUpSpeed = Time.deltaTime > 0f ? displacement.y / Time.deltaTime : 0f;
        if (body.linearVelocity.y > platformUpSpeed + 0.5f)
        {
            ClearPlatform();
            return;
        }
        // Hızı değiştirme; ortak PlayerMove bu yer değiştirmeyi silmez veya iki kez uygulamaz.
        body.position += displacement;
        lastPlatformPosition = position;
    }

    public void ClearPlatform()
    {
        platform = null;
        shuttlePlatform = null;
        lastPlatformPosition = Vector3.zero;
        lastContact = float.NegativeInfinity;
    }

    private void OnDisable()
    {
        ClearPlatform();
    }
}
