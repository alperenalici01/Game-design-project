using UnityEngine;

// Gerçek yol mesafesine göre yuvarlanır; ışınlanmayı temas taramasından ayrı tutar.
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public class MertcanRollingBall : MonoBehaviour
{
    [Header("Course 04 controlled rolling hazard")]
    [SerializeField] private Transform[] ballRoute = null;
    [SerializeField, Min(0.01f)] private float ballSpeed = 3f;
    [SerializeField, Min(0f)] private float ballEndWait = 3f;
    [SerializeField] private bool ballStartAtEnd = true;
    [SerializeField] private bool ballTeleportLoop = false;
    [SerializeField] private Renderer[] ballTreadRenderers = null;
    private Rigidbody rollingBody;
    private Vector3 rollingPosition;
    private Quaternion rollingRotation;
    private float rollingRadius;
    private float rollingWait;
    private int rollingNode;
    private int rollingStep;

    private MertcanHazard hazard;

    private void Start()
    {
        hazard = GetComponent<MertcanHazard>();
        rollingBody = GetComponent<Rigidbody>();
        SphereCollider sphere = GetComponent<SphereCollider>();
        if (rollingBody == null || sphere == null || ballRoute == null || ballRoute.Length < 2)
        {
            Debug.LogError("Course04 ball needs a kinematic body, sphere trigger and route points.", this);
            enabled = false;
            return;
        }
        foreach (Transform point in ballRoute)
        {
            if (point != null) continue;
            Debug.LogError("Course04 ball has an unassigned route point.", this);
            enabled = false;
            return;
        }
        rollingRadius = sphere.radius * Mathf.Max(Mathf.Abs(transform.lossyScale.x),
            Mathf.Abs(transform.lossyScale.y), Mathf.Abs(transform.lossyScale.z));
        rollingNode = ballStartAtEnd ? ballRoute.Length - 1 : 0;
        rollingStep = ballStartAtEnd ? -1 : 1;
        rollingPosition = ballRoute[rollingNode].position;
        rollingRotation = transform.rotation.normalized;
        rollingWait = ballTeleportLoop ? 0f : Mathf.Max(0f, ballEndWait);
        if (ballTeleportLoop) rollingBody.interpolation = RigidbodyInterpolation.None;
        rollingBody.position = rollingPosition;
        rollingBody.rotation = rollingRotation;
        if (ballTeleportLoop) CheckRollingOverlap(rollingPosition);
        if (ballTreadRenderers == null) return;
        MaterialPropertyBlock tread = new MaterialPropertyBlock();
        tread.SetColor("_BaseColor", new Color(0.32f, 0.008f, 0.008f, 1f));
        tread.SetColor("_Color", new Color(0.32f, 0.008f, 0.008f, 1f));
        foreach (Renderer accent in ballTreadRenderers)
            if (accent != null) accent.SetPropertyBlock(tread);
    }

    private void FixedUpdate()
    {
        if (rollingBody == null || !enabled) return;
        float remaining = Time.fixedDeltaTime;
        float speed = Mathf.Max(0.01f, ballSpeed);
        // Consume each segment in order, including any unused time after a corner or wait.
        for (int guard = 0; guard < 64 && remaining > 0f; guard++)
        {
            if (rollingWait > 0f)
            {
                float spent = Mathf.Min(remaining, rollingWait);
                rollingWait -= spent;
                remaining -= spent;
                continue;
            }
            int next = rollingNode + rollingStep;
            Vector3 destination = ballRoute[next].position;
            float distance = Vector3.Distance(rollingPosition, destination);
            float traveled = Mathf.Min(distance, speed * remaining);
            Vector3 position = Vector3.MoveTowards(rollingPosition, destination, traveled);
            Vector3 delta = position - rollingPosition;
            if (delta.sqrMagnitude > 0.0000001f)
            {
                if (ballTeleportLoop)
                {
                    // Sweep only this real movement step, never the reset between endpoints.
                    CheckRollingOverlap(rollingPosition);
                    foreach (RaycastHit hit in Physics.SphereCastAll(rollingPosition, rollingRadius,
                                 delta.normalized, delta.magnitude, ~0, QueryTriggerInteraction.Collide))
                        hazard.NotifyPlayer(hit.collider);
                }
                Vector3 axis = Vector3.Cross(Vector3.up, delta.normalized);
                float degrees = delta.magnitude / Mathf.Max(0.01f, rollingRadius) * Mathf.Rad2Deg;
                rollingRotation = (Quaternion.AngleAxis(degrees, axis) * rollingRotation).normalized;
            }
            rollingPosition = position;
            remaining -= traveled / speed;
            if (traveled < distance) break;
            rollingNode = next;
            if (rollingNode == 0 || rollingNode == ballRoute.Length - 1)
            {
                if (ballTeleportLoop)
                {
                    CheckRollingOverlap(rollingPosition);
                    TeleportRollingBallToStart();
                }
                else
                {
                    rollingStep = -rollingStep;
                    rollingWait = Mathf.Max(0f, ballEndWait);
                }
            }
        }
        // Kinematic movement and a trigger collider give contact without a physical shove.
        rollingBody.MovePosition(rollingPosition);
        rollingBody.MoveRotation(rollingRotation);
    }

    private void CheckRollingOverlap(Vector3 center)
    {
        foreach (Collider other in Physics.OverlapSphere(center, rollingRadius, ~0,
                     QueryTriggerInteraction.Collide))
            hazard.NotifyPlayer(other);
    }

    private void TeleportRollingBallToStart()
    {
        rollingNode = ballStartAtEnd ? ballRoute.Length - 1 : 0;
        rollingStep = ballStartAtEnd ? -1 : 1;
        rollingPosition = ballRoute[rollingNode].position;
        rollingWait = 0f;
        // A hard pose reset, not MovePosition: there is no travel or sweep across the map.
        // Keep the accumulated visual rotation; the teleport contributes zero rolling distance.
        rollingBody.position = rollingPosition;
        rollingBody.rotation = rollingRotation;
        transform.SetPositionAndRotation(rollingPosition, rollingRotation);
        Physics.SyncTransforms();
        CheckRollingOverlap(rollingPosition);
        // Any unused fixed-step time continues as ordinary movement in the same direction.
    }
}
