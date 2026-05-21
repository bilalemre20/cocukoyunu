using UnityEngine;
using UnityEngine.SceneManagement;

public class ShapeLevelSelector : MonoBehaviour
{
    public void SelectLevel(int levelIndex)
    {
        // 1. Seçilen leveli hafýzaya "SelectedShapeLevel" adýyla kaydet
        PlayerPrefs.SetInt("SelectedShapeLevel", levelIndex);

        // 2. Þekil Oyun Sahnesini Aç
        SceneManager.LoadScene("ShapeGame");
    }

    public void GoBack()
    {
        // Ana Menüye Dön
        SceneManager.LoadScene("ActivitySelection");
    }
}