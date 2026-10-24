using UnityEngine;

public class HoverBob : MonoBehaviour
{
    [SerializeField] private float bobHeight = 0.4f;  // metres up and down
    [SerializeField] private float bobSpeed = 3f;     // how fast it bobs

    private Vector3 startLocalPosition;

    void Start()
    {
        // remember where we sit inside our parent
        startLocalPosition = transform.localPosition;
    }

    void Update()
    {
        // up and down around the start, relative to the parent,
        // so it keeps bobbing while the parent patrols
        float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = startLocalPosition + Vector3.up * offset;
    }
}
