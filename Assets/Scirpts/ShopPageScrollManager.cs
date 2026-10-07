using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Left / Right buttons se shop ke pages change hote hain.
/// - Page indicator dots
/// - Har page ke apne buttons
/// - Ek common price text jo page ke hisaab se badalta hai
/// </summary>
public class ShopPageScrollManager : MonoBehaviour
{
    [Serializable]
    public class ShopPage
    {
        public string pageName = "Page";
        public RectTransform page;                                  // Content ke andar ka page object
        [TextArea] public string priceLabel = "Buy for 100 Coins";  // Common text me ye dikhega
        public Button[] buttons;                                    // Is page ke buttons
    }

    [Header("Layout")]
    [SerializeField] private RectTransform viewport;   // Masked area (jitna ek page dikhta hai)
    [SerializeField] private RectTransform content;    // Sab pages ka parent

    [Header("Pages")]
    [SerializeField] private List<ShopPage> pages = new List<ShopPage>();

    [Header("Left / Right Buttons")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private bool loopPages = false;   // True: last ke baad wapas first page

    [Header("Common Price Text")]
    [SerializeField] private TMP_Text priceText;

    [Header("Page Indicator Dots")]
    [SerializeField] private Transform dotsContainer;  // HorizontalLayoutGroup wala object
    [SerializeField] private Image dotPrefab;
    [SerializeField] private Color activeDotColor = Color.white;
    [SerializeField] private Color inactiveDotColor = new Color(1f, 1f, 1f, 0.35f);
    [SerializeField] private float activeDotScale = 1.3f;

    [Header("Animation")]
    [SerializeField] private float snapSpeed = 12f;

    [Header("Events")]
    public UnityEvent<int> onPageChanged;

    private readonly List<Image> dots = new List<Image>();
    private int currentPage;
    private float pageWidth;
    private bool ready;

    public int CurrentPage => currentPage;
    public int PageCount => pages.Count;

    private void Awake()
    {
        if (leftButton != null) leftButton.onClick.AddListener(PreviousPage);
        if (rightButton != null) rightButton.onClick.AddListener(NextPage);
    }

    private IEnumerator Start()
    {
        if (viewport == null && content != null) viewport = content.parent as RectTransform;

        // Content par agar Layout Group / Size Fitter ho to wo pages ko apni jagah se hata deta hai
        foreach (var lg in content.GetComponents<LayoutGroup>()) lg.enabled = false;
        foreach (var sf in content.GetComponents<ContentSizeFitter>()) sf.enabled = false;

        // Viewport ki width asli (0 se zyada) hone tak wait karo
        int tries = 0;
        while (tries < 30)
        {
            Canvas.ForceUpdateCanvases();
            if (viewport.rect.width > 1f) break;
            tries++;
            yield return null;
        }

        SetupLayout();
        CreateDots();
        content.anchoredPosition = Vector2.zero;
        ready = true;
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (leftButton != null) leftButton.onClick.RemoveListener(PreviousPage);
        if (rightButton != null) rightButton.onClick.RemoveListener(NextPage);
    }

    // ---------- Setup ----------
    private void SetupLayout()
    {
        pageWidth = viewport.rect.width;

        content.anchorMin = new Vector2(0, 0);
        content.anchorMax = new Vector2(0, 1);
        content.pivot = new Vector2(0, 0.5f);
        content.sizeDelta = new Vector2(pageWidth * pages.Count, 0);

        for (int i = 0; i < pages.Count; i++)
        {
            RectTransform p = pages[i].page;
            if (p == null) continue;
            p.anchorMin = new Vector2(0, 0);
            p.anchorMax = new Vector2(0, 1);
            p.pivot = new Vector2(0, 0.5f);
            p.sizeDelta = new Vector2(pageWidth, 0);
            p.anchoredPosition = new Vector2(i * pageWidth, 0);
        }
    }

    private void CreateDots()
    {
        foreach (var d in dots) if (d != null) Destroy(d.gameObject);
        dots.Clear();

        if (dotsContainer == null || dotPrefab == null) return;

        for (int i = 0; i < pages.Count; i++)
        {
            Image dot = Instantiate(dotPrefab, dotsContainer);
            dot.gameObject.SetActive(true);
            dots.Add(dot);
        }
    }

    // ---------- Smooth move ----------
    private void Update()
    {
        if (!ready || pages.Count == 0) return;

        // Screen/Canvas size badle to pages dobara set karo
        if (Mathf.Abs(viewport.rect.width - pageWidth) > 0.5f)
        {
            SetupLayout();
            content.anchoredPosition = new Vector2(-currentPage * pageWidth, 0);
        }

        Vector2 pos = content.anchoredPosition;
        float targetX = -currentPage * pageWidth;
        pos.x = Mathf.Lerp(pos.x, targetX, Time.unscaledDeltaTime * snapSpeed);
        if (Mathf.Abs(pos.x - targetX) < 0.5f) pos.x = targetX;
        content.anchoredPosition = pos;
    }

    // ---------- Public API ----------
    private int lastMoveFrame = -1;

    // Ek hi frame me do baar call aaye (button par double onClick) to dusri ignore
    private bool AlreadyMovedThisFrame()
    {
        if (lastMoveFrame == Time.frameCount) return true;
        lastMoveFrame = Time.frameCount;
        return false;
    }

    public void NextPage()
    {
        if (AlreadyMovedThisFrame()) return;
        int next = currentPage + 1;
        if (next >= pages.Count) next = loopPages ? 0 : pages.Count - 1;
        GoToPage(next);
    }

    public void PreviousPage()
    {
        if (AlreadyMovedThisFrame()) return;
        int prev = currentPage - 1;
        if (prev < 0) prev = loopPages ? pages.Count - 1 : 0;
        GoToPage(prev);
    }

    public void GoToPage(int index)
    {
        if (pages.Count == 0) return;
        index = Mathf.Clamp(index, 0, pages.Count - 1);
        bool changed = index != currentPage;
        currentPage = index;
        UpdateUI();
        if (changed) onPageChanged?.Invoke(currentPage);
    }

    /// <summary>Runtime par kisi page ka price text badalna ho to.</summary>
    public void SetPriceLabel(int pageIndex, string label)
    {
        if (pageIndex < 0 || pageIndex >= pages.Count) return;
        pages[pageIndex].priceLabel = label;
        if (pageIndex == currentPage) UpdateUI();
    }

    /// <summary>Kisi page ke buttons code se access karne ke liye.</summary>
    public Button[] GetPageButtons(int pageIndex)
    {
        if (pageIndex < 0 || pageIndex >= pages.Count) return null;
        return pages[pageIndex].buttons;
    }

    // ---------- UI ----------
    private void UpdateUI()
    {
        if (pages.Count == 0) return;

        // Common price text
        if (priceText != null)
            priceText.text = pages[currentPage].priceLabel;

        // Dots
        for (int i = 0; i < dots.Count; i++)
        {
            bool active = i == currentPage;
            dots[i].color = active ? activeDotColor : inactiveDotColor;
            dots[i].rectTransform.localScale = Vector3.one * (active ? activeDotScale : 1f);
        }

        // Left/Right buttons (loop off ho to first/last page par disable)
        if (!loopPages)
        {
            if (leftButton != null) leftButton.interactable = currentPage > 0;
            if (rightButton != null) rightButton.interactable = currentPage < pages.Count - 1;
        }
    }
}