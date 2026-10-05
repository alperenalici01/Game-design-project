using UnityEngine;

public class Coin_turn : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is create
    // Update is called once per frame
    [SerializeField] private float rotationSpeed = 1f;
    void Update()
    {
        transform.Rotate(0f,0f , rotationSpeed);
    }
}
