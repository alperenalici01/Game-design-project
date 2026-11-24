using UnityEngine;

// Tehlikede açılan sonuç paneli. Oyun durumunu ve yeniden başlatmayı seviye yöneticisi yönetir.
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public class MertcanGameOverScreen : MonoBehaviour
{
    [SerializeField] private GameObject gameOverOverlay;
    [SerializeField] private RectTransform gameOverPanel;
    [SerializeField] private UnityEngine.UI.Text resultText;
    [SerializeField] private UnityEngine.UI.Text[] labels;
    [SerializeField] private RectTransform coinIcon;
    [SerializeField] private GameObject coinHudRoot;

    public bool IsConfigured => gameOverOverlay != null && gameOverPanel != null && resultText != null;

    private void Start()
    {
        if (gameOverOverlay == null) return;
        gameOverOverlay.SetActive(false);
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (labels != null)
            foreach (UnityEngine.UI.Text label in labels)
                if (label != null) label.font = font;
        if (coinIcon != null && coinIcon.GetComponent<MertcanCoinIcon>() == null)
            coinIcon.gameObject.AddComponent<MertcanCoinIcon>().raycastTarget = false;
    }

    public void Show(int collectedCoins, int totalCoins)
    {
        if (coinHudRoot != null) coinHudRoot.SetActive(false);
        resultText.text = collectedCoins + " / " + totalCoins;
        gameOverOverlay.SetActive(true);
        Canvas.ForceUpdateCanvases();
        FitPanel();
    }

    public void FitPanel()
    {
        if (gameOverPanel == null || gameOverOverlay == null) return;
        RectTransform screen = gameOverOverlay.GetComponent<RectTransform>();
        if (screen == null) return;
        gameOverPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
            Mathf.Min(680f, Mathf.Max(220f, screen.rect.width - 48f)));
        gameOverPanel.localScale = Vector3.one * Mathf.Min(1f,
            Mathf.Max(0.1f, (screen.rect.height - 48f) / 340f));
    }
}
