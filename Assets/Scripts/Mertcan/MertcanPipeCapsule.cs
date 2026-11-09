using UnityEngine;

// Kapsülü boru içinde bekletir, çıkarır ve geri çeker; içerideyken collider kapanır.
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public class MertcanPipeCapsule : MonoBehaviour
{
    [Header("Pipe capsule cycle (new pipe hazards only)")]
    [SerializeField, Min(0f)] private float pipeWaitInside = 3f;
    [SerializeField, Min(0f)] private float pipeWaitOutside = 1f;
    [SerializeField, Min(0.01f)] private float pipeRiseSpeed = 2.65f;
    [SerializeField, Min(0.01f)] private float pipeFallSpeed = 2.65f;
    [SerializeField, Min(0.01f)] private float pipeExitHeight = 2.65f;
    [SerializeField, Min(0f)] private float pipePhaseOffset = 0f;
    [SerializeField] private float pipeMouthLocalY = 2.51f;
    private Vector3 pipeHiddenPosition;
    private float pipeCycleStart;
    private CapsuleCollider pipeCollider;
    private Renderer pipeRenderer;

    private void Start()
    {
        pipeHiddenPosition = transform.localPosition;
        pipeCycleStart = Time.time;
        pipeCollider = GetComponent<CapsuleCollider>();
        pipeRenderer = GetComponent<Renderer>();
        Update();
    }

    private void Update()
    {
        float height = Mathf.Max(0.01f, pipeExitHeight);
        float upDuration = height / Mathf.Max(0.01f, pipeRiseSpeed);
        float downDuration = height / Mathf.Max(0.01f, pipeFallSpeed);
        float inside = Mathf.Max(0f, pipeWaitInside);
        float outside = Mathf.Max(0f, pipeWaitOutside);
        float time = Mathf.Repeat(Time.time - pipeCycleStart + pipePhaseOffset,
            inside + upDuration + outside + downDuration);
        float lift;
        if (time < inside) lift = 0f;
        else if (time < inside + upDuration)
            lift = height * (time - inside) / upDuration;
        else if (time < inside + upDuration + outside) lift = height;
        else lift = height * (1f - (time - inside - upDuration - outside) / downDuration);

        transform.localPosition = pipeHiddenPosition + Vector3.up * lift;
        if (pipeCollider == null) return;
        // Collider is on this moving capsule, never on the safe pipe or its landing cap.
        // Do not use Collider.bounds here: disabled colliders have empty bounds.
        float top = transform.localPosition.y +
            (pipeCollider.center.y + pipeCollider.height * 0.5f) * transform.localScale.y;
        bool exposed = top > pipeMouthLocalY + 0.03f;
        pipeCollider.enabled = exposed;
        if (pipeRenderer != null) pipeRenderer.enabled = exposed;
    }
}
