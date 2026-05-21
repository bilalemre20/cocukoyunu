using UnityEngine;
using UnityEngine.EventSystems; // Sürükleme kütüphanesi
using UnityEngine.UI;

public class ShapeDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Hedef Ayarlarý")]
    public RectTransform correctSlot; // Bu parça hangi gölgeye gitmeli?
    public float snapDistance = 100f; // Ne kadar yaklaþýrsa yapýþsýn?

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 startPosition; // Yanlýþsa döneceði yer
    private bool isLocked = false; // Doðru yerleþtiyse bir daha oynamasýn

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Eðer CanvasGroup yoksa otomatik ekle (Raycast bloklamak için lazým)
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    // Týklayýp sürüklemeye baþladýðýn an
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isLocked) return; // Kilitliyse kýpýrdama

        startPosition = rectTransform.anchoredPosition; // Eski yerini hafýzaya al
        canvasGroup.blocksRaycasts = false; // Arkadaki nesneleri görebilmek için ýþýný kapat
        canvasGroup.alpha = 0.6f; // Sürüklerken biraz þeffaf olsun
    }

    // Sürükleme devam ederken (Mouse hareket ettikçe)
    public void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        rectTransform.anchoredPosition += eventData.delta / transform.root.GetComponent<Canvas>().scaleFactor;
    }

    // Parmaðýný/Mouse'u býraktýðýn an
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isLocked) return;

        canvasGroup.blocksRaycasts = true; // Iþýný tekrar aç
        canvasGroup.alpha = 1f; // Opak yap

        // Hedefe ne kadar uzaðýz? Ölçelim.
        float distance = Vector2.Distance(rectTransform.anchoredPosition, correctSlot.anchoredPosition);

        // Eðer mesafe yeterince azsa (Yaklaþtýysak)
        if (distance <= snapDistance)
        {
            // TAM ÝSABET!
            rectTransform.anchoredPosition = correctSlot.anchoredPosition; // Tam üstüne oturt
            isLocked = true; // Artýk kilitlendi, hareket ettirilemez

            // Buraya "Doðru ses efekti" veya "Yýldýz çýkma efekti" ekleyebilirsin.
            Debug.Log("Doðru Eþleþme!");
        }
        else
        {
            // YANLIÞ YER!
            rectTransform.anchoredPosition = startPosition; // Baþlangýç noktasýna geri dön
            Debug.Log("Yanlýþ, geri dönüyor...");
        }
    }
}