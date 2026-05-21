using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CategoryDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Bu Nesnenin Kategorisi")]
    public string myCategory; // Örn: "daire" (Slot ile harfi harfine AYNI olmalý)

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;    // Yanlýþsa döneceði pozisyon
    private Transform originalParent; // Yanlýþsa döneceði "Pool_Slot"

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Eðer CanvasGroup eklemeyi unuttuysan kod otomatik ekler
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 1. Mevcut konumunu ve babasýný (Pool_Slot'u) hafýzaya al
        startPosition = transform.position;
        originalParent = transform.parent;

        // 2. Sürüklerken diðer nesnelerin üstünde görünsün diye en dýþa (Root'a) al
        transform.SetParent(transform.root);

        // 3. Iþýnlarý kapat ki alttaki slotu (kutuyu) görebilelim
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Mouse neredeyse resim oraya gitsin
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // Iþýnlarý tekrar aç
        bool placed = false;

        // Býraktýðýmýz noktada ne var?
        GameObject targetObj = eventData.pointerEnter;

        if (targetObj != null)
        {
            // Býraktýðýmýz þeyin üzerinde "CategorySlot" scripti var mý?
            CategorySlot slot = targetObj.GetComponent<CategorySlot>();

            if (slot != null)
            {
                // 1. Kategori eþleþiyor mu? (Örn: daire == daire)
                // 2. Kutu boþ mu?
                if (slot.categoryID == myCategory && !slot.isOccupied)
                {
                    // --- DOÐRU YERLEÞTÝRME VE SIÐDIRMA KISMI ---

                    // 1. Slotun içine gir
                    transform.SetParent(slot.transform);

                    // 2. Pivotu ve Çapalarý (Anchors) tam merkeze al
                    // Bu iþlem resmin kaymasýný engeller
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

                    // 3. Pozisyonu tam merkeze (0,0) çek
                    rectTransform.anchoredPosition = Vector2.zero;

                    // 4. --- BOYUTU KUTUYA EÞÝTLEME ---
                    // Slotun (Kutunun) boyutunu öðreniyoruz
                    RectTransform slotRect = slot.GetComponent<RectTransform>();
                    if (slotRect != null)
                    {
                        // Nesnenin geniþliðini ve yüksekliðini kutuyla ayný yapýyoruz
                        rectTransform.sizeDelta = slotRect.sizeDelta;
                    }
                    // -------------------------------------------

                    slot.isOccupied = true; // Kutuyu dolu iþaretle
                    placed = true;

                    // Artýk hareket etmesin, iþlem bitti
                    this.enabled = false;

                    Debug.Log("Doðru Yerleþti ve Sýðdýrýldý!");
                    // Buraya ses efekti kodu ekleyebilirsin
                }
            }
        }

        // Eðer boþluða býraktýysak veya yanlýþ kutuya koyduysak
        if (!placed)
        {
            // Eski yerine (Pool_Slot içine) geri dön
            transform.SetParent(originalParent);
            transform.position = startPosition;
        }
    }
}