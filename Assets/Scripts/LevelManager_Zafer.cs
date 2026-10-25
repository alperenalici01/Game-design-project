using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager_Zafer : MonoBehaviour
{
    // the one LevelManager in the scene: any script can reach it
    public static LevelManager_Zafer Instance;

    [SerializeField] private Rigidbody player;       // drag the Player here
    [SerializeField] private Transform coinsParent;  // drag Level > Coins here
    [SerializeField] private int fontSize = 36;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Cargo[] allCargo;
    private int coins;
    private bool won;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;   // time runs normally
    }

    void Start()
    {
        // remember where the player starts and which cargo items exist
        startPosition = player.position;
        startRotation = player.rotation;
        allCargo = coinsParent.GetComponentsInChildren<Cargo>(true);
    }

    void Update()
    {
        // after the victory: R plays the level again
        if (won && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            won = false;
            Time.timeScale = 1f;
            ResetLevel();
        }
    }

    // a coin/cargo calls this
    public void AddCoin()
    {
        coins++;
    }

    // spikes, the turtle and the fall zone call this
    public void Restart()
    {
        if (won) return;   // no dying after the victory
        ResetLevel();
    }

    // the exit door calls this
    public void Win()
    {
        if (won) return;
        won = true;
        Time.timeScale = 0f;   // freeze the game
    }

    void ResetLevel()
    {
        // back to the start, standing still
        player.linearVelocity = Vector3.zero;
        player.angularVelocity = Vector3.zero;
        player.position = startPosition;
        player.rotation = startRotation;
        player.transform.SetPositionAndRotation(startPosition, startRotation);

        // every cargo comes back, the counter starts again
        coins = 0;
        foreach (Cargo cargo in allCargo)
        {
            cargo.gameObject.SetActive(true);
        }
    }

    // draws the text on the screen
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
                  "VICTORY!\n" + coins + " / " + allCargo.Length + " coins\n\nPress R to play again", style);
    }
}