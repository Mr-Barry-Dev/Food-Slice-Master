using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// GIFT PROGRESS
///
/// - Gift image shuru me BLACK hota hai.
/// - Har level complete hone par original color neeche se upar ki taraf aata jata hai.
/// - Text me total percentage dikhta hai (4 level = 100%).
/// - 100% hone par Spawn Object aata hai, Gift Panel uske andar chala jata hai
///   aur phir New Gift Panel open hota hai.
///
/// Level complete hone par OtherGameManager me GiftProgress.AddLevelProgress() call hota hai.
/// </summary>
public class GiftProgress : MonoBehaviour
{
    // ============================================================
    // SETTINGS
    // ============================================================

    // Default value bhi 4 hai. Inspector me winsRequiredForFullGift badal sakte ho.
    public const int LevelsRequired = 4;

    [Header("===== GIFT PROGRESS SETTINGS =====")]
    [Min(1)]
    [Tooltip("Kitni baar level/game win hone par gift image 100% hogi. Example: 4 = har win par 25%, chauthi win par 100%.")]
    public int winsRequiredForFullGift = 4;

    private static GiftProgress activeInstance;

    private static int GetRequiredLevels()
    {
        return activeInstance != null
            ? Mathf.Max(1, activeInstance.winsRequiredForFullGift)
            : LevelsRequired;
    }

    private const string COUNT_KEY = "GiftLevelsCompleted";
    private const string SHOWN_KEY = "GiftShownLevels";


    // ============================================================
    // GIFT PANEL
    // ============================================================

    [Header("===== GIFT PANEL =====")]

    [Tooltip("Gift image + percentage text wala holder (ROOT panel nahi). 100% par ye spawn object ke andar jayega. Khali chhodo to sirf Gift Image andar jayegi.")]
    public RectTransform giftPanel;

    [Tooltip("Gift ki EK hi image (original color wali, sprite zaroor lagao). Script iske upar black layer khud laga degi, alag black image nahi chahiye.")]
    [FormerlySerializedAs("giftColorImage")]
    public Image giftImage;

    // Black layer (code khud banati hai, upar se hatti hai to neeche se color dikhta hai)
    private Image blackOverlay;

    [Tooltip("Percentage dikhane wala TMP Text.")]
    public TMP_Text percentText;


    // ============================================================
    // SPAWN OBJECT + NEW GIFT PANEL
    // ============================================================

    [Header("===== SPAWN OBJECT =====")]

    [Tooltip("100% hone par ye object spawn hoga (box / chest). Gift panel isi ke andar jayega. Scene me inactive rakho, aur Gift Panel wale Canvas me hi rakho.")]
    public GameObject spawnObject;

    [Header("===== NEW GIFT PANEL =====")]

    [Tooltip("Gift andar jane ke baad ye naya panel open hoga. Scene me inactive rakho (Gift Panel ke andar mat rakhna).")]
    public GameObject newGiftPanel;

    [Tooltip("Optional: New Gift Panel band karne wala button.")]
    public Button closeNewPanelButton;

    [Tooltip("New Gift Panel open hone ke baad ye event chalega. Yahan reward (coins/gems) de sakte ho.")]
    public UnityEvent onGiftOpened;

    [Tooltip("New Gift Panel ka close button dabane par ye event chalega (OtherGameManager isse gift panel band karke win panel wapas kholta hai).")]
    public UnityEvent onNewGiftPanelClosed = new UnityEvent();

    [Tooltip("100% se KAM hone par fill animation ke baad itne seconds ruk kar gift panel apne aap band ho jayega (pick one wala panel nahi khulega).")]
    public float autoCloseDelay = 1f;

    [Tooltip("Gift panel apne aap band hone par ye event chalega (OtherGameManager isse win panel wapas kholta hai).")]
    public UnityEvent onAutoClose = new UnityEvent();


    // ============================================================
    // ANIMATION
    // ============================================================

    [Header("===== ANIMATION =====")]

    [Tooltip("OFF (recommended): panel poora khulne ke BAAD OtherGameManager animation chalata hai. ON: panel active hote hi animation shuru ho jayegi.")]
    public bool autoPlayOnEnable = false;

    [Tooltip("Gift panel poora khulne ke baad percentage + color animation shuru hone ka delay.")]
    public float startDelay = 0.3f;

