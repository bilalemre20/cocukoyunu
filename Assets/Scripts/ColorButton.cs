using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
    public Color myColor = Color.red; // Inspector'dan her buton için ayrý renk seçeceksin

    void Start()
    {
        // Butonun kendi rengini ayarla ki hangi renk olduðu belli olsun
        GetComponent<Image>().color = myColor;

        // Týklanýnca çalýþacak fonksiyonu ekle
        GetComponent<Button>().onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        // Ana yöneticiye "Rengi deðiþtir" emri gönder
        if (PaintingManager.Instance != null)
        {
            PaintingManager.Instance.SetColor(myColor);
        }
    }
}