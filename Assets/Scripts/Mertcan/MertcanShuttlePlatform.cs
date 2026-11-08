using UnityEngine;

// Başlangıç fazı ve uç beklemeleriyle kendi hattında ileri geri gider.
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public class MertcanShuttlePlatform : MonoBehaviour
{
    [Header("Course 03 shuttle platform (local scene instances only)")]
    [SerializeField] private Vector3 shuttleDirection = Vector3.left;
    [SerializeField, Min(0.01f)] private float shuttleDistance = 4f;
    [SerializeField, Min(0.01f)] private float shuttleSpeed = 1.6f;
    [SerializeField, Min(0f)] private float shuttleStartPhase = 0f;
    [SerializeField, Min(0f)] private float shuttleEndWait = 2f;
    private Vector3 shuttleStartPosition;
    private float shuttleStartTime;

    private void Start()
    {
        shuttleStartPosition = transform.localPosition;
        shuttleStartTime = Time.time;
        Update();
    }

    private void Update()
    {
        float travel = Mathf.Max(0.01f, shuttleDistance);
        float duration = travel / Mathf.Max(0.01f, shuttleSpeed);
        float wait = Mathf.Max(0f, shuttleEndWait);
        float time = Mathf.Repeat(Time.time - shuttleStartTime + shuttleStartPhase,
            2f * (wait + duration));
        float fraction;
        if (time < wait) fraction = 0f;
        else if (time < wait + duration) fraction = (time - wait) / duration;
        else if (time < 2f * wait + duration) fraction = 1f;
        else fraction = 1f - (time - 2f * wait - duration) / duration;
        transform.localPosition = shuttleStartPosition + shuttleDirection.normalized * travel * fraction;
    }
}