    [Tooltip("Black se color fill hone ka time.")]
    public float fillDuration = 0.8f;

    [Tooltip("100% hone ke baad spawn object aane se pehle ka delay.")]
    public float openDelay = 0.6f;

    [Tooltip("Spawn object ke pop-in ka time.")]
    public float spawnPopDuration = 0.35f;

    [Tooltip("Gift panel ke spawn object ke andar jane ka time.")]
    public float giftIntoSpawnDuration = 0.6f;

    [Tooltip("New Gift Panel open animation ka time.")]
    public float newPanelDuration = 0.35f;

    [Tooltip("New Gift Panel ka starting scale.")]
    public float newPanelStartScale = 0.7f;


    // ============================================================
    // SOUND
    // ============================================================

    [Header("===== SOUND =====")]

    [Tooltip("New Gift Panel open hone par ye sound bajega.")]
    public AudioClip giftOpenClip;

    [Range(0f, 1f)]
    public float giftOpenVolume = 1f;


    // ============================================================
    // INTERNAL
    // ============================================================

    private Coroutine playRoutine;
    private Coroutine closeRoutine;

    private bool originCached = false;
    private Vector3 originPosition;
    private Vector3 originScale = Vector3.one;

    private Vector3 spawnOriginalScale = Vector3.one;

    // New Gift Panel ki original position aur scale, taaki har baar
    // Spawn Object ke center se usi normal position par open ho.
    private Vector3 newPanelOriginalPosition;
    private Vector3 newPanelOriginalScale = Vector3.one;
    private bool newPanelOriginCached = false;


    // ============================================================
    // STATIC API (OtherGameManager yahi call karta hai)
    // ============================================================

    // Ek level complete hone par call karo
    public static void AddLevelProgress()
    {
        int requiredLevels = GetRequiredLevels();
        int count = GetCount();

        // Pehle wala gift abhi khula nahi to usse 100% par hi rakho
        if (count >= requiredLevels)
            return;

        PlayerPrefs.SetInt(COUNT_KEY, count + 1);
        PlayerPrefs.Save();
    }

    public static int GetCount()
    {
        return Mathf.Clamp(
            PlayerPrefs.GetInt(COUNT_KEY, 0),
            0,
            GetRequiredLevels()
        );
    }

    public static float GetProgress()
    {
        return (float)GetCount() / GetRequiredLevels();
    }


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        activeInstance = this;
        winsRequiredForFullGift = Mathf.Max(1, winsRequiredForFullGift);

        if (giftPanel == null && giftImage != null)
            giftPanel = giftImage.rectTransform;

        SetupImages();

        if (spawnObject != null)
        {
            spawnOriginalScale = spawnObject.transform.localScale;
            spawnObject.SetActive(false);
        }

        if (newGiftPanel != null)
        {
            newPanelOriginalPosition = newGiftPanel.transform.position;
            newPanelOriginalScale = newGiftPanel.transform.localScale;
            newPanelOriginCached = true;
            newGiftPanel.SetActive(false);
        }

