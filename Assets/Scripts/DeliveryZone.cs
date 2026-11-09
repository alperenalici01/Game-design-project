using UnityEngine;
public class DeliveryZone : MonoBehaviour
{
    private int score; // points delivered so far
    void OnTriggerEnter(Collider other)
    {
        // Who? the courier, carrying cargo
        PlayerCarry carry = other.GetComponent<PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;
        // Act: take the cargo, add its points, remove it
        Cargo cargo = carry.HandOver();
        score += cargo.Points();
        Debug.Log("Delivered! +" + cargo.Points() + " Score: " + score);
        Destroy(cargo.gameObject);
    }
}