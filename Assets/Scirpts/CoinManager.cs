// using System.Globalization;
// using UnityEngine;
// using TMPro;

// public class CoinManager : MonoBehaviour
// {
//     // ============================================================
//     // SAVE KEYS
//     // ============================================================

//     private const string COINS_KEY = "TotalCoins";
//     private const string GEMS_KEY = "TotalGems";


//     // ============================================================
//     // TEXT ARRAYS
//     // ============================================================

//     [Header("===== COIN TEXTS =====")]

//     [Tooltip("Total coins yahan show honge. Multiple panels ke texts add kar sakte ho.")]
//     public TMP_Text[] coinTexts;


//     [Header("===== GEM TEXTS =====")]

//     [Tooltip("Total gems yahan show honge. Multiple panels ke texts add kar sakte ho.")]
//     public TMP_Text[] gemTexts;


//     // ============================================================
//     // TEXT FORMAT
//     // ============================================================

//     [Header("===== TEXT FORMAT =====")]

//     [Tooltip("Number se pehle ka text. Example: 'Coins: '")]
//     public string coinPrefix = "";

//     [Tooltip("Number ke baad ka text.")]
//     public string coinSuffix = "";

//     public string gemPrefix = "";

//     public string gemSuffix = "";


//     // ============================================================
//     // SHORT NUMBER FORMAT (K, M, B)
//     // ============================================================

//     [Header("===== SHORT NUMBER FORMAT =====")]

//     [Tooltip("On rakho to 1500 -> 1.5K, 2500000 -> 2.5M, 1200000000 -> 1.2B dikhega.")]
//     public bool useShortFormat = true;

//     [Tooltip("Decimal ke baad kitne digits dikhane hain (1.25K me 2).")]
//     [Range(0, 3)]
//     public int decimalPlaces = 2;


//     // ============================================================
//     // INSTANCE
//     // ============================================================

//     // Har scene ka apna CoinManager hota hai (us scene ke texts ke saath).
//     // Data PlayerPrefs me save hota hai, isliye scenes ke beech share hota hai.
//     public static CoinManager Instance { get; private set; }


//     // ============================================================
//     // UNITY
//     // ============================================================

//     private void Awake()
//     {
//         Instance = this;
//     }

//     private void Start()
//     {
//         RefreshAllTexts();
//     }

//     private void OnEnable()
//     {
//         RefreshAllTexts();
//     }

//     private void OnDestroy()
//     {
//         if (Instance == this)
//         {
//             Instance = null;
//         }
//     }


//     // ============================================================
//     // STATIC API (kisi bhi scene se call kar sakte ho)
//     // ============================================================

//     public static int GetCoins()
//     {
//         return PlayerPrefs.GetInt(COINS_KEY, 0);
//     }

//     public static int GetGems()
//     {
//         return PlayerPrefs.GetInt(GEMS_KEY, 0);
//     }

//     public static void AddCoins(int amount)
//     {
//         if (amount <= 0)
//             return;

//         PlayerPrefs.SetInt(COINS_KEY, GetCoins() + amount);
//         PlayerPrefs.Save();

//         if (Instance != null)
//         {
//             Instance.RefreshAllTexts();
//         }
//     }

//     public static void AddGems(int amount)
//     {
//         if (amount <= 0)
//             return;

//         PlayerPrefs.SetInt(GEMS_KEY, GetGems() + amount);
//         PlayerPrefs.Save();

//         if (Instance != null)
//         {
//             Instance.RefreshAllTexts();
//         }
//     }

//     public static bool SpendCoins(int amount)
//     {
//         if (amount <= 0 || GetCoins() < amount)
//             return false;

//         PlayerPrefs.SetInt(COINS_KEY, GetCoins() - amount);
//         PlayerPrefs.Save();

//         if (Instance != null)
//         {
//             Instance.RefreshAllTexts();
//         }

//         return true;
//     }

//     public static bool SpendGems(int amount)
//     {
//         if (amount <= 0 || GetGems() < amount)
//             return false;

//         PlayerPrefs.SetInt(GEMS_KEY, GetGems() - amount);
//         PlayerPrefs.Save();

