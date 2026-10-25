using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float distance = 6f;
    [SerializeField] private float height = 3f;
    [SerializeField] private float lookHeight = 1f;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError(
                "Kameranın Target alanına Player'ı bağla.", this
            );

            enabled = false;
            return;
        }

        UpdateCamera();
    }

    void LateUpdate()
    {
        if (target == null) return;

        UpdateCamera();
    }

    private void UpdateCamera()
    {
        // Karakterin yalnızca yatay dönüşünü takip et.
        Quaternion yaw = Quaternion.Euler(
            0f, target.eulerAngles.y, 0f
        );

        // Ek gecikme olmadan karakterin arkasında kal.
        transform.position =
            target.position
            + yaw * new Vector3(0f, 0f, -distance)
            + Vector3.up * height;

        transform.LookAt(
            target.position + Vector3.up * lookHeight
        );
    }
}

