using UnityEngine;

// Havuzdaki tek blok: hareket gövdesini ve ait olduğu dalga yöneticisini tutar.
[DisallowMultipleComponent]
public class MertcanBlockPartyPiece : MonoBehaviour
{
    internal MertcanBlockParty Owner { get; set; }
    internal Rigidbody Body { get; set; }
    public bool IsHazard => GetComponent<MertcanHazard>() != null;
    public bool IsPushable => !IsHazard && Owner != null;
    public float VelocityX => IsPushable ? -Mathf.Max(0.1f, Owner.Speed) : 0f;
}
