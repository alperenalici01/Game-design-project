using UnityEngine;

public class PlatformOrbit : MonoBehaviour
{
   public Transform centerCube; // Drag your middle cube here in the Inspector
    public float speed = 50f;
    public float radius = 2f;    // Distance from the center cube
    
    private float angle;

    void Start()
    {
        if (centerCube != null)
        {
            // Calculate starting angle based on vertical (Y) and horizontal (X) position
            Vector3 dir = transform.position - centerCube.position;
            angle = Mathf.Atan2(dir.y, dir.x);
        }
    }

    void Update()
    {
        if (centerCube != null)
        {
            // Increment angle over time
            angle += speed * Mathf.Deg2Rad * Time.deltaTime;

            // Calculate X and Y position for a vertical Ferris wheel loop
            float x = centerCube.position.x + Mathf.Cos(angle) * radius;
            float y = centerCube.position.y + Mathf.Sin(angle) * radius;
            float z = centerCube.position.z; // Keep depth constant

            // Update position
            transform.position = new Vector3(x, y, z);

            // Lock rotation so the cubes never tilt and stay perfectly flat for the player
            transform.rotation = Quaternion.identity; 
        }
    }
}