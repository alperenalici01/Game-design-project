using UnityEngine;

public class Capsule_Movement : MonoBehaviour
{
    [SerializeField] private float capsuleHeight = 1f;
    [SerializeField] private float capsuleSpeed = 1.5f;

    private Vector3 startLocalPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startLocalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float offset =Mathf.Sin(Time.time * capsuleSpeed) * capsuleHeight;
        transform.localPosition = startLocalPosition + Vector3.up * offset;
    }
}
