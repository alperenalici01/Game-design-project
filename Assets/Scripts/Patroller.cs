using UnityEngine;

public class Patroller : MonoBehaviour
{
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2.0f;

    private Vector3 pointA;
    private bool goingToB = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pointA = transform.position;
    }

    // Update is called once per frame
    void Update()
        {
            Vector3 target = goingToB ? pointB.position : pointA;
            
            // Obje mevcut konumundan hedefe doğru normal bir şekilde hareket eder
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            
            if (transform.position == target)
            {
                goingToB = !goingToB;
            }
        }
}
