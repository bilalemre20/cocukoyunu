using UnityEngine;
using UnityEngine.SceneManagement; // Sahne geçiþleri için þart!

public class MainMenuManager : MonoBehaviour
{
    // Play tuþuna basýnca çalýþacak
    public void OpenActivitySelection()
    {
        SceneManager.LoadScene("ActivitySelection");
    }

    // Ayarlar tuþuna basýnca (Þimdilik sadece konsola yazsýn)
    public void OpenSettings()
    {
        Debug.Log("Ayarlar açýlýyor...");
        // Ýleride buraya ayarlar paneli açma kodu gelecek
    }

    // Oyundan çýkýþ tuþu için (Ýstersen ekleyebilirsin)
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Oyundan çýkýldý.");
    }
}