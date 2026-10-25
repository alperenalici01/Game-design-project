using UnityEngine;

public class LegSwing : MonoBehaviour
{
    [Header("Bones (drag them from the Hierarchy)")]
    [SerializeField] private Transform leftLeg;    // e.g. thigh_stretch.l
    [SerializeField] private Transform rightLeg;   // e.g. thigh_stretch.r
    [SerializeField] private Transform leftArm;    // optional
    [SerializeField] private Transform rightArm;   // optional

    [Header("Swing")]
    [SerializeField] private float legAngle = 30f;          // degrees forward/back
    [SerializeField] private float armAngle = 20f;
    [SerializeField] private Vector3 swingAxis = Vector3.right; // try (0,0,1) if legs swing sideways
    [SerializeField] private Vector3 armSwingAxis = Vector3.forward; // arms often need a different axis
    // tick this if both arms move forward together
    [SerializeField] private bool mirroredArms = false;
    [SerializeField] private float stepsPerSecond = 4f;     // keep the same as WalkBob
    [SerializeField] private float speedForFullSwing = 5f;  // keep the same as WalkBob
    // tick this if both legs move forward together (left/right bones are mirrored)
    [SerializeField] private bool mirroredBones = false;

    private Rigidbody rb;
    private Quaternion leftLegStart, rightLegStart, leftArmStart, rightArmStart;
    private float stepTime;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        // remember each bone's resting pose
        if (leftLeg)  leftLegStart  = leftLeg.localRotation;
        if (rightLeg) rightLegStart = rightLeg.localRotation;
        if (leftArm)  leftArmStart  = leftArm.localRotation;
        if (rightArm) rightArmStart = rightArm.localRotation;
    }

    // LateUpdate: after everything else has moved this frame
    void LateUpdate()
    {
        Vector3 v = rb.linearVelocity;
        float speed = new Vector3(v.x, 0f, v.z).magnitude;
        float amount = Mathf.Clamp01(speed / speedForFullSwing);

        // same step clock as WalkBob, so the hop matches the steps
        stepTime += Time.deltaTime * stepsPerSecond * amount;
        float s = Mathf.Sin(stepTime * Mathf.PI);   // -1 .. +1
        // the right side gets the opposite sign, unless its bones are mirrored
        float r = mirroredBones ? s : -s;
        float ra = mirroredArms ? s : -s;

        // legs: one forward, the other back
        Swing(leftLeg,  leftLegStart,   s * legAngle * amount, swingAxis);
        Swing(rightLeg, rightLegStart,  r * legAngle * amount, swingAxis);
        // arms: opposite to the legs on the same side
        Swing(leftArm,  leftArmStart,  -s * armAngle * amount, armSwingAxis);
        Swing(rightArm, rightArmStart, -ra * armAngle * amount, armSwingAxis);
    }

    void Swing(Transform bone, Quaternion start, float angle, Vector3 axis)
    {
        if (bone == null) return;
        bone.localRotation = start * Quaternion.AngleAxis(angle, axis);
    }
}
