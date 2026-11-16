using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed = 50f;

    void Update()
    {
        // Change Vector3.right to Vector3.forward if it tilts the wrong way
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
    }
}