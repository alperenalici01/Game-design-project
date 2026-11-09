using UnityEngine;

public class PlayerJumpMete : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Zıplama Ayarları")]
    public float jumpForce = 7f; // Kuvveti biraz artırdık
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Karakterin merkezinden alta doğru kısa bir ışın gönderip yere basıp basmadığını kontrol ediyoruz
        // 1.1f değeri karakterin boyuna göre değişebilir, karakteriniz çok küçükse 0.6f yapabilirsiniz
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.2f);

        // Boşluk tuşuna basıldığında ve yerdeysek
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}