using UnityEngine;
using UnityEngine.EventSystems; // UI olaylarý için þart

public class DotPoint : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Kimlik Ayarlarý")]
    public string dotID;     // Örn: "karpuz", "ucgen"
    public bool isTopRow;    // Üst sýra mý? (Kendi sýrasýyla eþleþmesin diye)

    private LineManager lineManager;

    void Start()
    {
        lineManager = FindObjectOfType<LineManager>();
    }

    // 1. Týklama Baþladý (Çizgiyi Baþlat)
    public void OnPointerDown(PointerEventData eventData)
    {
        if (lineManager != null)
        {
            lineManager.StartDrawing(this);
        }
    }

    // 2. Sürükleniyor (Çizgiyi Güncelle)
    public void OnDrag(PointerEventData eventData)
    {
        if (lineManager != null)
        {
            lineManager.UpdateDrawing(eventData.position);
        }
    }

    // 3. Býrakýldý (Kontrol Et)
    public void OnPointerUp(PointerEventData eventData)
    {
        if (lineManager != null)
        {
            lineManager.FinishDrawing();
        }
    }

    // 4. Parmak Üzerime Geldi (Hedef Ben Miyim?)
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (lineManager != null)
        {
            lineManager.SetHoveredDot(this); // Yöneticiye "Þu an benim üzerimdesin" de
        }
    }

    // 5. Parmak Üzerimden Gitti
    public void OnPointerExit(PointerEventData eventData)
    {
        if (lineManager != null)
        {
            lineManager.SetHoveredDot(null); // Yöneticiye "Boþluktasýn" de
        }
    }
}