//         if (Instance != null)
//         {
//             Instance.RefreshAllTexts();
//         }

//         return true;
//     }


//     // ============================================================
//     // NUMBER FORMATTING
//     // ============================================================

//     private string FormatForDisplay(int value)
//     {
//         if (!useShortFormat)
//             return value.ToString();

//         return FormatNumber(value, decimalPlaces);
//     }

//     // Kisi bhi script se use kar sakte ho:
//     // CoinManager.FormatNumber(1500)  ->  "1.5K"
//     public static string FormatNumber(long value, int decimals = 2)
//     {
//         if (value < 0)
//             return "-" + FormatNumber(-value, decimals);

//         if (value < 1000)
//             return value.ToString();

//         double divisor;
//         string suffix;

//         if (value >= 1000000000L)
//         {
//             divisor = 1000000000d;
//             suffix = "B";
//         }
//         else if (value >= 1000000L)
//         {
//             divisor = 1000000d;
//             suffix = "M";
//         }
//         else
//         {
//             divisor = 1000d;
//             suffix = "K";
//         }

//         decimals = Mathf.Clamp(decimals, 0, 3);

//         double scaled = value / divisor;

//         // Round nahi, neeche truncate karte hain (999999 -> 999.99K, 1000K nahi)
//         double factor = System.Math.Pow(10, decimals);
//         scaled = System.Math.Floor(scaled * factor) / factor;

//         string format = decimals == 0
//             ? "0"
//             : "0." + new string('#', decimals);

//         return scaled.ToString(format, CultureInfo.InvariantCulture) + suffix;
//     }


//     // ============================================================
//     // REFRESH TEXTS
//     // ============================================================

//     public void RefreshAllTexts()
//     {
//         string coinString =
//             coinPrefix + FormatForDisplay(GetCoins()) + coinSuffix;

//         string gemString =
//             gemPrefix + FormatForDisplay(GetGems()) + gemSuffix;

//         if (coinTexts != null)
//         {
//             foreach (TMP_Text t in coinTexts)
//             {
//                 if (t != null)
//                 {
//                     t.text = coinString;
//                 }
//             }
//         }

//         if (gemTexts != null)
//         {
//             foreach (TMP_Text t in gemTexts)
//             {
//                 if (t != null)
//                 {
//                     t.text = gemString;
//                 }
//             }
//         }
//     }


//     // ============================================================
//     // TESTING (Inspector me component ke 3-dot menu se chalao)
//     // ============================================================

//     [ContextMenu("Add 100 Coins (Test)")]
//     private void TestAddCoins()
//     {
//         AddCoins(100);
//     }

//     [ContextMenu("Add 50 Gems (Test)")]
//     private void TestAddGems()
//     {
//         AddGems(50);
//     }

//     [ContextMenu("Reset Coins And Gems")]
//     private void ResetAll()
//     {
//         PlayerPrefs.DeleteKey(COINS_KEY);
//         PlayerPrefs.DeleteKey(GEMS_KEY);
//         PlayerPrefs.Save();
//         RefreshAllTexts();
//     }
// }



