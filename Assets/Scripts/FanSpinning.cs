using UnityEngine;

public class FanSpinnign : MonoBehaviour
{
    void Update()
    {
        // Z ekseninde (üçüncü değer) döndürür
        transform.Rotate(0f, 0f, 1f);
    }
}