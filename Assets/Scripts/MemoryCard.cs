using System.Buffers;
using UnityEngine;
using UnityEngine.UI;

public class MemoryCard : MonoBehaviour
{
    [Header("Kart Kimliði")]
    public string cardID; // Örn: "ucgen", "elma"

    [Header("Kapak Resmi")]
    public GameObject coverImage; // Kartýn arkasý (Cover objesi)

    [HideInInspector] public bool isMatched = false; // Eþleþti mi?
    private Button myButton;
    private MemoryManager manager;

    void Start()
    {
        myButton = GetComponent<Button>();
        manager = FindObjectOfType<MemoryManager>();

        // Týklama olayýný baðla
        myButton.onClick.AddListener(OnCardClicked);
    }

    void OnCardClicked()
    {
        // Eðer zaten açýksa veya oyun kilitliyse (animasyon varsa) týklama
        if (coverImage.activeSelf && manager.CanClick())
        {
            FlipOpen();
            manager.CardSelected(this);
        }
    }

    // Kartý Aç (Kapaðý Gizle)
    public void FlipOpen()
    {
        coverImage.SetActive(false);
    }

    // Kartý Kapat (Kapaðý Göster)
    public void FlipClose()
    {
        coverImage.SetActive(true);
    }

    // Eþleþince Kilitle
    public void LockCard()
    {
        isMatched = true;
        myButton.interactable = false; // Artýk týklanamaz
    }
}