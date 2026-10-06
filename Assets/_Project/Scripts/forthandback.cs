using UnityEngine;

public class forthandback : MonoBehaviour
{
    
    public Vector3 direction = Vector3.right;
    public float distance = 10f;
    public float speed = 2f;
    private Vector3 startPosition;
    private float travelled;
 
    void Start()
    {
        startPosition = transform.position;
    }
 
    void Update()
    {
        travelled += speed * Time.deltaTime;
        if (travelled >= distance)
            travelled = 0f;
 
        transform.position = startPosition + direction.normalized * travelled;
    }

}
