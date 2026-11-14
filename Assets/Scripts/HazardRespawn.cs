using UnityEngine;

public class HazardRespawn : MonoBehaviour
{
    private Vector3 startPosition;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Karakterin doğduğu X ve Z'yi al, Y'yi yerin içine girmemesi için en az 1.2f yap
            startPosition = new Vector3(
                player.transform.position.x,
                Mathf.Max(player.transform.position.y, 1.2f),
                player.transform.position.z
            );
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // Zeminin biraz üzerinden bırak ki çarpışma kusursuz algılansın
            other.transform.position = startPosition + Vector3.up * 0.5f;
        }
    }
}