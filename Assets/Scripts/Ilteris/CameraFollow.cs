using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;                          // the Player
    [SerializeField] private Vector3 offset = new Vector3(0f, 6f, -12f); // above and to the side
    [SerializeField] private float smooth = 5f;                          // higher = follows faster

    // LateUpdate: after the player has moved this frame
    void LateUpdate()
    {
        Vector3 wanted = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, wanted, smooth * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up);   // look at the player's chest
    }
}
