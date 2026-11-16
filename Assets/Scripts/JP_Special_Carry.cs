using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class JP_Special_Carry : MonoBehaviour
{
    [SerializeField] private Transform carryPoint;
    [SerializeField] private CargoType stackableType;   
    [SerializeField] private int maxStack = 5;          
    [SerializeField] private float stackSpacing = 0.8f;

    private PlayerCarry carry;
    private Cargo lead;                                 
    private List<Cargo> extras = new List<Cargo>();      
    private FieldInfo typeField;                        
    void Awake()
    {
        carry = GetComponent<PlayerCarry>();
    }

    private CargoType TypeOf(Cargo c)
    {
        if (typeField == null)
        {
            foreach (FieldInfo f in typeof(Cargo).GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (f.FieldType == typeof(CargoType)) { typeField = f; break; }
            }
            if (typeField == null)
            {
                Debug.LogWarning("Cargo içinde CargoType alanı bulunamadı");
                return null;
            }
        }
        return typeField.GetValue(c) as CargoType;
    }

    private bool IsCoin(Cargo c)
    {
        return stackableType != null && TypeOf(c) == stackableType;
    }

    void OnTriggerEnter(Collider other)
    {
        if (carry == null) return;

        Cargo cargo = other.GetComponent<Cargo>();
        if (cargo == null) return;
        if (!IsCoin(cargo)) return;                       
        if (!cargo.CanBePickedUp()) return;
        if (other.transform.IsChildOf(transform)) return; 
        if (!carry.IsCarrying()) return;                  

        
        if (lead == null)
        {
            foreach (Cargo c in carryPoint.GetComponentsInChildren<Cargo>())
            {
                if (!extras.Contains(c)) { lead = c; break; }
            }
        }
        if (lead == null || !IsCoin(lead)) return;        
        if (extras.Count + 1 >= maxStack) return;

        cargo.PickUp(carryPoint);
        cargo.transform.localPosition = Vector3.up * ((extras.Count + 1) * stackSpacing);
        extras.Add(cargo);

    }
    
    public float StackedMass(float baseMass)
    {
        if (stackableType == null || extras.Count == 0) return -1f;

        float w = stackableType.Weight();
        float total = 0f;
        for (int i = 0; i <= extras.Count; i++)
        {
            total += w * Mathf.Pow(0.5f, i); 
        }
        return baseMass + total;
    }

    void Update()
    {
        if (extras.Count == 0 || carry.IsCarrying()) return;


        foreach (Cargo c in extras)
        {
            if (c == null) continue;
            if (lead == null) Destroy(c.gameObject);      
            else c.Drop();                                
        }
        extras.Clear();
        lead = null;
    }
}