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

            // Düşen kargoyu bırak, buz tutuşunu sıfırla
            PlayerCarry carry = other.GetComponentInParent<PlayerCarry>();
            if (carry != null && carry.IsCarrying()) carry.DropCargo();
            PlayerMove move = other.GetComponentInParent<PlayerMove>();
            if (move != null) move.SetGrip(1f);

            // Zeminin biraz üzerinden bırak ki çarpışma kusursuz algılansın
            other.transform.position = startPosition + Vector3.up * 0.5f;
        }
    }
}