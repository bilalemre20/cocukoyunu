using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PaintingManager : MonoBehaviour
{
    public static PaintingManager Instance;

    // --- YENÝ EKLENEN LÝSTE ---
    [Header("Level Resimleri (SIRASI ÖNEMLÝ!)")]
    public Sprite[] allLevelImages;
    // --------------------------

    [Header("Boya Ayarlarý")]
    public Color currentPaintColor = Color.black;
    [Range(5, 50)] public int brushSize = 20;

    [Header("Hassasiyet Ayarlarý")]
    [Range(0.01f, 0.5f)] public float lineDarknessThreshold = 0.3f;

    [Header("Geçmiþ Ayarlarý")]
    public int maxHistorySize = 10;

    private SpriteRenderer spriteRenderer;
    private Texture2D cloneTexture;
    private Texture2D baseTexture;
    private float pixelsPerUnit;

    private bool isErasing = false;
    private bool[,] currentAllowedRegionMask;
    private bool isMaskCalculated = false;

    private List<Color[]> undoHistory = new List<Color[]>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // --- YENÝ EKLENEN KISIM: SEÇÝLEN RESMÝ YÜKLEME ---
        // 1. Menüden hangi numaranýn seçildiðini öðren (Varsayýlan 0)
        int selectedIndex = PlayerPrefs.GetInt("SelectedLevelIndex", 0);

        // 2. Eðer liste doluysa ve numara geçerliyse o resmi SpriteRenderer'a koy
        if (allLevelImages != null && allLevelImages.Length > 0)
        {
            if (selectedIndex >= 0 && selectedIndex < allLevelImages.Length)
            {
                spriteRenderer.sprite = allLevelImages[selectedIndex];
            }
        }
        // ----------------------------------------------------

        // Artýk spriteRenderer.sprite güncellendiði için alttaki kodlar seçilen resme göre çalýþacak
        baseTexture = spriteRenderer.sprite.texture;
        pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;

        cloneTexture = new Texture2D(baseTexture.width, baseTexture.height);
        cloneTexture.SetPixels(baseTexture.GetPixels());
        cloneTexture.Apply();

        Sprite newSprite = Sprite.Create(cloneTexture, spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f), pixelsPerUnit);
        spriteRenderer.sprite = newSprite;

        if (GetComponent<BoxCollider2D>() != null)
            GetComponent<BoxCollider2D>().size = spriteRenderer.sprite.bounds.size;

        currentAllowedRegionMask = new bool[cloneTexture.width, cloneTexture.height];
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pixelPos = GetPixelCoordinate(Input.mousePosition);
            if (IsInsideBounds(pixelPos))
            {
                SaveToHistory();
                GenerateRegionMask((int)pixelPos.x, (int)pixelPos.y);
                isMaskCalculated = true;
            }
        }

        if (Input.GetMouseButton(0) && isMaskCalculated)
        {
            Vector2 pixelPos = GetPixelCoordinate(Input.mousePosition);
            if (IsInsideBounds(pixelPos))
            {
                DrawBrush((int)pixelPos.x, (int)pixelPos.y, brushSize);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMaskCalculated = false;
        }
    }

    void SaveToHistory()
    {
        Color[] currentPixels = cloneTexture.GetPixels();
        undoHistory.Add(currentPixels);
        if (undoHistory.Count > maxHistorySize) undoHistory.RemoveAt(0);
    }

    public void UndoLastAction()
    {
        if (undoHistory.Count > 0)
        {
            Color[] previousPixels = undoHistory[undoHistory.Count - 1];
            cloneTexture.SetPixels(previousPixels);
            cloneTexture.Apply();
            undoHistory.RemoveAt(undoHistory.Count - 1);
        }
    }

    Vector2 GetPixelCoordinate(Vector3 mousePos)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 localPoint = transform.InverseTransformPoint(worldPoint);
        float pixelX = (localPoint.x * pixelsPerUnit) + (cloneTexture.width * 0.5f);
        float pixelY = (localPoint.y * pixelsPerUnit) + (cloneTexture.height * 0.5f);
        return new Vector2(Mathf.FloorToInt(pixelX), Mathf.FloorToInt(pixelY));
    }

    bool IsInsideBounds(Vector2 pos)
    {
        return pos.x >= 0 && pos.x < cloneTexture.width && pos.y >= 0 && pos.y < cloneTexture.height;
    }

    void GenerateRegionMask(int startX, int startY)
    {
        System.Array.Clear(currentAllowedRegionMask, 0, currentAllowedRegionMask.Length);
        Color startColor = cloneTexture.GetPixel(startX, startY);

        if (IsBlackLine(startColor) || startColor.a < 0.1f) return;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));
        currentAllowedRegionMask[startX, startY] = true;

        int width = cloneTexture.width;
        int height = cloneTexture.height;
        int safeCounter = 0;
        int maxIterations = width * height;

        while (queue.Count > 0 && safeCounter < maxIterations)
        {
            Vector2Int p = queue.Dequeue();
            safeCounter++;
            Vector2Int[] dirs = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };

            foreach (var dir in dirs)
            {
                int nx = p.x + dir.x;
                int ny = p.y + dir.y;

                if (nx < 0 || nx >= width || ny < 0 || ny >= height) continue;
                if (currentAllowedRegionMask[nx, ny]) continue;

                Color neighborColor = cloneTexture.GetPixel(nx, ny);
                if (!IsBlackLine(neighborColor))
                {
                    currentAllowedRegionMask[nx, ny] = true;
                    queue.Enqueue(new Vector2Int(nx, ny));
                }
            }
        }
    }

    void DrawBrush(int centerX, int centerY, int radius)
    {
        bool pixelsChanged = false;
        int width = cloneTexture.width;
        int height = cloneTexture.height;

        for (int x = centerX - radius; x <= centerX + radius; x++)
        {
            for (int y = centerY - radius; y <= centerY + radius; y++)
            {
                if (x < 0 || x >= width || y < 0 || y >= height) continue;
                if (Vector2Int.Distance(new Vector2Int(centerX, centerY), new Vector2Int(x, y)) > radius) continue;
                if (currentAllowedRegionMask[x, y] == false) continue;

                Color pixelUnderBrush = cloneTexture.GetPixel(x, y);

                if (pixelUnderBrush.a < 0.1f) continue;
                if (IsBlackLine(pixelUnderBrush)) continue;

                Color targetColor = isErasing ? baseTexture.GetPixel(x, y) : currentPaintColor;
                if (pixelUnderBrush == targetColor) continue;

                cloneTexture.SetPixel(x, y, targetColor);
                pixelsChanged = true;
            }
        }

        if (pixelsChanged)
        {
            cloneTexture.Apply();
        }
    }

    bool IsBlackLine(Color c)
    {
        return c.r < lineDarknessThreshold && c.g < lineDarknessThreshold && c.b < lineDarknessThreshold && c.a > 0.8f;
    }

    public void SetColor(Color newColor)
    {
        isErasing = false;
        currentPaintColor = newColor;
    }

    public void ActivateEraser()
    {
        isErasing = true;
    }
}