using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBackButton : MonoBehaviour
{
    // Sarý oka bu fonksiyonu baðlayacaðýz
    public void GoBackToLevelMenu()
    {
        // 10'lu resim seçme sayfasýna (ColoringLevelMenu) geri dön
        SceneManager.LoadScene("ColoringLevelMenu");
    }
}