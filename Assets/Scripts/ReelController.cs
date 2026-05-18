// ReelController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ReelController : MonoBehaviour
{
    [Header("Reel Settings")]
    public SymbolData[] symbols;
    public RectTransform symbolStrip;
    public float spinSpeed = 800f;
    public float symbolHeight = 200f;

    public bool IsSpinning { get; private set; }
    private int _resultIndex;
    private Image[] _symbolImages;
    private int _currentCenterIndex = 0;

    // We show 5 slots: partial, full, FULL CENTER, full, partial
    private const int SLOT_COUNT = 5;
    private const int CENTER_SLOT = 2; // index 2 is center

    void Start()
    {
        BuildStrip();
        SetStripToIndex(0);
    }

    void BuildStrip()
    {
        for (int i = symbolStrip.childCount - 1; i >= 0; i--)
            Destroy(symbolStrip.GetChild(i).gameObject);

        // Strip stretches reel width, anchors to top
        symbolStrip.anchorMin = new Vector2(0f, 1f);
        symbolStrip.anchorMax = new Vector2(1f, 1f);
        symbolStrip.pivot = new Vector2(0.5f, 1f);
        symbolStrip.anchoredPosition = Vector2.zero;
        symbolStrip.sizeDelta = new Vector2(0f,
            symbolHeight * SLOT_COUNT);

        _symbolImages = new Image[SLOT_COUNT];

        for (int i = 0; i < SLOT_COUNT; i++)
        {
            var go = new GameObject($"Slot_{i}");
            go.transform.SetParent(symbolStrip, false);

            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 1f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(0f, symbolHeight);
            rt.anchoredPosition = new Vector2(0f,
                -(i * symbolHeight) - symbolHeight * 0.5f);

            var img = go.AddComponent<Image>();
            img.preserveAspect = true;

            _symbolImages[i] = img;
        }
    }

    /// <summary>
    /// Refreshes all slot sprites so centerSymbolIndex
    /// appears at CENTER_SLOT, with correct symbols above/below.
    /// </summary>
    void RefreshSprites(int centerSymbolIndex)
    {
        for (int i = 0; i < SLOT_COUNT; i++)
        {
            int offset = i - CENTER_SLOT;
            int symIndex = (centerSymbolIndex + offset
                + symbols.Length * 10) % symbols.Length;
            _symbolImages[i].sprite = symbols[symIndex].sprite;
        }
    }

    void SetStripToIndex(int symbolIndex)
    {
        _currentCenterIndex = symbolIndex;
        RefreshSprites(symbolIndex);
        // Position strip so CENTER_SLOT aligns with reel center
        // Reel center = -symbolHeight/2 from reel top
        // Slot i center in strip = -(i * symbolHeight + symbolHeight/2)
        // strip.y = reel_center - slot_center
        //         = CENTER_SLOT * symbolHeight
        symbolStrip.anchoredPosition = new Vector2(0f,
            CENTER_SLOT * symbolHeight);
    }

    public void Spin(int targetIndex, float stopDelay,
        System.Action onStopped)
    {
        _resultIndex = targetIndex;
        StartCoroutine(SpinRoutine(stopDelay, onStopped));
    }

    IEnumerator SpinRoutine(float stopDelay, System.Action onStopped)
    {
        IsSpinning = true;
        float elapsed = 0f;
        float slotTravelTime = symbolHeight / spinSpeed;
        float nextSwapTime = slotTravelTime;

        // Start from known centered position
        symbolStrip.anchoredPosition = new Vector2(0f,
            CENTER_SLOT * symbolHeight);

        while (elapsed < stopDelay)
        {
            // Scroll downward — symbols appear to fall down
            symbolStrip.anchoredPosition -= new Vector2(0f,
                spinSpeed * Time.deltaTime);

            // When strip has moved one slot down, swap sprites
            // and reset to center so it loops seamlessly
            if (elapsed >= nextSwapTime)
            {
                nextSwapTime += slotTravelTime;
                _currentCenterIndex = (_currentCenterIndex + 1)
                    % symbols.Length;
                RefreshSprites(_currentCenterIndex);
                // Snap back to centered position — user won't notice
                symbolStrip.anchoredPosition = new Vector2(0f,
                    CENTER_SLOT * symbolHeight);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to final result
        yield return StartCoroutine(SnapToResult());
        IsSpinning = false;
        onStopped?.Invoke();
    }

    IEnumerator SnapToResult()
    {
        RefreshSprites(_resultIndex);
        float targetY = CENTER_SLOT * symbolHeight;
        float startY = symbolStrip.anchoredPosition.y;
        float duration = 0.25f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = 1f - Mathf.Pow(
                1f - Mathf.Clamp01(elapsed / duration), 3f);
            symbolStrip.anchoredPosition = new Vector2(0f,
                Mathf.Lerp(startY, targetY, t));
            yield return null;
        }

        symbolStrip.anchoredPosition = new Vector2(0f, targetY);
        _currentCenterIndex = _resultIndex;
    }

    public SymbolData GetResult() => symbols[_resultIndex];
    public int GetResultIndex() => _resultIndex;
}