using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    // ============================================================
    // SAVE KEYS - EXISTING DATA PRESERVED
    // ============================================================

    private const string COINS_KEY = "TotalCoins";
    private const string GEMS_KEY = "TotalGems";


    // ============================================================
    // COIN TEXTS
    // ============================================================

    [Header("===== COIN TEXTS =====")]

    [Tooltip("Game Over, Win, Gift etc. ke sabhi coin TMP texts assign karo.")]
    public TMP_Text[] coinTexts;


    // ============================================================
    // GEM TEXTS
    // ============================================================

    [Header("===== GEM TEXTS =====")]

    [Tooltip("Game Over, Win, Gift etc. ke sabhi gem TMP texts assign karo.")]
    public TMP_Text[] gemTexts;


    // ============================================================
    // COIN AND GEM ICONS
    // ============================================================

    [Header("===== COIN ICONS =====")]

    [Tooltip("Coin icons ke RectTransform ya Transform yahan assign karo.")]
    public Transform[] coinIcons;


    [Header("===== GEM ICONS =====")]

    [Tooltip("Gem icons ke RectTransform ya Transform yahan assign karo.")]
    public Transform[] gemIcons;


    // ============================================================
    // TEXT FORMAT
    // ============================================================

    [Header("===== TEXT FORMAT =====")]

    public string coinPrefix = "";
    public string coinSuffix = "";

    public string gemPrefix = "";
    public string gemSuffix = "";


    // ============================================================
    // SHORT NUMBER FORMAT
    // ============================================================

    [Header("===== SHORT NUMBER FORMAT =====")]

    [Tooltip("1500 = 1.5K, 2500000 = 2.5M")]
    public bool useShortFormat = true;

    [Range(0, 3)]
    public int decimalPlaces = 2;


    // ============================================================
    // ANIMATION SETTINGS
    // ============================================================

    [Header("===== CURRENCY ANIMATION =====")]

    [Tooltip("Number counting animation duration.")]
    [Min(0.01f)]
    public float animationDuration = 0.5f;

    [Tooltip("Animation complete hone par text kitna bada hoga.")]
    [Min(1f)]
    public float textPopMultiplier = 1.18f;

    [Tooltip("Text pop effect ki duration.")]
    [Min(0.01f)]
    public float textPopDuration = 0.12f;

    [Tooltip("Coin aur gem icon bounce kitna bada hoga.")]
    [Min(1f)]
    public float iconBounceMultiplier = 1.22f;

    [Tooltip("Coin aur gem icon bounce ki duration.")]
    [Min(0.01f)]
    public float iconBounceDuration = 0.25f;


    // ============================================================
    // INSTANCE AND MULTIPLE SCENE SUPPORT
    // ============================================================

    public static CoinManager Instance { get; private set; }

    private static readonly List<CoinManager> activeManagers =
        new List<CoinManager>();


    // ============================================================
    // ANIMATION TRACKING
    // ============================================================

    private readonly Dictionary<TMP_Text, long> displayedValues =
        new Dictionary<TMP_Text, long>();

    private readonly Dictionary<TMP_Text, Coroutine> textCoroutines =
        new Dictionary<TMP_Text, Coroutine>();

    private readonly Dictionary<TMP_Text, Vector3> originalTextScales =
        new Dictionary<TMP_Text, Vector3>();

    private readonly Dictionary<Transform, Coroutine> iconCoroutines =
        new Dictionary<Transform, Coroutine>();

    private readonly Dictionary<Transform, Vector3> originalIconScales =
        new Dictionary<Transform, Vector3>();


    // ============================================================
    // UNITY LIFECYCLE
    // ============================================================

    private void Awake()
    {
        RegisterManager();
    }

    private void OnEnable()
    {
        RegisterManager();
        RefreshAllTexts();
    }

    private void Start()
    {
        RefreshAllTexts();
    }

    private void OnDisable()
    {
        UnregisterManager();
        StopAllAnimations();
    }

    private void OnDestroy()
    {
        UnregisterManager();
        StopAllAnimations();
    }


    private void RegisterManager()
    {
        if (!activeManagers.Contains(this))
            activeManagers.Add(this);

        Instance = this;
    }

    private void UnregisterManager()
    {
        activeManagers.Remove(this);

        if (Instance == this)
        {
            Instance = activeManagers.Count > 0
                ? activeManagers[activeManagers.Count - 1]
                : null;
        }
    }


    // ============================================================
    // GET CURRENCY
    // ============================================================

    public static int GetCoins()
    {
        return PlayerPrefs.GetInt(COINS_KEY, 0);
    }

    public static int GetGems()
    {
        return PlayerPrefs.GetInt(GEMS_KEY, 0);
    }


    // ============================================================
    // ADD COINS
    // ============================================================

    public static void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        long total = (long)GetCoins() + amount;

        int safeTotal = (int)System.Math.Min(total, int.MaxValue);

        PlayerPrefs.SetInt(COINS_KEY, safeTotal);
        PlayerPrefs.Save();

        RefreshAllManagers(true, false);
    }


    // ============================================================
    // ADD GEMS
    // ============================================================

    public static void AddGems(int amount)
    {
        if (amount <= 0)
            return;

        long total = (long)GetGems() + amount;

        int safeTotal = (int)System.Math.Min(total, int.MaxValue);

        PlayerPrefs.SetInt(GEMS_KEY, safeTotal);
        PlayerPrefs.Save();

        RefreshAllManagers(false, true);
    }


    // ============================================================
    // SPEND COINS
    // ============================================================

    public static bool SpendCoins(int amount)
    {
        if (amount <= 0 || GetCoins() < amount)
            return false;

        PlayerPrefs.SetInt(COINS_KEY, GetCoins() - amount);
        PlayerPrefs.Save();

        RefreshAllManagers(true, false);

        return true;
    }


    // ============================================================
    // SPEND GEMS
    // ============================================================

    public static bool SpendGems(int amount)
    {
        if (amount <= 0 || GetGems() < amount)
            return false;

        PlayerPrefs.SetInt(GEMS_KEY, GetGems() - amount);
        PlayerPrefs.Save();

        RefreshAllManagers(false, true);

        return true;
    }


    // ============================================================
    // REFRESH ALL ACTIVE SCENE MANAGERS
    // ============================================================

    private static void RefreshAllManagers(
        bool animateCoins,
        bool animateGems)
    {
        // Loop ke dauran manager disable ho sakta hai,
        // isliye active managers ki copy use karte hain.
        CoinManager[] managers = activeManagers.ToArray();

        foreach (CoinManager manager in managers)
        {
            if (manager != null && manager.isActiveAndEnabled)
            {
                manager.RefreshCurrencyDisplay(
                    animateCoins,
                    animateGems
                );
            }
        }
    }


    // ============================================================
    // PUBLIC REFRESH - IMMEDIATE UPDATE
    // ============================================================

    public void RefreshAllTexts()
    {
        RefreshCurrencyDisplay(false, false);
    }


    // ============================================================
    // UPDATE CURRENCY DISPLAY
    // ============================================================

    private void RefreshCurrencyDisplay(
        bool animateCoins,
        bool animateGems)
    {
        RefreshTextArray(
            coinTexts,
            GetCoins(),
            coinPrefix,
            coinSuffix,
            animateCoins
        );

        RefreshTextArray(
            gemTexts,
            GetGems(),
            gemPrefix,
            gemSuffix,
            animateGems
        );

        if (animateCoins)
            BounceIcons(coinIcons);

        if (animateGems)
            BounceIcons(gemIcons);
    }


    private void RefreshTextArray(
        TMP_Text[] texts,
        int targetValue,
        string prefix,
        string suffix,
        bool animate)
    {
        if (texts == null)
            return;

        foreach (TMP_Text text in texts)
        {
            if (text == null)
                continue;

            SaveOriginalTextScale(text);

            if (!animate || !displayedValues.ContainsKey(text))
            {
                SetTextImmediately(
                    text,
                    targetValue,
                    prefix,
                    suffix
                );
            }
            else
            {
                StartTextCountAnimation(
                    text,
                    displayedValues[text],
                    targetValue,
                    prefix,
                    suffix
                );
            }
        }
    }


    // ============================================================
    // TEXT COUNTING ANIMATION
    // ============================================================

    private void StartTextCountAnimation(
        TMP_Text text,
        long startValue,
        long targetValue,
        string prefix,
        string suffix)
    {
        if (textCoroutines.TryGetValue(text, out Coroutine running))
        {
            if (running != null)
                StopCoroutine(running);

            textCoroutines.Remove(text);
        }

        // Agar animation ke beech naya reward aaye,
        // toh current text scale normal kar do.
        RestoreTextScale(text);

        if (animationDuration <= 0f || startValue == targetValue)
        {
            SetTextImmediately(
                text,
                targetValue,
                prefix,
                suffix
            );

            PopText(text);
            return;
        }

        Coroutine coroutine = StartCoroutine(
            CountTextRoutine(
                text,
                startValue,
                targetValue,
                prefix,
                suffix
            )
        );

        textCoroutines[text] = coroutine;
    }


    private IEnumerator CountTextRoutine(
        TMP_Text text,
        long startValue,
        long targetValue,
        string prefix,
        string suffix)
    {
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, animationDuration);

        while (elapsed < duration)
        {
            if (text == null)
                yield break;

            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

            // Smooth counting.
            float easedProgress = progress * progress *
                (3f - 2f * progress);

            double currentValue =
                startValue +
                ((double)targetValue - startValue) * easedProgress;

            long displayedValue = (long)System.Math.Round(currentValue);

            displayedValues[text] = displayedValue;

            text.text = prefix +
                FormatForDisplay(displayedValue) +
                suffix;

            yield return null;
        }

        if (text != null)
        {
            displayedValues[text] = targetValue;

            text.text = prefix +
                FormatForDisplay(targetValue) +
                suffix;

            // Counting ke baad halka sa text pop.
            yield return PopTextRoutine(text);
        }

        if (text != null)
            textCoroutines.Remove(text);
    }


    // ============================================================
    // TEXT POP ANIMATION
    // ============================================================

    private void PopText(TMP_Text text)
    {
        if (text == null)
            return;

        StartCoroutine(PopTextRoutine(text));
    }


    private IEnumerator PopTextRoutine(TMP_Text text)
    {
        if (text == null)
            yield break;

        SaveOriginalTextScale(text);

        Transform target = text.transform;
        Vector3 originalScale = originalTextScales[text];

        float duration = Mathf.Max(0.01f, textPopDuration);
        float multiplier = Mathf.Max(1f, textPopMultiplier);

        float halfDuration = duration * 0.5f;
        float elapsed = 0f;

        while (elapsed < halfDuration)
        {
            if (text == null)
                yield break;

            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / halfDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            target.localScale = Vector3.Lerp(
                originalScale,
                originalScale * multiplier,
                t
            );

            yield return null;
        }

        elapsed = 0f;

        while (elapsed < halfDuration)
        {
            if (text == null)
                yield break;

            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / halfDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            target.localScale = Vector3.Lerp(
                originalScale * multiplier,
                originalScale,
                t
            );

            yield return null;
        }

        if (text != null)
            target.localScale = originalScale;
    }


    // ============================================================
    // ICON BOUNCE ANIMATION
    // ============================================================

    private void BounceIcons(Transform[] icons)
    {
        if (icons == null)
            return;

        foreach (Transform icon in icons)
        {
            if (icon == null)
                continue;

            SaveOriginalIconScale(icon);

            if (iconCoroutines.TryGetValue(
                icon,
                out Coroutine running))
            {
                if (running != null)
                    StopCoroutine(running);

                iconCoroutines.Remove(icon);
            }

            Coroutine coroutine = StartCoroutine(
                IconBounceRoutine(icon)
            );

            iconCoroutines[icon] = coroutine;
        }
    }


    private IEnumerator IconBounceRoutine(Transform icon)
    {
        if (icon == null)
            yield break;

        Vector3 originalScale = originalIconScales[icon];

        float duration = Mathf.Max(0.01f, iconBounceDuration);
        float halfDuration = duration * 0.5f;
        float multiplier = Mathf.Max(1f, iconBounceMultiplier);

        float elapsed = 0f;

        while (elapsed < halfDuration)
        {
            if (icon == null)
                yield break;

            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / halfDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            icon.localScale = Vector3.Lerp(
                originalScale,
                originalScale * multiplier,
                t
            );

            yield return null;
        }

        elapsed = 0f;

        while (elapsed < halfDuration)
        {
            if (icon == null)
                yield break;

            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / halfDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            icon.localScale = Vector3.Lerp(
                originalScale * multiplier,
                originalScale,
                t
            );

            yield return null;
        }

        if (icon != null)
        {
            icon.localScale = originalScale;
            iconCoroutines.Remove(icon);
        }
    }


    // ============================================================
    // SCALE HELPERS
    // ============================================================

    private void SaveOriginalTextScale(TMP_Text text)
    {
        if (text == null)
            return;

        if (!originalTextScales.ContainsKey(text))
            originalTextScales[text] = text.transform.localScale;
    }


    private void RestoreTextScale(TMP_Text text)
    {
        if (text == null)
            return;

        if (originalTextScales.TryGetValue(
            text,
            out Vector3 originalScale))
        {
            text.transform.localScale = originalScale;
        }
    }


    private void SaveOriginalIconScale(Transform icon)
    {
        if (icon == null)
            return;

        if (!originalIconScales.ContainsKey(icon))
            originalIconScales[icon] = icon.localScale;
    }


    // ============================================================
    // IMMEDIATE TEXT UPDATE
    // ============================================================

    private void SetTextImmediately(
        TMP_Text text,
        long value,
        string prefix,
        string suffix)
    {
        if (text == null)
            return;

        if (textCoroutines.TryGetValue(
            text,
            out Coroutine running))
        {
            if (running != null)
                StopCoroutine(running);

            textCoroutines.Remove(text);
        }

        RestoreTextScale(text);

        displayedValues[text] = value;

        text.text = prefix +
            FormatForDisplay(value) +
            suffix;
    }


    // ============================================================
    // STOP ANIMATIONS
    // ============================================================

    private void StopAllAnimations()
    {
        foreach (KeyValuePair<TMP_Text, Coroutine> pair in textCoroutines)
        {
            if (pair.Value != null)
                StopCoroutine(pair.Value);
        }

        foreach (KeyValuePair<Transform, Coroutine> pair in iconCoroutines)
        {
            if (pair.Value != null)
                StopCoroutine(pair.Value);
        }

        foreach (KeyValuePair<TMP_Text, Vector3> pair in originalTextScales)
        {
            if (pair.Key != null)
                pair.Key.transform.localScale = pair.Value;
        }

        foreach (KeyValuePair<Transform, Vector3> pair in originalIconScales)
        {
            if (pair.Key != null)
                pair.Key.localScale = pair.Value;
        }

        textCoroutines.Clear();
        iconCoroutines.Clear();
    }


    // ============================================================
    // NUMBER FORMATTING - K, M, B
    // ============================================================

    private string FormatForDisplay(long value)
    {
        if (!useShortFormat)
            return value.ToString(CultureInfo.InvariantCulture);

        return FormatNumber(value, decimalPlaces);
    }


    public static string FormatNumber(long value, int decimals = 2)
    {
        if (value < 0)
            return "-" + FormatNumber(-value, decimals);

        if (value < 1000)
            return value.ToString(CultureInfo.InvariantCulture);

        double divisor;
        string suffix;

        if (value >= 1000000000L)
        {
            divisor = 1000000000d;
            suffix = "B";
        }
        else if (value >= 1000000L)
        {
            divisor = 1000000d;
            suffix = "M";
        }
        else
        {
            divisor = 1000d;
            suffix = "K";
        }

        decimals = Mathf.Clamp(decimals, 0, 3);

        double scaled = value / divisor;

        double factor = System.Math.Pow(10, decimals);

        // Existing truncation behaviour preserved.
        scaled = System.Math.Floor(scaled * factor) / factor;

        string format = decimals == 0
            ? "0"
            : "0." + new string('#', decimals);

        return scaled.ToString(
            format,
            CultureInfo.InvariantCulture
        ) + suffix;
    }


    // ============================================================
    // TESTING
    // ============================================================

    [ContextMenu("Add 100 Coins (Test)")]
    private void TestAddCoins()
    {
        AddCoins(100);
    }


    [ContextMenu("Add 50 Gems (Test)")]
    private void TestAddGems()
    {
        AddGems(50);
    }


    [ContextMenu("Reset Coins And Gems")]
    private void ResetAll()
    {
        PlayerPrefs.DeleteKey(COINS_KEY);
        PlayerPrefs.DeleteKey(GEMS_KEY);
        PlayerPrefs.Save();

        // Reset ke baad animation nahi, direct refresh.
        CoinManager[] managers = activeManagers.ToArray();

        foreach (CoinManager manager in managers)
        {
            if (manager != null)
                manager.RefreshAllTexts();
        }
    }
}