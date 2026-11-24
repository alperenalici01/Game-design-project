using UnityEngine;
using TMPro; // TextMeshPro kullanmak için gerekli

public class GameTimer : MonoBehaviour
{
    [Header("Ayarlar")]
    [SerializeField] private float baslangicSuresi = 60f; // Saniye cinsinden
    [SerializeField] private bool geriSayimMi = true;    // İşaretliyse geri, değilse ileri sayar
    [SerializeField] private bool otomatikBaslat = true;

    [Header("UI Bağlantısı")]
    [SerializeField] private TextMeshProUGUI timerText;

    private float mevcutSure;
    private bool calisiyorMu;

    private void Start()
    {
        Sifirla();
        if (otomatikBaslat)
        {
            Baslat();
        }
    }

    private void Update()
    {
        if (!calisiyorMu) return;

        if (geriSayimMi)
        {
            mevcutSure -= Time.deltaTime;

            if (mevcutSure <= 0f)
            {
                mevcutSure = 0f;
                calisiyorMu = false;
                SureBitti();
            }
        }
        else
        {
            mevcutSure += Time.deltaTime;
        }

        EkraniGuncelle(mevcutSure);
    }

    private void EkraniGuncelle(float gosterilecekSure)
    {
        if (timerText == null) return;

        int dakika = Mathf.FloorToInt(gosterilecekSure / 60f);
        int saniye = Mathf.FloorToInt(gosterilecekSure % 60f);

        // Dakika:Saniye formatında (Örn: 01:05) ekrana yazar
        timerText.text = string.Format("{0:00}:{1:00}", dakika, saniye);
    }

    private void SureBitti()
    {
        Debug.Log("Süre doldu!");
        // Süre bittiğinde yapılacak işlemleri (Game Over ekranı vb.) buraya yazabilirsiniz.
    }

    // Dışarıdan erişim için kontrol metodları
    public void Baslat() => calisiyorMu = true;
    public void Durdur() => calisiyorMu = false;

    public void Sifirla()
    {
        mevcutSure = geriSayimMi ? baslangicSuresi : 0f;
        EkraniGuncelle(mevcutSure);
    }
}
 
