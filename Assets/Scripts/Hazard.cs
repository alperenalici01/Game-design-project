using UnityEngine;

public class Hazard : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Who? the courier, carrying cargo
        PlayerCarry carry = other.GetComponent<PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;
        // Act: knock the cargo off
        carry.DropCargo();
        Debug.Log(name + " knocked the cargo off!");
    }
}
