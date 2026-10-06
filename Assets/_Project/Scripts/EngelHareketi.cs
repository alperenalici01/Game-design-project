using UnityEngine;

public class EngelHareketi : MonoBehaviour
{
    [SerializeField] private float engelHeight = 8f; // metres up and down
    [SerializeField] private float engelSpeed = 3f; // how fast it floats
    private Vector3 startLocalPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startLocalPosition = transform.localPosition;
        
    }

    // Update is called once per frame
    void Update()
    {
        float offset = Mathf.Sin(Time.time * engelSpeed) * engelHeight;    // between -0.25 and +0.25
        transform.localPosition = startLocalPosition + Vector3.down * offset;
    }
}
