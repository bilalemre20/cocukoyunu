using UnityEngine;
using UnityEngine.UI;

public class EraserButton : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnEraserClick);
    }

    void OnEraserClick()
    {
        if (PaintingManager.Instance != null)
        {
            PaintingManager.Instance.ActivateEraser();
            Debug.Log("Silgi Modu Aktif!");
        }
    }
}