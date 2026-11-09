using UnityEngine;

// Oyuncunun fizik kökünü tanır ve aynı coinin yalnızca bir kez sayılmasını sağlar.
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public class MertcanCoin : MonoBehaviour
{
    [SerializeField] private MertcanCoinCounter coinOwner;
    private bool collected;
    private bool registeredCoin;
    public MertcanCoinCounter Owner => coinOwner;

    public void Register()
    {
        registeredCoin = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        Collect(other);
    }
    private void OnTriggerStay(Collider other)
    {
        Collect(other);
    }

    private void Collect(Collider other)
    {
        if (collected || coinOwner == null || !coinOwner.enabled || coinOwner.IsCompleted ||
            other.attachedRigidbody == null || other.attachedRigidbody != coinOwner.PlayerBody ||
            !registeredCoin) return;
        collected = true;
        coinOwner.AddCollectedCoin();
        gameObject.SetActive(false);
    }
}
