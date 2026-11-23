using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    // the one LevelManager in the scene: any script can reach it
    public static LevelManager Instance;

    [Header("Level")]
    [SerializeField] private Rigidbody player;        // drag the Player here
    [SerializeField] private Transform coinsParent;   // drag Level > Coins here
    [SerializeField] private Transform enemiesParent; // drag Level > Enemies here

    [Header("Round")]
    [SerializeField] private float roundLength = 120f; // seconds to reach the flag
    [SerializeField] private int pointsPerCoin = 10;   // a coin is worth 10 points

    [Header("HUD")]
    [SerializeField] private TMP_Text coinText;       // top-left
    [SerializeField] private TMP_Text timeText;       // top-centre
    [SerializeField] private LevelEndScreen endScreen; // the panel on the Canvas

    private Vector3 startPosition;
    private Quaternion startRotation;
    private CoinPickup[] allCoins;
    private StompableEnemy[] allEnemies = new StompableEnemy[0];
    private int coins;
    private float timeLeft;
    private bool over;   // true once the round has ended

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;   // a fresh round always starts with time running
    }

    void Start()
    {
        // remember where the player starts, which coins and enemies exist
        startPosition = player.position;
        startRotation = player.rotation;
        allCoins = coinsParent.GetComponentsInChildren<CoinPickup>(true);
        if (enemiesParent != null)
        {
            allEnemies = enemiesParent.GetComponentsInChildren<StompableEnemy>(true);
        }
        timeLeft = roundLength;   // a full clock
    }

    void Update()
    {
        if (over) return;   // ended: nothing changes any more

        // count down, but never below zero
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0f) timeLeft = 0f;
        ShowHud();
        if (timeLeft <= 0f) EndRound("Time's up!", false);
    }

    // a coin calls this
    public void AddCoin()
    {
        if (over) return;
        coins++;
    }

    // spikes, bees, the fall zone and enemies call this
    public void Restart()
    {
        if (over) return;   // no dying after the end
        ResetLevel();
    }

    // the flag calls this
    public void Win()
    {
        EndRound("You made it!", true);
    }

    void EndRound(string message, bool reachedFlag)
    {
        if (over) return;   // don't end it twice
        over = true;
        Time.timeScale = 0f;   // stop the world

        // coins give points; reaching the flag adds the seconds still left
        int score = coins * pointsPerCoin;
        if (reachedFlag) score += Mathf.CeilToInt(timeLeft);

        endScreen.Show(message, coins, allCoins.Length, score);
    }

    void ResetLevel()
    {
        // back to the start, standing still
        player.linearVelocity = Vector3.zero;
        player.angularVelocity = Vector3.zero;
        player.position = startPosition;
        player.rotation = startRotation;
        player.transform.SetPositionAndRotation(startPosition, startRotation);

        // every coin and every stomped enemy comes back
        coins = 0;
        foreach (CoinPickup coin in allCoins) coin.gameObject.SetActive(true);
        foreach (StompableEnemy enemy in allEnemies) enemy.gameObject.SetActive(true);

        // a fresh clock too
        timeLeft = roundLength;
    }

    // write both numbers on the screen
    void ShowHud()
    {
        coinText.text = "Coins " + coins + " / " + allCoins.Length;
        int seconds = Mathf.CeilToInt(timeLeft);
        int minutes = seconds / 60;
        timeText.text = minutes + ":" + (seconds % 60).ToString("00");
    }
}
