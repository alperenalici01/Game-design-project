using UnityEngine;

public class Spear : MonoBehaviour
{
        public Vector3 direction = new Vector3(-1f, 1f, 0f); 
    public float distance = 5f;
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
 
        
        float t = Mathf.PingPong(travelled, distance);
 
        transform.position = startPosition + direction.normalized * t;
    }

}
