using UnityEngine;

// Başlangıçta kayıtlı coinleri sayar; respawn bu toplamı veya toplananları sıfırlamaz.
[DefaultExecutionOrder(-100)]
[DisallowMultipleComponent]
public class MertcanCoinCounter : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Text coinCounter;
    [SerializeField] private RectTransform coinIcon;
    private Rigidbody playerBody;
    private MertcanLevelCompletion completion;
    public int TotalCoins { get; private set; }
    public int CollectedCoins { get; private set; }
    public Rigidbody PlayerBody => playerBody;
    public bool IsCompleted => completion != null && completion.IsCompleted;

    private void Awake()
    {
        playerBody = GetComponent<Rigidbody>();
        completion = GetComponent<MertcanLevelCompletion>();
    }

    private void Start()
    {
        foreach (MertcanCoin coin in FindObjectsByType<MertcanCoin>(FindObjectsInactive.Include))
        {
            if (coin.Owner != this || coin.gameObject.scene != gameObject.scene) continue;
            coin.Register();
            TotalCoins++;
        }
        if (coinCounter != null)
            coinCounter.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (coinIcon != null && coinIcon.GetComponent<MertcanCoinIcon>() == null)
            coinIcon.gameObject.AddComponent<MertcanCoinIcon>().raycastTarget = false;
        Refresh();
    }

    public void AddCollectedCoin()
    {
        CollectedCoins++;
        Refresh();
    }

    private void Refresh()
    {
        if (coinCounter != null) coinCounter.text = CollectedCoins + " / " + TotalCoins;
    }
}
