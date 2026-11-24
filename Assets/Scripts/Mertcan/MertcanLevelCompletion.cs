using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Zafer veya Game Over durumunu bir kez işler; R ile aynı sahneyi tamamen yeniden yükler.
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public class MertcanLevelCompletion : MonoBehaviour
{
    [SerializeField] private MertcanRespawn player;
    [SerializeField] private ThirdPersonCamera followCamera;
    [SerializeField] private MertcanCoinCounter coins;
    [SerializeField] private MertcanVictoryScreen victoryScreen;
    [SerializeField] private MertcanGameOverScreen gameOverScreen;
    private Rigidbody body;
    private bool levelCompleted;
    private bool gameOver;
    private bool restartingLevel;
    private bool ownsVictoryPause;
    private const string MertcanScenePath = "Assets/Scenes/Mertcan.unity";
    public bool IsCompleted => levelCompleted;
    public bool IsGameOver => gameOver;
    public bool IsFinished => levelCompleted || gameOver;

    private bool CanEndLevel => !IsFinished && !restartingLevel && enabled && player != null &&
        player.enabled && body != null && gameObject.scene.path == MertcanScenePath;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public void CompleteLevel(Collider other)
    {
        if (!CanEndLevel || other == null || other.attachedRigidbody != body) return;
        if (victoryScreen == null || !victoryScreen.IsConfigured || coins == null)
        {
            Debug.LogError("Mertcan bitişinde sayaç veya zafer ekranı bağlantısı eksik.", this);
            return;
        }
        // Önce kilitle: aynı fizik adımındaki tehlike ve bitiş temasları tekrar işlem başlatamaz.
        levelCompleted = true;
        StopPlayerAndCamera();
        victoryScreen.Show(coins.CollectedCoins, coins.TotalCoins);
        PauseLevel();
    }

    public void GameOver()
    {
        if (!CanEndLevel) return;
        if (gameOverScreen == null || !gameOverScreen.IsConfigured || coins == null)
        {
            Debug.LogError("Mertcan Game Over ekranı veya coin sayacı bağlantısı eksik.", this);
            return;
        }
        gameOver = true;
        StopPlayerAndCamera();
        gameOverScreen.Show(coins.CollectedCoins, coins.TotalCoins);
        PauseLevel();
    }

    private void StopPlayerAndCamera()
    {
        player.StopForCompletion();
        if (followCamera != null) followCamera.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void PauseLevel()
    {
        ownsVictoryPause = true;
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (!IsFinished) return;
        if (gameOver) gameOverScreen.FitPanel();
        else victoryScreen.FitPanel();
        Keyboard keyboard = Keyboard.current;
        if (!restartingLevel && keyboard != null && keyboard.rKey.wasPressedThisFrame)
            RestartLevel();
    }

    private void RestartLevel()
    {
        if (!IsFinished || restartingLevel || gameObject.scene.path != MertcanScenePath) return;
        restartingLevel = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
        ownsVictoryPause = false;
        try
        {
            if (Application.CanStreamedLevelBeLoaded(MertcanScenePath))
            {
                SceneManager.LoadScene(MertcanScenePath, LoadSceneMode.Single);
                return;
            }
#if UNITY_EDITOR
            // Build Settings değiştirilmez: Editor'da aynı sahne dosyasını doğrudan aç.
            UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(MertcanScenePath,
                new LoadSceneParameters(LoadSceneMode.Single));
#else
            throw new System.InvalidOperationException("Mertcan scene is unavailable in this Player build.");
#endif
        }
        catch (System.Exception error)
        {
            restartingLevel = false;
            ownsVictoryPause = true;
            Time.timeScale = 0f;
            Debug.LogException(error, this);
        }
    }

    private void OnDisable()
    {
        if (!ownsVictoryPause) return;
        Time.timeScale = 1f;
        ownsVictoryPause = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
