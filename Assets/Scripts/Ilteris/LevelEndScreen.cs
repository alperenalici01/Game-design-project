using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelEndScreen : MonoBehaviour
{
    // the big text in the middle of the panel
    [SerializeField] private TMP_Text resultText;

    // our own key: teammates' best scores don't mix with ours
    private const string BestKey = "best_Ilteris";

    // the LevelManager calls this when the round ends
    public void Show(string message, int coins, int totalCoins, int score)
    {
        // the best score is kept on this computer
        int best = PlayerPrefs.GetInt(BestKey, 0);
        if (score > best) best = score;
        PlayerPrefs.SetInt(BestKey, best);

        resultText.text = message
            + "\nCoins " + coins + " / " + totalCoins
            + "\nScore " + score
            + "\nBest " + best;
        gameObject.SetActive(true);   // the panel appears
    }

    // the Play again button calls this
    public void PlayAgain()
    {
        // the LevelManager stopped time: start it again first
        Time.timeScale = 1f;
        // load the scene we are in once more: a fresh round
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
