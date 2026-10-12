using UnityEngine;

public class PlayerCarry : MonoBehaviour
{
    [SerializeField] private Transform carryPoint;
    private Cargo carried; // null = hands empty
    public bool IsCarrying()
    {
        return carried != null;
    }
    // runs when the courier enters a trigger
    void OnTriggerEnter(Collider other)
    {
        Cargo cargo = other.GetComponent<Cargo>();
        if (cargo == null) return; // not cargo
        if (IsCarrying()) return; // hands full
        if (!cargo.CanBePickedUp()) return; // just dropped
        carried = cargo;
        cargo.PickUp(carryPoint);
    }

    public void DropCargo()
    {
        if (!IsCarrying()) return;
        carried.Drop();
        carried = null;
    }
    // the green zone calls this
    public Cargo HandOver()
    {
        Cargo cargo = carried;
        carried = null; // hands empty again
        return cargo;
    }
}
