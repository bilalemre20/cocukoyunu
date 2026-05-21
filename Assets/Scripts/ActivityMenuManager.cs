using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ActivityMenuManager : MonoBehaviour, IEndDragHandler
{
    [Header("UI Baðlantýlarý")]
    public ScrollRect scrollRect;
    public RectTransform contentPanel;
    public Button nextButton;   // RightButton buraya
    public Button prevButton;   // LeftButton buraya

    [Header("Ayarlar")]
    public float snapSpeed = 10f; // Kayma hýzý

    private float[] pagePositions;
    private int currentPageIndex = 0;
    private bool isSnapping = false;
    private float targetH = 0;

    void Start()
    {
        // Ok butonlarýný dinle
        if (nextButton != null) nextButton.onClick.AddListener(NextPage);
        if (prevButton != null) prevButton.onClick.AddListener(PrevPage);

        Canvas.ForceUpdateCanvases();
        Invoke("InitializePagePositions", 0.1f);
    }

    void InitializePagePositions()
    {
        int pageCount = contentPanel.childCount;
        if (pageCount == 0) return;

        pagePositions = new float[pageCount];
        float step = 1f / (pageCount - 1);

        for (int i = 0; i < pageCount; i++)
        {
            pagePositions[i] = i * step;
        }
        UpdateArrowButtons();
    }

    void Update()
    {
        if (isSnapping && pagePositions != null)
        {
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
                scrollRect.horizontalNormalizedPosition, targetH, snapSpeed * Time.deltaTime
            );

            if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetH) < 0.001f)
            {
                scrollRect.horizontalNormalizedPosition = targetH;
                isSnapping = false;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        SnapToNearest();
    }

    void SnapToNearest()
    {
        float currentPos = scrollRect.horizontalNormalizedPosition;
        float nearestDist = float.MaxValue;
        int nearestIndex = 0;

        for (int i = 0; i < pagePositions.Length; i++)
        {
            float dist = Mathf.Abs(currentPos - pagePositions[i]);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearestIndex = i;
            }
        }
        GoToPage(nearestIndex);
    }

    public void NextPage()
    {
        if (currentPageIndex < pagePositions.Length - 1) GoToPage(currentPageIndex + 1);
    }

    public void PrevPage()
    {
        if (currentPageIndex > 0) GoToPage(currentPageIndex - 1);
    }

    void GoToPage(int index)
    {
        currentPageIndex = index;
        targetH = pagePositions[index];
        isSnapping = true;
        UpdateArrowButtons();
    }

    void UpdateArrowButtons()
    {
        if (prevButton) prevButton.interactable = (currentPageIndex > 0);
        if (nextButton) nextButton.interactable = (currentPageIndex < pagePositions.Length - 1);
    }

    // --- OYUN BAÞLATMA FONKSÝYONLARI ---

    public void PlayBoyama()
    {
        // Boyama sahnesinin adý tam olarak "PaintingGame" olmalý
        //SceneManager.LoadScene("PaintingGame");
        SceneManager.LoadScene("ColoringLevelMenu");
    }

    public void PlayKamyon()
    {
        Debug.Log("Kamyon oyunu açýlýyor (Henüz sahne yok)");
        // SceneManager.LoadScene("SizeGame"); 
    }

    public void PlaySekil()
    {
        // Þekil eþleþtirme level seçim ekranýna git
        SceneManager.LoadScene("ShapeLevelMenu");
    }
}