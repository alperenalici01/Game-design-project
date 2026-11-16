using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // Sahne geçişleri için bu şart!

public class LevelManager_Zafer : MonoBehaviour
{
    public static LevelManager_Zafer Instance;

    [SerializeField] private Rigidbody player;
    [SerializeField] private Transform coinsParent;
    [SerializeField] private int fontSize = 36;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Cargo[] allCargo;
    private int coins;
    private bool won;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    void Start()
    {
        startPosition = player.position;
        startRotation = player.rotation;
        allCargo = coinsParent.GetComponentsInChildren<Cargo>(true);

        // Şunu ekleyin: Konsola toplam kaç coin bulduğunu yazdırsın
        Debug.Log("Algılanan Toplam Coin Sayısı: " + allCargo.Length);
    }

    void Update()
    {
        // İstrsen R tuşu ile hala bölümü sıfırlayabilirsin
        if (won && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            won = false;
            Time.timeScale = 1f;
            ResetLevel();
        }
    }

    // Bir coin hedefe getirildiğinde burası çağrılacak
    public void AddCoin()
    {
        if (won) return;

        coins++;
        Debug.Log("Toplanan Coin: " + coins + " / " + allCargo.Length);

        // İŞTE BURASI: Eğer toplanan coin sayısı toplam coin sayısına ulaştıysa veya geçtiyse kazan!
        if (coins >= allCargo.Length)
        {
            Win();
        }
    }

    // Spikes, düşme bölgesi vb. çağırır
    public void Restart()
    {
        if (won) return;
        ResetLevel();
    }

    // Tüm coinler toplandığında burası tetiklenir
    public void Win()
    {
        if (won) return;
        won = true;

        // Ekranda "Victory" yazısını 1.5 saniye gösterip sonraki level'a geçelim:
        Invoke("LoadNextLevel", 1.5f);
    }

    void LoadNextLevel()
    {
        Time.timeScale = 1f; // Zamanı normale al
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Eğer sırada başka bir sahne varsa onu aç, yoksa ilk sahneye (veya ana menüye) dön
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Oyun bitti! Son leveldaydınız.");
            SceneManager.LoadScene(0); // Örn: Ana menü veya 0. sahne
        }
    }

    void ResetLevel()
    {
        player.linearVelocity = Vector3.zero;
        player.angularVelocity = Vector3.zero;
        player.position = startPosition;
        player.rotation = startRotation;
        player.transform.SetPositionAndRotation(startPosition, startRotation);

        coins = 0;
        foreach (Cargo cargo in allCargo)
        {
            cargo.gameObject.SetActive(true);
        }
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = fontSize;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.yellow;
        GUI.Label(new Rect(20, 20, 600, 80), "Coins: " + coins + " / " + allCargo.Length, style);

        if (!won) return;
        style.fontSize = fontSize * 2;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.green;
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height),
                  "VICTORY!\nAll coins collected!\nLoading next level...", style);
    }
}