using UnityEngine;

// Kolun sabit dikey eksen etrafındaki açısını fizik adımında ilerletir.
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public class MertcanRotatingArm : MonoBehaviour
{
    [Header("Course 05 rotating arm (scene instances only)")]
    [SerializeField] private Transform armBody = null;
    [SerializeField, Min(0f)] private float armAngularSpeed = 60f;
    [SerializeField] private bool armClockwise = true;
    [SerializeField] private float armStartAngle = 0f;
    [SerializeField] private float armHeight = 0.55f;
    private Rigidbody armPivotBody;
    private float armAngle;

    private void Start()
    {
        armPivotBody = GetComponent<Rigidbody>();
        if (armPivotBody == null || armBody == null)
        {
            Debug.LogError("Course05 arm requires its kinematic pivot body and horizontal arm child.", this);
            enabled = false;
            return;
        }
        armAngle = Mathf.Repeat(armStartAngle, 360f);
        armBody.localPosition = new Vector3(0f, armHeight, 0f);
        armPivotBody.rotation = Quaternion.Euler(0f, armAngle, 0f);
    }

    private void FixedUpdate()
    {
        if (armPivotBody == null || armBody == null) return;
        float direction = armClockwise ? 1f : -1f;
        armAngle = Mathf.Repeat(armAngle + direction * armAngularSpeed * Time.fixedDeltaTime, 360f);
        armBody.localPosition = new Vector3(0f, armHeight, 0f);
        // Fresh Euler quaternion avoids accumulated quaternion drift; yaw is about the shared vertical axis.
        armPivotBody.MoveRotation(Quaternion.Euler(0f, armAngle, 0f));
    }
}
