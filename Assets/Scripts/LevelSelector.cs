using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    // Bu fonksiyonu butonlara baðlayacaðýz.
    // index = 0 (Araba), index = 1 (Bisiklet) gibi...
    public void SelectLevel(int levelIndex)
    {
        // 1. Seçimi Hafýzaya Kaydet
        PlayerPrefs.SetInt("SelectedLevelIndex", levelIndex);

        // 2. Boyama Sahnesini Aç
        SceneManager.LoadScene("PaintingGame");
    }

    public void GoBack()
    {
        SceneManager.LoadScene("ActivitySelection");
    }
}