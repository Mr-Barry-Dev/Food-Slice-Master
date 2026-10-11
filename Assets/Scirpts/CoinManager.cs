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
    // REFRESH TEXTS
    // ============================================================

    public void RefreshAllTexts()
    {
        string coinString = coinPrefix + GetCoins() + coinSuffix;
        string gemString = gemPrefix + GetGems() + gemSuffix;

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