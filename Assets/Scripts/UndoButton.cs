using UnityEngine;
using UnityEngine.UI;

public class UndoButton : MonoBehaviour
{
    void Start()
    {
        // Butona týklandýðýnda PaintingManager'daki Undo fonksiyonunu çalýþtýr
        GetComponent<Button>().onClick.AddListener(OnUndoClick);
    }

    void OnUndoClick()
    {
        if (PaintingManager.Instance != null)
        {
            PaintingManager.Instance.UndoLastAction();
        }
    }
}