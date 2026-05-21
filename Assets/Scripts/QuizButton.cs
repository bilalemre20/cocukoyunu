using UnityEngine;
using UnityEngine.UI;

public class QuizButton : MonoBehaviour
{
    [Header("Ayarlar")]
    public bool isCorrectAnswer = false; // Bu buton doðru cevap mý?
    public Button myButton;
    public Image buttonImage;

    [Header("Renkler")]
    public Color correctColor = Color.green; // Doðruysa yeþil olsun
    public Color wrongColor = Color.red;     // Yanlýþsa kýrmýzý olsun

    void Start()
    {
        // Butonun týklanma olayýný dinle
        if (myButton != null)
            myButton.onClick.AddListener(CheckAnswer);
    }

    void CheckAnswer()
    {
        if (isCorrectAnswer)
        {
            Debug.Log("DOÐRU!");
            buttonImage.color = correctColor; // Yeþil yap

            // --- YENÝ EKLENEN KISIM: TÜM BUTONLARI KÝLÝTLE ---
            // Bu butonun "Babasýna" (Soru Kutusuna) git ve içindeki tüm butonlarý bul
            Button[] allButtonsInThisQuestion = transform.parent.GetComponentsInChildren<Button>();

            // Hepsini tek tek gez ve kapat
            foreach (Button btn in allButtonsInThisQuestion)
            {
                btn.interactable = false; // Artýk týklanamazlar
            }
            // --------------------------------------------------

            // Ýstersen burada level bitiþ kontrolü veya alkýþ sesi ekleyebilirsin.
        }
        else
        {
            Debug.Log("YANLIÞ!");
            buttonImage.color = wrongColor; // Kýrmýzý yap

            // Yanlýþsa 1 saniye sonra rengi geri düzeltebiliriz
            Invoke("ResetColor", 1f);
        }
    }

    void ResetColor()
    {
        // Eðer buton hala týklanabilir durumdaysa rengini düzelt
        // (Doðru cevap bulunduktan sonra yanlýþlar kýrmýzý kalmasýn diye kontrol ediyoruz)
        if (myButton.interactable)
        {
            buttonImage.color = Color.white;
        }
    }
}