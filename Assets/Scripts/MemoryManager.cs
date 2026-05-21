using UnityEngine;
using System.Collections;

public class MemoryManager : MonoBehaviour
{
    private MemoryCard firstCard;
    private MemoryCard secondCard;

    private bool isProcessing = false; // Þu an kontrol yapýyor muyuz?

    public bool CanClick()
    {
        return !isProcessing; // Ýþlem yapýyorsak týklamaya izin verme
    }

    public void CardSelected(MemoryCard card)
    {
        // Ýlk kart mý seçiliyor?
        if (firstCard == null)
        {
            firstCard = card;
        }
        // Ýkinci kart mý seçiliyor?
        else
        {
            secondCard = card;
            isProcessing = true; // Diðer týklamalarý engelle

            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        // Kartlar açýlsýn diye azýcýk bekle (Oyuncu görsün)
        yield return new WaitForSeconds(0.5f);

        // ID'leri ayný mý?
        if (firstCard.cardID == secondCard.cardID)
        {
            // --- EÞLEÞTÝ! ---
            firstCard.LockCard();
            secondCard.LockCard();
            Debug.Log("Eþleþme Baþarýlý!");

            // Burada "Tebrikler" sesi çalabilirsin
        }
        else
        {
            // --- YANLIÞ! ---
            // Oyuncu yanlýþ olduðunu görsün diye biraz daha beklet
            yield return new WaitForSeconds(0.5f);

            // Kartlarý geri kapat
            firstCard.FlipClose();
            secondCard.FlipClose();
        }

        // Seçimleri sýfýrla, yeni tura hazýrlan
        firstCard = null;
        secondCard = null;
        isProcessing = false;
    }
}