        if (closeNewPanelButton != null)
        {
            closeNewPanelButton.onClick.RemoveListener(CloseNewGiftPanel);
            closeNewPanelButton.onClick.AddListener(CloseNewGiftPanel);
        }
    }


    // ============================================================
    // SETUP IMAGES (fill bottom -> top + black copy)
    // ============================================================

    private void SetupImages()
    {
        if (giftImage == null)
        {
            Debug.LogWarning(
                "GiftProgress: Gift Image assign nahi hai."
            );

            return;
        }

        if (giftImage.sprite == null)
        {
            Debug.LogWarning(
                "GiftProgress: Gift Image me Sprite assign nahi hai."
            );
        }

        if (blackOverlay != null)
            return;

        // Original image ko bilkul nahi chhedte.
        // Uske upar usi sprite ki BLACK copy lagate hain (child ke roop me).
        GameObject overlayObject =
            new GameObject(
                "GiftBlackLayer",
                typeof(RectTransform),
                typeof(Image)
            );

        overlayObject.layer = giftImage.gameObject.layer;

        RectTransform overlayRect =
            overlayObject.GetComponent<RectTransform>();

        overlayRect.SetParent(giftImage.transform, false);

        // Poori image ko cover kare
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;
        overlayRect.localScale = Vector3.one;

        // Image ke baaki children (jaise percentage text) black layer ke upar dikhein
        overlayRect.SetAsFirstSibling();

        blackOverlay = overlayObject.GetComponent<Image>();

        blackOverlay.sprite = giftImage.sprite;
        blackOverlay.preserveAspect = giftImage.preserveAspect;
        blackOverlay.raycastTarget = false;

        // Black color, alpha original jaisa (taaki shape same rahe)
        blackOverlay.color =
            new Color(0f, 0f, 0f, giftImage.color.a);

        // Black layer UPAR se hatti hai, isliye neeche se color dikhta hai
        blackOverlay.type = Image.Type.Filled;
        blackOverlay.fillMethod = Image.FillMethod.Vertical;
        blackOverlay.fillOrigin = (int)Image.OriginVertical.Top;
        blackOverlay.fillAmount = 1f;
    }


    // ============================================================
    // ON ENABLE
    // ============================================================

    private void OnEnable()
    {
        ResetVisuals();

        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }

        // Panel khulte waqt purana percentage / color dikhe, animation baad me
        ShowStartState();

        if (autoPlayOnEnable)
            Play();
    }


    // Percentage counting + color fill animation shuru karo
    // (OtherGameManager gift panel poora khulne ke baad ise call karta hai)
    public void Play()
    {
        if (!isActiveAndEnabled)
            return;

        if (playRoutine != null)
            StopCoroutine(playRoutine);

        playRoutine = StartCoroutine(PlayRoutine());
    }


    // Animation se pehle wali state (pichla percentage aur color)
    private int GetAnimationStartCount(int count)
    {
        int shown = Mathf.Clamp(
            PlayerPrefs.GetInt(SHOWN_KEY, 0),
            0,
            count
        );

        // Agar naya level complete hua hai, to animation pichle milestone
        // se shuru ho. Example: count 2 ho to 25% se 50% tak animate kare.
        // Pehle level par startCount 0 hi rahega (0% se 25%).
        if (count > shown)
            shown = Mathf.Max(shown, count - 1);

        return shown;
    }


    // Animation se pehle wali state (pichla percentage aur color)
    private void ShowStartState()
    {
        int count = GetCount();
        int startCount = GetAnimationStartCount(count);

        SetFill((float)startCount / GetRequiredLevels());
    }


    private void OnDisable()
    {
        playRoutine = null;
        closeRoutine = null;
    }


    // Panel dobara khule to sab kuch original state me aa jaye
    private void ResetVisuals()
    {
        if (originCached && giftPanel != null)
        {
            giftPanel.position = originPosition;
            giftPanel.localScale = originScale;
        }

        if (spawnObject != null)
            spawnObject.SetActive(false);

        if (newGiftPanel != null)
        {
            if (newPanelOriginCached)
            {
                newGiftPanel.transform.position = newPanelOriginalPosition;
                newGiftPanel.transform.localScale = newPanelOriginalScale;
            }

            newGiftPanel.SetActive(false);
        }
    }


    // Bahar se bhi call kar sakte ho (jaise panel active hi ho aur level complete ho jaye)
    public void Refresh()
    {
        if (!isActiveAndEnabled)
            return;

        OnEnable();
        Play();
    }


    // ============================================================
    // MAIN ROUTINE: fill animation, phir 100% par gift open
    // ============================================================

    private IEnumerator PlayRoutine()
    {
        int count = GetCount();

        int startCount = GetAnimationStartCount(count);

        int requiredLevels = GetRequiredLevels();
        float from = (float)startCount / requiredLevels;
        float to = (float)count / requiredLevels;

        SetFill(from);

        if (startDelay > 0f)
            yield return new WaitForSecondsRealtime(startDelay);

        if (to > from)
            yield return StartCoroutine(AnimateFill(from, to));

        SetFill(to);

        PlayerPrefs.SetInt(SHOWN_KEY, count);
        PlayerPrefs.Save();

        // 100% ho gaya: spawn object + pick one (New Gift Panel) wala sequence
        if (count >= requiredLevels)
        {
            if (openDelay > 0f)
                yield return new WaitForSecondsRealtime(openDelay);

            yield return StartCoroutine(OpenGiftSequence());
        }
        else
        {
            // 100% nahi hua: pick one panel NAHI khulega.
            // Percentage + color animation ho chuka, ab gift panel apne aap band.
            if (autoCloseDelay > 0f)
                yield return new WaitForSecondsRealtime(autoCloseDelay);

            playRoutine = null;

            if (onAutoClose != null)
                onAutoClose.Invoke();

            yield break;
        }

        playRoutine = null;
    }


    private IEnumerator AnimateFill(float from, float to)
    {
        float duration = Mathf.Max(0.01f, fillDuration);
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(timer / duration);

            SetFill(Mathf.Lerp(from, to, EaseOutCubic(t)));

            yield return null;
        }

        SetFill(to);
    }


    private void SetFill(float value)
    {
        value = Mathf.Clamp01(value);

        // 0% = poori black, 100% = poora original color (neeche se upar)
        if (blackOverlay != null)
            blackOverlay.fillAmount = 1f - value;

        if (percentText != null)
            percentText.text = Mathf.RoundToInt(value * 100f) + "%";
    }


    // ============================================================
    // OPEN SEQUENCE
    // 1) Spawn object aata hai
    // 2) Gift panel uske andar jata hai
    // 3) New gift panel open hota hai
    // ============================================================

    private IEnumerator OpenGiftSequence()
    {
        if (giftPanel == null)
            yield break;

        // Original position/scale yaad rakho (baad me wapas laane ke liye)
        originPosition = giftPanel.position;
        originScale = giftPanel.localScale;
        originCached = true;


        // ----- 1) SPAWN OBJECT POP -----

        if (spawnObject != null)
        {
            spawnObject.SetActive(true);

            yield return StartCoroutine(
                ScaleTransform(
                    spawnObject.transform,
                    Vector3.zero,
                    spawnOriginalScale,
                    spawnPopDuration,
                    EaseOutBack
                )
            );
        }


        // ----- 2) GIFT PANEL ANDAR JAYE -----

        Vector3 startPos = giftPanel.position;
        Vector3 startScale = giftPanel.localScale;

        Vector3 endPos =
            spawnObject != null
                ? spawnObject.transform.position
                : startPos;

        float duration = Mathf.Max(0.01f, giftIntoSpawnDuration);
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(timer / duration);

            // Ease in: dheere shuru, tezi se andar
            float eased = t * t;

            giftPanel.position =
                Vector3.Lerp(startPos, endPos, eased);

            giftPanel.localScale =
                Vector3.Lerp(startScale, Vector3.zero, eased);

            yield return null;
        }

        giftPanel.position = endPos;
        giftPanel.localScale = Vector3.zero;


        // Gift andar gaya, spawn object thoda uchhalta hai
        if (spawnObject != null)
        {
            yield return StartCoroutine(
                ScaleTransform(
                    spawnObject.transform,
                    spawnOriginalScale,
                    spawnOriginalScale * 1.15f,
                    0.12f,
                    EaseOutCubic
                )
            );

            yield return StartCoroutine(
                ScaleTransform(
                    spawnObject.transform,
                    spawnOriginalScale * 1.15f,
                    spawnOriginalScale,
                    0.12f,
                    EaseOutCubic
                )
            );
        }


        // ----- 3) NEW GIFT PANEL OPEN -----
        // Panel Spawn Object ke center se chhota shuru hoga,
        // phir apni original position aur original size tak expand hoga.

        if (newGiftPanel != null)
        {
            newGiftPanel.SetActive(true);

            PlayGiftOpenSound();

            yield return StartCoroutine(AnimateNewGiftPanelFromSpawn());
        }
        else
        {
            Debug.LogWarning(
                "GiftProgress: New Gift Panel assign nahi hai."
            );
        }


        // Gift mil gaya: progress 0 se dobara shuru
        PlayerPrefs.SetInt(COUNT_KEY, 0);
        PlayerPrefs.SetInt(SHOWN_KEY, 0);
        PlayerPrefs.Save();

        if (onGiftOpened != null)
            onGiftOpened.Invoke();
    }


    // New Gift Panel ko Spawn Object ke center se uski normal position tak
    // move + scale karta hai. Isse panel wahi se nikalta hua lagega jahan
    // gift image aur percentage text shrink hokar gaye the.
    private IEnumerator AnimateNewGiftPanelFromSpawn()
    {
        if (newGiftPanel == null)
            yield break;

        Transform panelTransform = newGiftPanel.transform;

        // Safety fallback agar original transform cache nahi hua.
        if (!newPanelOriginCached)
        {
            newPanelOriginalPosition = panelTransform.position;
            newPanelOriginalScale = panelTransform.localScale;
            newPanelOriginCached = true;
        }

        Vector3 endPosition = newPanelOriginalPosition;
        Vector3 endScale = newPanelOriginalScale;
        Vector3 startPosition = spawnObject != null
            ? spawnObject.transform.position
            : endPosition;

        float duration = Mathf.Max(0.01f, newPanelDuration);
        float timer = 0f;

        // Center point se start: scale zero aur position Spawn Object par.
        panelTransform.position = startPosition;
        panelTransform.localScale = Vector3.zero;

        while (timer < duration)
        {
            if (panelTransform == null)
                yield break;

            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / duration);
            float eased = EaseOutCubic(t);

            panelTransform.position = Vector3.Lerp(startPosition, endPosition, eased);
            panelTransform.localScale = Vector3.LerpUnclamped(Vector3.zero, endScale, eased);

            yield return null;
        }

        if (panelTransform != null)
        {
            panelTransform.position = endPosition;
            panelTransform.localScale = endScale;
        }
    }


    // ============================================================
    // CLOSE NEW GIFT PANEL
    // ============================================================

    public void CloseNewGiftPanel()
    {
        if (closeRoutine != null)
            return;

        if (!isActiveAndEnabled)
            return;

        closeRoutine = StartCoroutine(CloseRoutine());
    }


    private IEnumerator CloseRoutine()
    {
        if (newGiftPanel != null && newGiftPanel.activeSelf)
        {
            yield return StartCoroutine(
                ScaleTransform(
                    newGiftPanel.transform,
                    newGiftPanel.transform.localScale,
                    Vector3.one * newPanelStartScale,
                    newPanelDuration * 0.7f,
                    EaseOutCubic
                )
            );

            newGiftPanel.SetActive(false);
        }

        if (spawnObject != null)
            spawnObject.SetActive(false);

        // Gift panel wapas, ab 0% black gift ke saath
        if (originCached && giftPanel != null)
        {
            giftPanel.position = originPosition;
            giftPanel.localScale = originScale;
        }

        SetFill(0f);

        closeRoutine = null;

        // Gift panel band karke win panel wapas kholne ke liye
        if (onNewGiftPanelClosed != null)
            onNewGiftPanelClosed.Invoke();
    }


    // ============================================================
    // HELPERS
    // ============================================================

    private void PlayGiftOpenSound()
    {
        if (giftOpenClip == null)
            return;

        // Settings me Sound OFF hai to kuch na bajao
        if (!GameAudio.SoundEnabled)
            return;

        if (Camera.main == null)
            return;

        AudioSource.PlayClipAtPoint(
            giftOpenClip,
            Camera.main.transform.position,
            giftOpenVolume
        );
    }


    private delegate float EaseFunction(float t);


    private IEnumerator ScaleTransform(
        Transform target,
        Vector3 from,
        Vector3 to,
        float duration,
        EaseFunction ease)
    {
        if (target == null)
            yield break;

        duration = Mathf.Max(0.01f, duration);

        float timer = 0f;

        target.localScale = from;

        while (timer < duration)
        {
            if (target == null)
                yield break;

            timer += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(timer / duration);

            target.localScale =
                Vector3.LerpUnclamped(from, to, ease(t));

            yield return null;
        }

        if (target != null)
            target.localScale = to;
    }


    private static float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }


    // Thoda overshoot karke settle hota hai (pop effect)
    private static float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;

        return 1f +
               c3 * Mathf.Pow(t - 1f, 3f) +
               c1 * Mathf.Pow(t - 1f, 2f);
    }


    // ============================================================
    // ON DESTROY
    // ============================================================

    private void OnDestroy()
    {
        if (closeNewPanelButton != null)
            closeNewPanelButton.onClick.RemoveListener(CloseNewGiftPanel);

        if (activeInstance == this)
            activeInstance = null;
    }
}