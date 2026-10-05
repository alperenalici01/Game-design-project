using UnityEngine;

public class TurtlePatrol : MonoBehaviour
{
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 1.5f; // metres per second

    private Vector3 pointA;
    private bool goingToB = true;

    void Start()
    {
        pointA = transform.position; // A = where we start
    }

    void Update()
    {
        Vector3 target = goingToB ? pointB.position : pointA;

        Vector3 direction = target - transform.position;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction); // face where we walk
        }

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (transform.position == target)
        {
            goingToB = !goingToB; // arrived: turn around
        }
    }
}