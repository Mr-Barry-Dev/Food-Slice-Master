using System.Globalization;
using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    // ============================================================
    // SAVE KEYS
    // ============================================================

    private const string COINS_KEY = "TotalCoins";
    private const string GEMS_KEY = "TotalGems";


    // ============================================================
    // TEXT ARRAYS
    // ============================================================

    [Header("===== COIN TEXTS =====")]

    [Tooltip("Total coins yahan show honge. Multiple panels ke texts add kar sakte ho.")]
    public TMP_Text[] coinTexts;


    [Header("===== GEM TEXTS =====")]

    [Tooltip("Total gems yahan show honge. Multiple panels ke texts add kar sakte ho.")]
    public TMP_Text[] gemTexts;


    // ============================================================
    // TEXT FORMAT
    // ============================================================

    [Header("===== TEXT FORMAT =====")]

    [Tooltip("Number se pehle ka text. Example: 'Coins: '")]
    public string coinPrefix = "";

    [Tooltip("Number ke baad ka text.")]
    public string coinSuffix = "";

    public string gemPrefix = "";

    public string gemSuffix = "";


    // ============================================================
    // SHORT NUMBER FORMAT (K, M, B)
    // ============================================================

    [Header("===== SHORT NUMBER FORMAT =====")]

    [Tooltip("On rakho to 1500 -> 1.5K, 2500000 -> 2.5M, 1200000000 -> 1.2B dikhega.")]
    public bool useShortFormat = true;

    [Tooltip("Decimal ke baad kitne digits dikhane hain (1.25K me 2).")]
    [Range(0, 3)]
    public int decimalPlaces = 2;


    // ============================================================
    // INSTANCE
    // ============================================================

    // Har scene ka apna CoinManager hota hai (us scene ke texts ke saath).
    // Data PlayerPrefs me save hota hai, isliye scenes ke beech share hota hai.
    public static CoinManager Instance { get; private set; }


    // ============================================================
    // UNITY
    // ============================================================

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshAllTexts();
    }

    private void OnEnable()
    {
        RefreshAllTexts();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


    // ============================================================
    // STATIC API (kisi bhi scene se call kar sakte ho)
    // ============================================================

    public static int GetCoins()
    {
        return PlayerPrefs.GetInt(COINS_KEY, 0);
    }

    public static int GetGems()
    {
        return PlayerPrefs.GetInt(GEMS_KEY, 0);
    }

    public static void AddCoins(int amount)
    {
        if (amount <= 0)
            return;

        PlayerPrefs.SetInt(COINS_KEY, GetCoins() + amount);
        PlayerPrefs.Save();

        if (Instance != null)
        {
            Instance.RefreshAllTexts();
        }
    }

    public static void AddGems(int amount)
    {
        if (amount <= 0)
            return;

        PlayerPrefs.SetInt(GEMS_KEY, GetGems() + amount);
        PlayerPrefs.Save();

        if (Instance != null)
        {
            Instance.RefreshAllTexts();
        }
    }

    public static bool SpendCoins(int amount)
    {
        if (amount <= 0 || GetCoins() < amount)
            return false;

        PlayerPrefs.SetInt(COINS_KEY, GetCoins() - amount);
        PlayerPrefs.Save();

        if (Instance != null)
        {
            Instance.RefreshAllTexts();
        }

        return true;
    }

    public static bool SpendGems(int amount)
    {
        if (amount <= 0 || GetGems() < amount)
            return false;

        PlayerPrefs.SetInt(GEMS_KEY, GetGems() - amount);
        PlayerPrefs.Save();

        if (Instance != null)
        {
            Instance.RefreshAllTexts();
        }

        return true;
    }


    // ============================================================
    // NUMBER FORMATTING
    // ============================================================

    private string FormatForDisplay(int value)
    {
        if (!useShortFormat)
            return value.ToString();

        return FormatNumber(value, decimalPlaces);
    }

    // Kisi bhi script se use kar sakte ho:
    // CoinManager.FormatNumber(1500)  ->  "1.5K"
    public static string FormatNumber(long value, int decimals = 2)
    {
        if (value < 0)
            return "-" + FormatNumber(-value, decimals);

        if (value < 1000)
            return value.ToString();

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

        // Round nahi, neeche truncate karte hain (999999 -> 999.99K, 1000K nahi)
        double factor = System.Math.Pow(10, decimals);
        scaled = System.Math.Floor(scaled * factor) / factor;

        string format = decimals == 0
            ? "0"
            : "0." + new string('#', decimals);

        return scaled.ToString(format, CultureInfo.InvariantCulture) + suffix;
    }


    // ============================================================
    // REFRESH TEXTS
    // ============================================================

    public void RefreshAllTexts()
    {
        string coinString =
            coinPrefix + FormatForDisplay(GetCoins()) + coinSuffix;

        string gemString =
            gemPrefix + FormatForDisplay(GetGems()) + gemSuffix;

        if (coinTexts != null)
        {
            foreach (TMP_Text t in coinTexts)
            {
                if (t != null)
                {
                    t.text = coinString;
                }
            }
        }

        if (gemTexts != null)
        {
            foreach (TMP_Text t in gemTexts)
            {
                if (t != null)
                {
                    t.text = gemString;
                }
            }
        }
    }


    // ============================================================
    // TESTING (Inspector me component ke 3-dot menu se chalao)
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
        RefreshAllTexts();
    }
}