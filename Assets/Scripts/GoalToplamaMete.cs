using UnityEngine;

public class GoalToplamaMete : MonoBehaviour
{
    [Header("Ödül Ayarları")]
    public int pointValue = 10; // Bu eşya kaç puan verecek?

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // GameManager'ı bul ve puanı ekle
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(pointValue);
            }

            // Eşyayı sahneden yok et
            Destroy(gameObject);
        }
    }
}