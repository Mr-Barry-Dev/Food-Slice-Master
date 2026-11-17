// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;

// /// <summary>
// /// Shop panel ke liye multi-page scroller.
// /// - Left / Right button se page change
// /// - Niche dots: selected dot YELLOW, baaki GREY
// /// - Dot select hone par animation (color + size + pop)
// /// - Mobile/mouse swipe support
// /// </summary>
// public class ShopPageScroller : MonoBehaviour, IBeginDragHandler, IEndDragHandler
// {
//     [Header("UI References")]
//     [Tooltip("Saare pages jiske child hain (ye move hoga). Isme Layout Group NA lagao.")]
//     [SerializeField] private RectTransform content;
//     [Tooltip("Visible area (Mask wala panel). Page width isi ke barabar hogi.")]
//     [SerializeField] private RectTransform viewport;
//     [SerializeField] private Button leftButton;
//     [SerializeField] private Button rightButton;

//     [Header("Dots")]
//     [Tooltip("Ek chhota Image prefab (circle sprite)")]
//     [SerializeField] private Image dotPrefab;
//     [SerializeField] private Transform dotsParent;
//     [SerializeField] private Color activeColor = new Color(1f, 0.76f, 0.03f, 1f); // Yellow
//     [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Grey
//     [SerializeField] private Vector2 dotSize = new Vector2(20f, 20f);
//     [SerializeField] private Vector2 activeDotSize = new Vector2(44f, 20f);  // lamba pill
//     [SerializeField] private float dotAnimTime = 0.3f;

//     [Header("Scroll")]
//     [SerializeField] private float slideTime = 0.4f;
//     [SerializeField] private float swipeThreshold = 60f;
//     [SerializeField] private bool loop = false;

//     private readonly List<Image> dots = new List<Image>();
//     private readonly List<Coroutine> dotRoutines = new List<Coroutine>();
//     private Coroutine slideRoutine;
//     private int pageCount;
//     private int currentPage;
//     private float pageWidth;
//     private float dragStartX;

//     private void Start()
//     {
//         Setup();
//     }

//     /// <summary>Agar runtime pe pages add/remove karo to ye dobara call karo.</summary>
//     public void Setup()
//     {
//         pageCount = content.childCount;
//         pageWidth = viewport.rect.width;

//         // Har page ko viewport ke size ka banao aur side-by-side rakho
//         for (int i = 0; i < pageCount; i++)
//         {
//             RectTransform page = content.GetChild(i) as RectTransform;
//             page.anchorMin = new Vector2(0f, 0.5f);
//             page.anchorMax = new Vector2(0f, 0.5f);
//             page.pivot = new Vector2(0f, 0.5f);
//             page.sizeDelta = new Vector2(pageWidth, viewport.rect.height);
//             page.anchoredPosition = new Vector2(i * pageWidth, 0f);
//         }

//         content.anchorMin = new Vector2(0f, 0.5f);
//         content.anchorMax = new Vector2(0f, 0.5f);
//         content.pivot = new Vector2(0f, 0.5f);
//         content.sizeDelta = new Vector2(pageWidth * pageCount, viewport.rect.height);

//         BuildDots();

//         leftButton.onClick.RemoveAllListeners();
//         rightButton.onClick.RemoveAllListeners();
//         leftButton.onClick.AddListener(Previous);
//         rightButton.onClick.AddListener(Next);

//         currentPage = Mathf.Clamp(currentPage, 0, Mathf.Max(0, pageCount - 1));
//         content.anchoredPosition = new Vector2(-currentPage * pageWidth, 0f);
//         UpdateDotsInstant();
//         UpdateButtons();
//     }

//     // ---------------- Navigation ----------------

//     public void Next()
//     {
//         if (currentPage < pageCount - 1) GoToPage(currentPage + 1);
//         else if (loop) GoToPage(0);
//     }

//     public void Previous()
//     {
//         if (currentPage > 0) GoToPage(currentPage - 1);
//         else if (loop) GoToPage(pageCount - 1);
//     }

//     public void GoToPage(int index)
//     {
//         index = Mathf.Clamp(index, 0, pageCount - 1);
//         if (index == currentPage && slideRoutine == null) { SnapBack(); return; }

//         int previous = currentPage;
//         currentPage = index;

