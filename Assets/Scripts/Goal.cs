using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool loopToFirstScene = false;  // after the last scene, go back to the first one

    private bool triggered;

    // Detect: something walks into the goal
    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        // Who? only the player
        if (!IsPlayer(other)) return;

        // Act: load the next scene in Build Settings order
        triggered = true;
        LoadNextScene();
    }

    bool IsPlayer(Collider other)
    {
        if (other.CompareTag(playerTag)) return true;
        return other.GetComponentInParent<PlayerMove>() != null
            || other.GetComponentInParent<PlayerMove_Mertcan>() != null
            || other.GetComponentInParent<PlayerCarry>() != null
            || other.GetComponentInParent<SimpleCourierController>() != null;
    }

    void LoadNextScene()
    {
        int next = SceneManager.GetActiveScene().buildIndex + 1;

        if (next >= SceneManager.sceneCountInBuildSettings)
        {
            if (!loopToFirstScene)
            {
                Debug.Log("Last scene reached, no more levels.");
                triggered = false;
                return;
            }
            next = 0;
        }

        Time.timeScale = 1f;   // a frozen game would freeze the next scene too
        SceneManager.LoadScene(next);
    }
}
