using UnityEngine;

// Bayrağın mevcut trigger'ını bitiş yöneticisine bağlar; başka nesneler kazanamaz.
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public class MertcanFinishTrigger : MonoBehaviour
{
    [SerializeField] private MertcanLevelCompletion finishPlayer;
    private void OnTriggerEnter(Collider other)
    {
        TryFinish(other);
    }
    private void OnTriggerStay(Collider other)
    {
        TryFinish(other);
    }

    private void TryFinish(Collider other)
    {
        if (finishPlayer != null) finishPlayer.CompleteLevel(other);
    }
}