//         if (slideRoutine != null) StopCoroutine(slideRoutine);
//         slideRoutine = StartCoroutine(SlideTo(-currentPage * pageWidth));

//         AnimateDot(previous, false);
//         AnimateDot(currentPage, true);
//         UpdateButtons();
//     }

//     private void SnapBack()
//     {
//         if (slideRoutine != null) StopCoroutine(slideRoutine);
//         slideRoutine = StartCoroutine(SlideTo(-currentPage * pageWidth));
//     }

//     private IEnumerator SlideTo(float targetX)
//     {
//         Vector2 start = content.anchoredPosition;
//         float t = 0f;
//         while (t < 1f)
//         {
//             t += Time.unscaledDeltaTime / slideTime;
//             float eased = EaseInOutCubic(Mathf.Clamp01(t));
//             content.anchoredPosition = new Vector2(Mathf.Lerp(start.x, targetX, eased), 0f);
//             yield return null;
//         }
//         content.anchoredPosition = new Vector2(targetX, 0f);
//         slideRoutine = null;
//     }

//     private void UpdateButtons()
//     {
//         if (loop) { leftButton.interactable = rightButton.interactable = pageCount > 1; return; }
//         leftButton.interactable = currentPage > 0;
//         rightButton.interactable = currentPage < pageCount - 1;
//     }

//     // ---------------- Dots ----------------

//     private void BuildDots()
//     {
//         foreach (Image d in dots) if (d != null) Destroy(d.gameObject);
//         dots.Clear();
//         dotRoutines.Clear();

//         for (int i = 0; i < pageCount; i++)
//         {
//             Image dot = Instantiate(dotPrefab, dotsParent);
//             dot.gameObject.SetActive(true);
//             int index = i;

//             // Dot pe click karke seedha us page par jaane ke liye
//             Button b = dot.GetComponent<Button>();
//             if (b == null) b = dot.gameObject.AddComponent<Button>();
//             b.targetGraphic = dot;
//             b.onClick.AddListener(() => GoToPage(index));

//             dots.Add(dot);
//             dotRoutines.Add(null);
//         }
//     }

//     private void UpdateDotsInstant()
//     {
//         for (int i = 0; i < dots.Count; i++)
//         {
//             bool active = i == currentPage;
//             dots[i].color = active ? activeColor : inactiveColor;
//             dots[i].rectTransform.sizeDelta = active ? activeDotSize : dotSize;
//             dots[i].rectTransform.localScale = Vector3.one;
//         }
//     }

//     private void AnimateDot(int index, bool becomeActive)
//     {
//         if (index < 0 || index >= dots.Count) return;
//         if (dotRoutines[index] != null) StopCoroutine(dotRoutines[index]);
//         dotRoutines[index] = StartCoroutine(DotRoutine(index, becomeActive));
//     }

//     private IEnumerator DotRoutine(int index, bool becomeActive)
//     {
//         Image dot = dots[index];
//         RectTransform rt = dot.rectTransform;

//         Color fromColor = dot.color;
//         Color toColor = becomeActive ? activeColor : inactiveColor;
//         Vector2 fromSize = rt.sizeDelta;
//         Vector2 toSize = becomeActive ? activeDotSize : dotSize;

//         float t = 0f;
//         while (t < 1f)
//         {
//             t += Time.unscaledDeltaTime / dotAnimTime;
//             float k = Mathf.Clamp01(t);
//             float eased = EaseInOutCubic(k);

//             dot.color = Color.Lerp(fromColor, toColor, eased);
//             rt.sizeDelta = Vector2.Lerp(fromSize, toSize, eased);

//             // Selected dot par halka "pop" (bounce) effect
//             float pop = becomeActive ? 1f + Mathf.Sin(k * Mathf.PI) * 0.35f : 1f;
//             rt.localScale = new Vector3(1f, pop, 1f);

//             yield return null;
//         }

//         dot.color = toColor;
//         rt.sizeDelta = toSize;
//         rt.localScale = Vector3.one;
//         dotRoutines[index] = null;
//     }

//     // ---------------- Swipe ----------------

//     public void OnBeginDrag(PointerEventData eventData)
//     {
//         dragStartX = eventData.position.x;
//     }

