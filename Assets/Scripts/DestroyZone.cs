using UnityEngine;

public class Destroy_Zone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {

        PlayerCarry carry = other.GetComponent<PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        Cargo[] all = other.GetComponentsInChildren<Cargo>();

        if (all.Length <= 1)
        {
            Cargo only = carry.HandOver();
            if (only != null) Destroy(only.gameObject);
            Debug.Log("1 coin destroyed");
            return;
        }

    
        Cargo top = all[0];
        foreach (Cargo c in all)
        {
            if (c.transform.localPosition.y > top.transform.localPosition.y)
                top = c;
        }
        Destroy(top.gameObject);
        Debug.Log("1 coin destroyed");
    }
}