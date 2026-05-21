using UnityEngine;

public class LineManager : MonoBehaviour
{
    [Header("Ayarlar")]
    public GameObject linePrefab; // Mavi küp (Prefab) buraya sürüklenecek

    private LineRenderer currentLine;
    private DotPoint startDot;
    private DotPoint targetDot;

    // --- 1. Çizime Baþla (KIRMIZI OLACAK) ---
    public void StartDrawing(DotPoint dot)
    {
        startDot = dot;

        // Yeni çizgi oluþtur
        GameObject newLineObj = Instantiate(linePrefab);
        currentLine = newLineObj.GetComponent<LineRenderer>();
        currentLine.positionCount = 2;

        // Baþlangýç noktasýný ayarla
        Vector3 startPos = GetWorldPosition(startDot.transform.position);
        currentLine.SetPosition(0, startPos);
        currentLine.SetPosition(1, startPos);

        // --- YENÝ KISIM: BAÞLANGIÇ RENGÝNÝ KIRMIZI YAP ---
        Gradient redGradient = new Gradient();
        redGradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        currentLine.colorGradient = redGradient;
        // --------------------------------------------------
    }

    // --- 2. Sürüklerken Çizgiyi Güncelle ---
    public void UpdateDrawing(Vector2 screenPos)
    {
        if (currentLine != null)
        {
            Vector3 worldPos = GetWorldPosition(screenPos);
            currentLine.SetPosition(1, worldPos);
        }
    }

    // --- 3. Fareyi Býraktýðýnda (YEÞÝL OLACAK) ---
    public void FinishDrawing()
    {
        if (currentLine == null) return;

        // Eðer geçerli bir hedefin üzerindeysek VE ayný nokta deðilse
        if (targetDot != null && startDot != targetDot)
        {
            // A. Ayný gruptan mý? (Üst ile Üst eþleþemez)
            if (startDot.isTopRow == targetDot.isTopRow)
            {
                DestroyLine();
                return;
            }

            // B. ID'leri tutuyor mu? (Doðru eþleþme mi?)
            if (startDot.dotID == targetDot.dotID)
            {
                Debug.Log("DOÐRU EÞLEÞME!");

                // Çizgiyi hedefe kalýcý olarak yapýþtýr
                Vector3 endPos = GetWorldPosition(targetDot.transform.position);
                currentLine.SetPosition(1, endPos);

                // --- DOÐRU OLUNCA YEÞÝL YAP ---
                Gradient greenGradient = new Gradient();
                greenGradient.SetKeys(
                    new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.green, 1.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
                );
                currentLine.colorGradient = greenGradient;
                // ------------------------------

                currentLine = null; // Bu çizgiyle iþimiz bitti.
            }
            else
            {
                Debug.Log("YANLIÞ EÞLEÞME!");
                DestroyLine();
            }
        }
        else
        {
            // Boþluða býrakýldý
            DestroyLine();
        }

        // Temizlik
        startDot = null;
        targetDot = null;
    }

    public void SetHoveredDot(DotPoint dot)
    {
        targetDot = dot;
    }

    void DestroyLine()
    {
        if (currentLine != null)
        {
            Destroy(currentLine.gameObject);
            currentLine = null;
        }
    }

    Vector3 GetWorldPosition(Vector3 screenPos)
    {
        screenPos.z = 10f;
        return Camera.main.ScreenToWorldPoint(screenPos);
    }
}