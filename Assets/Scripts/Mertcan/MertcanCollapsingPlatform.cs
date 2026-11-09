using UnityEngine;

// Sadece oyuncu üstüne bastığında uyarı, düşme ve güvenli yeniden oluşma döngüsünü çalıştırır.
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public class MertcanCollapsingPlatform : MonoBehaviour
{
    [Header("Course 07 collapsing platform (visual falls; support never carries down)")]
    [SerializeField] private Rigidbody collapsePlayerBody = null;
    [SerializeField] private Transform collapseVisual = null;
    [SerializeField, Min(0f)] private float collapseDelay = 0.8f;
    [SerializeField, Min(0f)] private float collapseRespawnDelay = 3f;
    [SerializeField, Min(0.1f)] private float collapseFallDuration = 0.6f;
    [SerializeField, Min(1f)] private float collapseDropDistance = 5f;
    [SerializeField, Range(0f, 0.05f)] private float collapseShake = 0.025f;
    private BoxCollider collapseSupport;
    private Vector3 collapseHomePosition;
    private Quaternion collapseHomeRotation;
    private Vector3 collapseVisualHome;
    private Quaternion collapseVisualRotation;
    private bool collapseColliderInitiallyEnabled;
    private int collapseState; // 0 ready, 1 warning, 2 falling, 3 waiting for safe reappearance.
    private float collapseTimer;

    private void Start()
    {
        collapseSupport = GetComponent<BoxCollider>();
        if (collapseSupport == null || collapseSupport.isTrigger || collapsePlayerBody == null ||
            collapseVisual == null || collapseVisual == transform || !collapseVisual.IsChildOf(transform))
        {
            Debug.LogError("Course07 needs a solid support, player physics root and separate visual child.", this);
            enabled = false;
            return;
        }
        collapseHomePosition = transform.position;
        collapseHomeRotation = transform.rotation;
        collapseVisualHome = collapseVisual.localPosition;
        collapseVisualRotation = collapseVisual.localRotation;
        collapseColliderInitiallyEnabled = collapseSupport.enabled;
        collapseState = 0;
        collapseTimer = 0f;
    }

    private void TryStartCollapse(Collision collision)
    {
        if (collapseSupport == null || collapseState != 0 ||
            collision.collider.attachedRigidbody != collapsePlayerBody ||
            collapsePlayerBody.linearVelocity.y > 0.5f) return;
        float top = collapseSupport.bounds.max.y;
        if (collision.collider.bounds.min.y < top - 0.12f) return;
        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint contact = collision.GetContact(i);
            // This callback belongs to the platform: a standing player's normal points DOWN.
            // Side/underside contacts, coins and other rigidbodies cannot arm the platform.
            if (contact.normal.y >= -0.5f || contact.point.y < top - 0.08f) continue;
            collapseState = 1;
            collapseTimer = 0f;
            return;
        }
    }

    private void FixedUpdate()
    {
        if (collapseSupport == null || collapseVisual == null || collapseState == 0) return;
        collapseTimer += Time.fixedDeltaTime;
        if (collapseState == 1)
        {
            // Shake the mesh only. The stable collider neither shakes nor imparts an impulse.
            collapseVisual.localPosition = collapseVisualHome + new Vector3(
                Mathf.Sin(collapseTimer * 65f), 0f, Mathf.Sin(collapseTimer * 49f)) * collapseShake;
            if (collapseTimer < Mathf.Max(0f, collapseDelay)) return;
            collapseSupport.enabled = false;
            collapseVisual.localPosition = collapseVisualHome;
            collapseState = 2;
            collapseTimer = 0f;
            // No Patroller/shuttle role or parenting: the departing player is never carried down.
            return;
        }
        if (collapseState == 2)
        {
            float fraction = Mathf.Clamp01(collapseTimer / Mathf.Max(0.1f, collapseFallDuration));
            collapseVisual.localPosition = collapseVisualHome +
                Vector3.down * (collapseDropDistance * fraction * fraction);
            if (fraction < 1f) return;
            collapseVisual.gameObject.SetActive(false);
            collapseState = 3;
            collapseTimer = 0f;
            return;
        }
        if (collapseTimer < Mathf.Max(0f, collapseRespawnDelay)) return;
        // A disabled collider has empty bounds; derive the HOME box from its saved geometry.
        Vector3 scale = transform.lossyScale;
        Vector3 halfSize = Vector3.Scale(collapseSupport.size * 0.5f,
            new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z)));
        Vector3 center = collapseHomePosition + collapseHomeRotation *
            Vector3.Scale(collapseSupport.center, scale);
        foreach (Collider occupant in Physics.OverlapBox(center, halfSize + Vector3.one * 0.06f,
                     collapseHomeRotation, ~0, QueryTriggerInteraction.Ignore))
            if (occupant.attachedRigidbody == collapsePlayerBody) return;

        transform.SetPositionAndRotation(collapseHomePosition, collapseHomeRotation);
        collapseVisual.localPosition = collapseVisualHome;
        collapseVisual.localRotation = collapseVisualRotation;
        collapseVisual.gameObject.SetActive(true);
        collapseSupport.enabled = collapseColliderInitiallyEnabled;
        collapseState = 0;
        collapseTimer = 0f;
    }
    private void OnCollisionEnter(Collision collision)
    {
        TryStartCollapse(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        TryStartCollapse(collision);
    }
}