//     public void OnEndDrag(PointerEventData eventData)
//     {
//         float diff = eventData.position.x - dragStartX;
//         if (Mathf.Abs(diff) < swipeThreshold) { SnapBack(); return; }
//         if (diff < 0) Next(); else Previous();
//     }

//     // ---------------- Helpers ----------------

//     private static float EaseInOutCubic(float x)
//     {
//         return x < 0.5f ? 4f * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;
//     }
// }



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Shop panel ke liye multi-page scroller.
/// - Left / Right button se page change
/// - Niche dots: selected dot YELLOW, baaki GREY
/// - Dot select hone par animation (color + size + pop)
/// - Har page ke apne Buy buttons (array): naya page aane par purane hide, naye show
/// - Mobile/mouse swipe support
/// </summary>
public class ShopPageScroller : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    /// <summary>Ek page ke buttons. Inspector me Element 0 = Page 1, Element 1 = Page 2 ...</summary>
    [System.Serializable]
    public class PageButtons
    {
        public string pageName = "Page";
        [Tooltip("Is page ke saare Buy buttons (jitne chahiye add karo)")]
        public GameObject[] buyButtons;
    }

    [Header("UI References")]
    [Tooltip("Saare pages jiske child hain (ye move hoga). Isme Layout Group NA lagao.")]
    [SerializeField] private RectTransform content;
    [Tooltip("Visible area (Mask wala panel). Page width isi ke barabar hogi.")]
    [SerializeField] private RectTransform viewport;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [Header("Page Buy Buttons")]
    [Tooltip("Har page ke liye ek element. Order wahi rakho jo Content ke pages ka hai.")]
    [SerializeField] private PageButtons[] pageButtons;
    [SerializeField] private float buttonFadeTime = 0.25f;
    [Tooltip("Naya page slide hone ke itne second baad uske buttons show honge")]
    [SerializeField] private float showDelay = 0.15f;
    [Tooltip("Ek ke baad ek button aane ka gap (0 = sab ek saath)")]
    [SerializeField] private float buttonStagger = 0.07f;

    [Header("Dots")]
    [Tooltip("Ek chhota Image prefab (circle sprite)")]
    [SerializeField] private Image dotPrefab;
    [SerializeField] private Transform dotsParent;
    [SerializeField] private Color activeColor = new Color(1f, 0.76f, 0.03f, 1f); // Yellow
    [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Grey
    [SerializeField] private Vector2 dotSize = new Vector2(20f, 20f);
    [SerializeField] private Vector2 activeDotSize = new Vector2(44f, 20f);  // lamba pill
    [SerializeField] private float dotAnimTime = 0.3f;

    [Header("Scroll")]
    [SerializeField] private float slideTime = 0.4f;
    [SerializeField] private float swipeThreshold = 60f;
    [SerializeField] private bool loop = false;

    private readonly List<Image> dots = new List<Image>();
    private readonly List<Coroutine> dotRoutines = new List<Coroutine>();
    private readonly Dictionary<GameObject, Coroutine> buttonRoutines = new Dictionary<GameObject, Coroutine>();
    private Coroutine slideRoutine;
    private int pageCount;
    private int currentPage;
    private float pageWidth;
    private float dragStartX;

    private void Start()
    {
        Setup();
    }

    /// <summary>Agar runtime pe pages add/remove karo to ye dobara call karo.</summary>
    public void Setup()
    {
        Canvas.ForceUpdateCanvases();

        pageCount = content.childCount;
        pageWidth = viewport.rect.width;

        // Har page ko viewport ke size ka banao aur side-by-side rakho
        for (int i = 0; i < pageCount; i++)
        {
            RectTransform page = content.GetChild(i) as RectTransform;
            page.anchorMin = new Vector2(0f, 0.5f);
            page.anchorMax = new Vector2(0f, 0.5f);
            page.pivot = new Vector2(0f, 0.5f);
            page.sizeDelta = new Vector2(pageWidth, viewport.rect.height);
            page.anchoredPosition = new Vector2(i * pageWidth, 0f);
        }

        content.anchorMin = new Vector2(0f, 0.5f);
        content.anchorMax = new Vector2(0f, 0.5f);
        content.pivot = new Vector2(0f, 0.5f);
        content.sizeDelta = new Vector2(pageWidth * pageCount, viewport.rect.height);

        BuildDots();

        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();
        leftButton.onClick.AddListener(Previous);
        rightButton.onClick.AddListener(Next);

        currentPage = Mathf.Clamp(currentPage, 0, Mathf.Max(0, pageCount - 1));
        content.anchoredPosition = new Vector2(-currentPage * pageWidth, 0f);
        UpdateDotsInstant();
        UpdateButtons();
        SetPageButtonsInstant(currentPage);
    }

    // ---------------- Navigation ----------------

    public void Next()
    {
        if (currentPage < pageCount - 1) GoToPage(currentPage + 1);
        else if (loop) GoToPage(0);
    }

    public void Previous()
    {
        if (currentPage > 0) GoToPage(currentPage - 1);
        else if (loop) GoToPage(pageCount - 1);
    }

    public void GoToPage(int index)
    {
        index = Mathf.Clamp(index, 0, pageCount - 1);
        if (index == currentPage && slideRoutine == null) { SnapBack(); return; }

        int previous = currentPage;
        currentPage = index;

        if (slideRoutine != null) StopCoroutine(slideRoutine);
        slideRoutine = StartCoroutine(SlideTo(-currentPage * pageWidth));

        AnimateDot(previous, false);
        AnimateDot(currentPage, true);
        UpdateButtons();

        HidePageButtons(previous);
        ShowPageButtons(currentPage);
    }

    private void SnapBack()
    {
        if (slideRoutine != null) StopCoroutine(slideRoutine);
        slideRoutine = StartCoroutine(SlideTo(-currentPage * pageWidth));
    }

    private IEnumerator SlideTo(float targetX)
    {
        Vector2 start = content.anchoredPosition;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / slideTime;
            float eased = EaseInOutCubic(Mathf.Clamp01(t));
            content.anchoredPosition = new Vector2(Mathf.Lerp(start.x, targetX, eased), 0f);
            yield return null;
        }
        content.anchoredPosition = new Vector2(targetX, 0f);
        slideRoutine = null;
    }

    private void UpdateButtons()
    {
        if (loop) { leftButton.interactable = rightButton.interactable = pageCount > 1; return; }
        leftButton.interactable = currentPage > 0;
        rightButton.interactable = currentPage < pageCount - 1;
    }

    // ---------------- Page Buy Buttons ----------------

    private GameObject[] GetButtons(int page)
    {
        if (pageButtons == null || page < 0 || page >= pageButtons.Length) return null;
        return pageButtons[page].buyButtons;
    }

    /// <summary>Start par: sirf current page ke buttons on, baaki sab off.</summary>
    private void SetPageButtonsInstant(int activePage)
    {
        if (pageButtons == null) return;

        for (int p = 0; p < pageButtons.Length; p++)
        {
            GameObject[] list = GetButtons(p);
            if (list == null) continue;

            foreach (GameObject go in list)
            {
                if (go == null) continue;
                CanvasGroup cg = GetCanvasGroup(go);
                bool on = p == activePage;
                cg.alpha = on ? 1f : 0f;
                go.transform.localScale = Vector3.one;
                go.SetActive(on);
            }
        }
    }

    private void HidePageButtons(int page)
    {
        GameObject[] list = GetButtons(page);
        if (list == null) return;

        foreach (GameObject go in list)
        {
            if (go == null || !go.activeSelf) continue;
            StartButtonRoutine(go, HideRoutine(go));
        }
    }

    private void ShowPageButtons(int page)
    {
        GameObject[] list = GetButtons(page);
        if (list == null) return;

        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == null) continue;
            StartButtonRoutine(list[i], ShowRoutine(list[i], showDelay + i * buttonStagger));
        }
    }

    private void StartButtonRoutine(GameObject go, IEnumerator routine)
    {
        if (buttonRoutines.TryGetValue(go, out Coroutine running) && running != null)
            StopCoroutine(running);
        buttonRoutines[go] = StartCoroutine(routine);
    }

    private IEnumerator ShowRoutine(GameObject go, float delay)
    {
        CanvasGroup cg = GetCanvasGroup(go);
        Transform tr = go.transform;

        // Pehle hidden state me rakho, delay ke baad dikhao
        if (go.activeSelf) { cg.alpha = 0f; }
        else { go.SetActive(true); cg.alpha = 0f; }
        tr.localScale = Vector3.one * 0.6f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        float wait = 0f;
        while (wait < delay) { wait += Time.unscaledDeltaTime; yield return null; }

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / buttonFadeTime;
            float k = Mathf.Clamp01(t);
            cg.alpha = k;
            tr.localScale = Vector3.one * EaseOutBack(k);   // halka pop
            yield return null;
        }

        cg.alpha = 1f;
        tr.localScale = Vector3.one;
        cg.interactable = true;
        cg.blocksRaycasts = true;
        buttonRoutines[go] = null;
    }

    private IEnumerator HideRoutine(GameObject go)
    {
        CanvasGroup cg = GetCanvasGroup(go);
        Transform tr = go.transform;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        float startAlpha = cg.alpha;
        Vector3 startScale = tr.localScale;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / buttonFadeTime;
            float k = Mathf.Clamp01(t);
            cg.alpha = Mathf.Lerp(startAlpha, 0f, k);
            tr.localScale = Vector3.Lerp(startScale, Vector3.one * 0.6f, k);
            yield return null;
        }

        cg.alpha = 0f;
        tr.localScale = Vector3.one;
        go.SetActive(false);
        buttonRoutines[go] = null;
    }

    private static CanvasGroup GetCanvasGroup(GameObject go)
    {
        CanvasGroup cg = go.GetComponent<CanvasGroup>();
        if (cg == null) cg = go.AddComponent<CanvasGroup>();
        return cg;
    }

    // ---------------- Dots ----------------

    private void BuildDots()
    {
        foreach (Image d in dots) if (d != null) Destroy(d.gameObject);
        dots.Clear();
        dotRoutines.Clear();

        for (int i = 0; i < pageCount; i++)
        {
            Image dot = Instantiate(dotPrefab, dotsParent);
            dot.gameObject.SetActive(true);
            int index = i;

            // Dot pe click karke seedha us page par jaane ke liye
            Button b = dot.GetComponent<Button>();
            if (b == null) b = dot.gameObject.AddComponent<Button>();
            b.targetGraphic = dot;
            b.onClick.AddListener(() => GoToPage(index));

            dots.Add(dot);
            dotRoutines.Add(null);
        }
    }

    private void UpdateDotsInstant()
    {
        for (int i = 0; i < dots.Count; i++)
        {
            bool active = i == currentPage;
            dots[i].color = active ? activeColor : inactiveColor;
            dots[i].rectTransform.sizeDelta = active ? activeDotSize : dotSize;
            dots[i].rectTransform.localScale = Vector3.one;
        }
    }

    private void AnimateDot(int index, bool becomeActive)
    {
        if (index < 0 || index >= dots.Count) return;
        if (dotRoutines[index] != null) StopCoroutine(dotRoutines[index]);
        dotRoutines[index] = StartCoroutine(DotRoutine(index, becomeActive));
    }

    private IEnumerator DotRoutine(int index, bool becomeActive)
    {
        Image dot = dots[index];
        RectTransform rt = dot.rectTransform;

        Color fromColor = dot.color;
        Color toColor = becomeActive ? activeColor : inactiveColor;
        Vector2 fromSize = rt.sizeDelta;
        Vector2 toSize = becomeActive ? activeDotSize : dotSize;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / dotAnimTime;
            float k = Mathf.Clamp01(t);
            float eased = EaseInOutCubic(k);

            dot.color = Color.Lerp(fromColor, toColor, eased);
            rt.sizeDelta = Vector2.Lerp(fromSize, toSize, eased);

            // Selected dot par halka "pop" (bounce) effect
            float pop = becomeActive ? 1f + Mathf.Sin(k * Mathf.PI) * 0.35f : 1f;
            rt.localScale = new Vector3(1f, pop, 1f);

            yield return null;
        }

        dot.color = toColor;
        rt.sizeDelta = toSize;
        rt.localScale = Vector3.one;
        dotRoutines[index] = null;
    }

    // ---------------- Swipe ----------------

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartX = eventData.position.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float diff = eventData.position.x - dragStartX;
        if (Mathf.Abs(diff) < swipeThreshold) { SnapBack(); return; }
        if (diff < 0) Next(); else Previous();
    }

    // ---------------- Helpers ----------------

    private static float EaseInOutCubic(float x)
    {
        return x < 0.5f ? 4f * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;
    }

    private static float EaseOutBack(float x)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
}