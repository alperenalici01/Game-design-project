using UnityEngine;

/// <summary>
/// Kaygan rampa. Herhangi bir Collider'lı objeye (prefab dahil) eklenebilir.
/// Unity 6 (6000.x) uyumlu: PhysicsMaterial kullanır.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class SlipperyRamp : MonoBehaviour
{
    [Header("Fizik Materyali")]
    [Tooltip("Hareket halindeki sürtünme (0 = buz gibi)")]
    [SerializeField, Min(0f)] private float dynamicFriction = 0f;
    [Tooltip("Duran objenin sürtünmesi (0 = hiç tutunamaz)")]
    [SerializeField, Min(0f)] private float staticFriction = 0f;
    [SerializeField, Range(0f, 1f)] private float bounciness = 0f;

    [Header("Ekstra Kayma Kuvveti")]
    [Tooltip("Rampa yönünde ekstra ivme uygular (düz zemine yakın eğimlerde işe yarar)")]
    [SerializeField] private bool applySlideForce = true;
    [SerializeField, Min(0f)] private float slideAcceleration = 12f;
    [Tooltip("Bu açının altındaki yüzeylerde kuvvet uygulanmaz")]
    [SerializeField, Range(0f, 89f)] private float minSlopeAngle = 5f;

    private Collider col;
    private PhysicsMaterial runtimeMaterial;

    private void Reset()
    {
        // Bileşen eklenirken varsayılan: kaygan
        dynamicFriction = 0f;
        staticFriction = 0f;
    }

    private void Awake()
    {
        col = GetComponent<Collider>();
        ApplyMaterial();
    }

    private void OnValidate()
    {
        // Editörde değer değişince anında güncelle
        if (Application.isPlaying)
        {
            if (col == null) col = GetComponent<Collider>();
            ApplyMaterial();
        }
    }

    private void ApplyMaterial()
    {
        if (runtimeMaterial == null)
        {
            runtimeMaterial = new PhysicsMaterial("SlipperyRamp_Runtime")
            {
                hideFlags = HideFlags.HideAndDontSave
            };
        }

        runtimeMaterial.dynamicFriction = dynamicFriction;
        runtimeMaterial.staticFriction = staticFriction;
        runtimeMaterial.bounciness = bounciness;
        // Diğer objenin sürtünmesi ne olursa olsun en düşük değer kazansın
        runtimeMaterial.frictionCombine = PhysicsMaterialCombine.Minimum;
        runtimeMaterial.bounceCombine = PhysicsMaterialCombine.Minimum;

        col.sharedMaterial = runtimeMaterial;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!applySlideForce) return;

        Rigidbody rb = collision.rigidbody;
        if (rb == null || rb.isKinematic) return;

        Vector3 normal = collision.GetContact(0).normal;
        if (Vector3.Dot(normal, Vector3.up) < 0f) normal = -normal; // yukarı bakan yüzey normali

        float slope = Vector3.Angle(normal, Vector3.up);
        if (slope < minSlopeAngle || slope > 89f) return;

        // Yüzey boyunca aşağı yön
        Vector3 slideDir = Vector3.ProjectOnPlane(Vector3.down, normal).normalized;
        rb.AddForce(slideDir * slideAcceleration, ForceMode.Acceleration);
    }

    private void OnDestroy()
    {
        if (runtimeMaterial != null) Destroy(runtimeMaterial);
    }
}
