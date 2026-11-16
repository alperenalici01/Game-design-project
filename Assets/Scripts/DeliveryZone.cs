using UnityEngine;

public class DeliveryZone : MonoBehaviour
{
    private int score;

    void OnTriggerEnter(Collider other)
    {
        PlayerCarry carry = other.GetComponent<PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        Cargo cargo = carry.HandOver();
        score += cargo.Points();
        Debug.Log("Delivered! +" + cargo.Points() + " Score: " + score);

        // BURASI ÇOK ÖNEMLİ: LevelManager'a 1 coin teslim edildiğini bildiriyoruz
        if (LevelManager_Zafer.Instance != null)
        {
            LevelManager_Zafer.Instance.AddCoin();
        }

        Destroy(cargo.gameObject);
    }
}