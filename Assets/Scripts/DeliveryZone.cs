using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Who? the courier, carrying cargo
        PlayerCarry carry = other.GetComponent<PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;
        // Act: take the cargo and remove it
        Cargo cargo = carry.HandOver();
        Destroy(cargo.gameObject);
        Debug.Log("Delivered!");
    }
}
