using UnityEngine;

public class CameraFollow_Zafer : MonoBehaviour
{
    [SerializeField] private Transform target; // Drag your Player here in the Inspector
    [SerializeField] private float smoothSpeed = 10f;
    private Vector3 offset;

    void Start()
    {
        if (target != null)
        {
            // Automatically calculates the initial distance/angle based on how you placed the camera in the editor
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Target position to follow the player
        Vector3 desiredPosition = target.position + offset;
        
        // Smoothly move the camera towards the player's position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}