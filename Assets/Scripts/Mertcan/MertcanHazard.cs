using UnityEngine;

// Tehlikenin hareketini değil, oyuncuya temas ettiğinde respawn istemesini yönetir.
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public class MertcanHazard : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        NotifyPlayer(collision.collider);
    }
    private void OnCollisionStay(Collision collision)
    {
        NotifyPlayer(collision.collider);
    }
    private void OnTriggerEnter(Collider other)
    {
        NotifyPlayer(other);
    }
    private void OnTriggerStay(Collider other)
    {
        NotifyPlayer(other);
    }

    public void NotifyPlayer(Collider other)
    {
        Rigidbody otherBody = other.attachedRigidbody;
        if (otherBody == null) return;
        MertcanRespawn player = otherBody.GetComponent<MertcanRespawn>();
        if (player != null) player.RequestRespawn();
    }
}
