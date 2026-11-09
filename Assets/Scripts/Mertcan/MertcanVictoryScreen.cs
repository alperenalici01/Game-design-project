using UnityEngine;

// Yalnızca mevcut zafer arayüzünü hazırlar ve gösterir; oyun durumunu değiştirmez.
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public class MertcanVictoryScreen : MonoBehaviour
{
    [SerializeField] private GameObject victoryOverlay;
    [SerializeField] private RectTransform victoryPanel;
    [SerializeField] private UnityEngine.UI.Text victoryResult;
    [SerializeField] private UnityEngine.UI.Text[] victoryLabels;
    [SerializeField] private RectTransform victoryCoinIcon;
    [SerializeField] private GameObject coinHudRoot;

    public bool IsConfigured => victoryOverlay != null && victoryPanel != null && victoryResult != null;

    private void Start()
    {
        if (victoryOverlay == null) return;
        victoryOverlay.SetActive(false);
        Font uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (victoryLabels != null)
            foreach (UnityEngine.UI.Text label in victoryLabels)
                if (label != null) label.font = uiFont;
        if (victoryCoinIcon != null && victoryCoinIcon.GetComponent<MertcanCoinIcon>() == null)
            victoryCoinIcon.gameObject.AddComponent<MertcanCoinIcon>().raycastTarget = false;
    }

    public void Show(int collectedCoins, int totalCoins)
    {
        if (coinHudRoot != null) coinHudRoot.SetActive(false);
        victoryResult.text = collectedCoins + " / " + totalCoins;
        victoryOverlay.SetActive(true);
        Canvas.ForceUpdateCanvases();
        FitPanel();
    }

    public void FitPanel()
    {
        if (victoryPanel == null || victoryOverlay == null) return;
        RectTransform screen = victoryOverlay.GetComponent<RectTransform>();
        if (screen == null) return;
        victoryPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
            Mathf.Min(680f, Mathf.Max(220f, screen.rect.width - 48f)));
        victoryPanel.localScale = Vector3.one * Mathf.Min(1f,
            Mathf.Max(0.1f, (screen.rect.height - 48f) / 340f));
    }
}
