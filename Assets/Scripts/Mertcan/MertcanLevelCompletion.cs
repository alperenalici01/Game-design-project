using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Kazanmayı bir kez işler; R ile aynı sahneyi tamamen yeniden yükler.
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public class MertcanLevelCompletion : MonoBehaviour
{
    [SerializeField] private MertcanRespawn player;
    [SerializeField] private ThirdPersonCamera followCamera;
    [SerializeField] private MertcanCoinCounter coins;
    [SerializeField] private MertcanVictoryScreen victoryScreen;
    private Rigidbody body;
    private bool levelCompleted;
    private bool restartingLevel;
    private bool ownsVictoryPause;
    private const string MertcanScenePath = "Assets/Scenes/Mertcan.unity";
    public bool IsCompleted => levelCompleted;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public void CompleteLevel(Collider other)
    {
        if (levelCompleted || restartingLevel || !enabled || player == null || !player.enabled ||
            body == null || other == null || other.attachedRigidbody != body ||
            gameObject.scene.path != MertcanScenePath) return;
        if (victoryScreen == null || !victoryScreen.IsConfigured || coins == null)
        {
            Debug.LogError("Mertcan bitişinde sayaç veya zafer ekranı bağlantısı eksik.", this);
            return;
        }
        // Önce kilitle: aynı fizik adımındaki tehlike ve bitiş temasları tekrar işlem başlatamaz.
        levelCompleted = true;
        player.StopForCompletion();
        if (followCamera != null) followCamera.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        victoryScreen.Show(coins.CollectedCoins, coins.TotalCoins);
        ownsVictoryPause = true;
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (!levelCompleted) return;
        victoryScreen.FitPanel();
        Keyboard keyboard = Keyboard.current;
        if (!restartingLevel && keyboard != null && keyboard.rKey.wasPressedThisFrame)
            RestartCompletedLevel();
    }

    private void RestartCompletedLevel()
    {
        if (!levelCompleted || restartingLevel || gameObject.scene.path != MertcanScenePath) return;
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
