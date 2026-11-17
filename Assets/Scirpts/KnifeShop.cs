// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;

// /// <summary>
// /// Knife Shop - EK PAGE KE LIYE. Ye script har page par alag se lagao.
// ///
// /// Har page par Inspector me ye set karo:
// ///   - pageIndex   : page 1 = 0, page 2 = 1, page 3 = 2 ...  (har page ka alag, warna save mix ho jayega)
// ///   - knifeSprites: sirf is page ki knives (max 9)
// ///   - coinCost / gemCost : is page ka random price (gemCost 0 = gems check nahi hoga)
// ///
// /// Random button : CoinManager se coins/gems check -> amount kaato -> random roll
// ///                 -> sirf is page ki LOCKED knives me se ek unlock.
// /// Knife touch   : owned ho to equip. Equip poore game me ek hi hota hai,
// ///                 isliye dusre pages ki knife equip hote hi unka look khud update ho jata hai.
// /// Look          : Locked = icon poora BLACK | Owned (equip nahi) = thoda FADE | Equipped = poora CLEAR
// ///
// /// Total coins/gems ke TMP texts CoinManager ke coinTexts / gemTexts me daalo.
// /// </summary>
// public class KnifeShop : MonoBehaviour
// {
//     [Serializable]
//     public class KnifeSlot
//     {
//         public Button button;      // knife ka button
//         public Image knifeIcon;    // knife ki image
//         public Image background;   // slot ka background
//     }

//     [Header("Page Setup (har page par alag)")]
//     [Tooltip("Page 1 = 0, Page 2 = 1, Page 3 = 2 ... Har page ka alag number rakho.")]
//     public int pageIndex = 0;
//     [Tooltip("Sirf is page ki knives ki sprites (slots se zyada nahi).")]
//     public Sprite[] knifeSprites;
//     [Tooltip("Sabse pehli knife (page 1 ki pehli) free milegi.")]
//     public bool firstKnifeFree = true;

//     [Header("Price (is page ka)")]
//     public int coinCost = 500;
//     [Tooltip("0 = gems check nahi hoga. 0 se zyada = gems bhi lagenge.")]
//     public int gemCost = 0;

//     [Header("UI - Slots (9 slots)")]
//     public KnifeSlot[] slots = new KnifeSlot[9];

//     [Header("UI - Random Button / Texts")]
//     public Button randomButton;
//     public TMP_Text costCoinsText;     // random button par coin price (optional)
//     public TMP_Text costGemsText;      // random button par gem price (optional)
//     public GameObject costGemsRoot;    // gem price icon+text, gemCost 0 ho to chhup jayega (optional)
//     public TMP_Text messageText;       // "Coins kam hain" jaise messages (optional)

//     [Header("Look")]
//     [Range(0f, 1f)] public float fadedAlpha = 0.5f;   // owned par equip nahi wali knife kitni fade
//     public Color lockedBackground = new Color(0.30f, 0.30f, 0.30f, 1f);
//     public Color ownedBackground  = new Color(0.30f, 0.65f, 1.00f, 1f);
//     public Color rollHighlight    = Color.white;

//     [Header("Random Roll")]
//     public int rollSteps = 14;             // roll me highlight kitni baar ghumegi
//     public float rollStartDelay = 0.05f;   // shuru me tez
//     public float rollEndDelay = 0.25f;     // end me dheemi

//     // ---------------- Reward Button (coins / gems add) ----------------
//     public enum RewardType
//     {
//         Coins,   // sirf coins
//         Gems,    // sirf gems
//         Both     // coins aur gems dono
//     }

//     [Header("Reward Button (coins / gems add karne ke liye)")]
//     [Tooltip("Optional. Khali chhodo to reward button nahi chalega.")]
//     public Button rewardButton;
//     [Tooltip("Press karne par kya add hoga: Coins, Gems ya Both")]
//     public RewardType rewardType = RewardType.Coins;
//     [Min(0)] public int rewardCoinAmount = 1000;
//     [Min(0)] public int rewardGemAmount = 50;

//     const string OWNED_KEY    = "KnifeOwned_";
//     const string EQUIPPED_KEY = "KnifeEquipped";

//     // Koi bhi page knife equip kare to saare pages ko khabar
//     static event Action OnEquippedChanged;

//     bool isRolling = false;
//     int highlightSlot = -1;

//     // Is page ki pehli knife ka global number (page 2 ki pehli knife = 9, page 3 = 18 ...)
//     int FirstGlobalIndex => pageIndex * slots.Length;
//     int GlobalIndex(int slotIndex) => FirstGlobalIndex + slotIndex;

//     // ---------------- Save / Load ----------------
//     bool IsOwned(int globalIndex)
//     {
//         if (firstKnifeFree && globalIndex == 0) return true;
//         return PlayerPrefs.GetInt(OWNED_KEY + globalIndex, 0) == 1;
//     }

//     void SetOwned(int globalIndex)
//     {
//         PlayerPrefs.SetInt(OWNED_KEY + globalIndex, 1);
//         PlayerPrefs.Save();
//     }

//     static int EquippedIndex
//     {
//         get => PlayerPrefs.GetInt(EQUIPPED_KEY, 0);
//         set { PlayerPrefs.SetInt(EQUIPPED_KEY, value); PlayerPrefs.Save(); }
//     }

//     // ---------------- Unity ----------------
//     void Awake()
//     {
//         for (int i = 0; i < slots.Length; i++)
//         {
//             int slotIndex = i; // closure ke liye local copy
//             slots[i].button.onClick.AddListener(() => OnKnifeClicked(slotIndex));
//         }

//         randomButton.onClick.AddListener(OnRandomClicked);

//         if (rewardButton != null)
//             rewardButton.onClick.AddListener(GiveReward);
//     }

//     void OnDestroy()
//     {
//         if (rewardButton != null)
//             rewardButton.onClick.RemoveListener(GiveReward);
//     }

//     void OnEnable()
//     {
//         OnEquippedChanged += Refresh;
//         isRolling = false;
//         highlightSlot = -1;
//         Refresh();   // page khulte hi latest state dikhe
//     }

//     void OnDisable()
//     {
//         OnEquippedChanged -= Refresh;
//         StopAllCoroutines();   // roll beech me ho aur page band ho jaye to
//         isRolling = false;
//         highlightSlot = -1;
//     }

//     // ---------------- Knife touch -> Equip ----------------
//     void OnKnifeClicked(int slotIndex)
//     {
//         if (isRolling) return;

//         if (slotIndex >= knifeSprites.Length) return;

//         int globalIndex = GlobalIndex(slotIndex);

//         if (!IsOwned(globalIndex))
//         {
//             ShowMessage("Knife locked hai! Random button se unlock karo.");
//             return;
//         }

//         EquippedIndex = globalIndex;
//         OnEquippedChanged?.Invoke();   // saare pages refresh (is page ka bhi)
//     }

//     // ---------------- Random button ----------------
//     void OnRandomClicked()
//     {
//         if (isRolling) return;

//         // Is page ki locked knives
//         List<int> lockedOnPage = GetLockedOnPage();
//         if (lockedOnPage.Count == 0)
//         {
//             ShowMessage("Is page ki saari knives unlock hain!");
//             Refresh();
//             return;
//         }

//         // STEP 1: CoinManager se coins (aur gems agar lagte hain) check
//         if (CoinManager.GetCoins() < coinCost)
//         {
//             ShowMessage("Coins kam hain!");
//             return;
//         }

//         if (gemCost > 0 && CoinManager.GetGems() < gemCost)
//         {
//             ShowMessage("Gems kam hain!");
//             return;
//         }

//         // STEP 2: Check pass -> amount kaato (SpendCoins/SpendGems 0 par false dete hain, isliye >0 par hi call)
//         if (coinCost > 0) CoinManager.SpendCoins(coinCost);
//         if (gemCost > 0)  CoinManager.SpendGems(gemCost);

//         // STEP 3: Random pick shuru
//         StartCoroutine(RollRoutine(lockedOnPage));
//     }

//     // Is page ki locked knives (global index list)
//     List<int> GetLockedOnPage()
//     {
//         List<int> list = new List<int>();
//         for (int i = 0; i < knifeSprites.Length && i < slots.Length; i++)
//         {
//             int g = GlobalIndex(i);
//             if (!IsOwned(g)) list.Add(g);
//         }
//         return list;
//     }

//     IEnumerator RollRoutine(List<int> lockedOnPage)
//     {
//         isRolling = true;
//         randomButton.interactable = false;

//         // Winner pehle hi chun lo
//         int winnerPos = UnityEngine.Random.Range(0, lockedOnPage.Count);
//         int winner = lockedOnPage[winnerPos];

//         // Highlight is tarah ghumegi ki aakhri step par winner par ruke
//         int count = lockedOnPage.Count;
//         int offset = ((winnerPos - (rollSteps - 1)) % count + count) % count;

//         for (int step = 0; step < rollSteps; step++)
//         {
//             int globalIdx = lockedOnPage[(step + offset) % count];
//             highlightSlot = globalIdx - FirstGlobalIndex;
//             UpdateSlots();

//             float t = (float)step / Mathf.Max(1, rollSteps - 1);
//             // Realtime: Time.timeScale 0 ho tab bhi chale
//             yield return new WaitForSecondsRealtime(Mathf.Lerp(rollStartDelay, rollEndDelay, t));
//         }

//         // Unlock (equip nahi hoga, player khud touch karke equip karega)
//         SetOwned(winner);
//         highlightSlot = -1;
//         isRolling = false;

//         ShowMessage("Nayi knife mili!");
//         Refresh();
//     }

//     // ---------------- UI ----------------
//     void Refresh()
//     {
//         // Is page ka price (CoinManager ke format se: 1.5K, 2M etc.)
//         if (costCoinsText) costCoinsText.text = CoinManager.FormatNumber(coinCost);
//         if (costGemsText)  costGemsText.text  = CoinManager.FormatNumber(gemCost);
//         if (costGemsRoot)  costGemsRoot.SetActive(gemCost > 0);

//         UpdateSlots();

//         // Is page me locked bachi hai to hi button chalega
//         // (coins kam hone par bhi dabega, taaki "Coins kam hain" message dikhe)
//         randomButton.interactable = !isRolling && GetLockedOnPage().Count > 0;
//     }

//     void UpdateSlots()
//     {
//         for (int i = 0; i < slots.Length; i++)
//         {
//             KnifeSlot s = slots[i];

//             // Is slot ke liye sprite nahi di -> slot chhupa do
//             if (i >= knifeSprites.Length)
//             {
//                 s.background.gameObject.SetActive(false);   // poora slot (parent) chhupao
//                 continue;
//             }

//             s.background.gameObject.SetActive(true);
//             s.knifeIcon.sprite = knifeSprites[i];

//             int g = GlobalIndex(i);
//             bool owned    = IsOwned(g);
//             bool equipped = owned && EquippedIndex == g;

//             if (!owned)
//             {
//                 // LOCKED: poora black icon
//                 s.knifeIcon.color  = Color.black;
//                 s.background.color = lockedBackground;
//             }
//             else if (equipped)
//             {
//                 // EQUIPPED: poora clear
//                 s.knifeIcon.color  = Color.white;
//                 s.background.color = ownedBackground;
//             }
//             else
//             {
//                 // OWNED par equip nahi: thoda fade
//                 s.knifeIcon.color  = new Color(1f, 1f, 1f, fadedAlpha);
//                 s.background.color = new Color(ownedBackground.r, ownedBackground.g, ownedBackground.b, fadedAlpha);
//             }

//             // Random roll ke time wali highlight
//             if (i == highlightSlot)
//                 s.background.color = rollHighlight;
//         }
//     }

//     // ---------------- Reward Button ----------------
//     // CoinManager.AddCoins / AddGems khud counting animation, text pop aur icon bounce chala deta hai
//     void GiveReward()
//     {
//         switch (rewardType)
//         {
//             case RewardType.Coins:
//                 CoinManager.AddCoins(rewardCoinAmount);
//                 break;

//             case RewardType.Gems:
//                 CoinManager.AddGems(rewardGemAmount);
//                 break;

//             case RewardType.Both:
//                 CoinManager.AddCoins(rewardCoinAmount);
//                 CoinManager.AddGems(rewardGemAmount);
//                 break;
//         }
//     }

//     void ShowMessage(string msg)
//     {
//         if (messageText) messageText.text = msg;
//         Debug.Log(msg);
//     }

//     // Testing: sirf is page ka knife data hataye (coins/gems ko haath nahi lagata)
//     [ContextMenu("Reset This Page Knife Save")]
//     void ResetThisPage()
//     {
//         for (int i = 0; i < slots.Length; i++)
//             PlayerPrefs.DeleteKey(OWNED_KEY + GlobalIndex(i));

//         // Agar equipped knife isi page ki thi to default par wapas
//         int eq = EquippedIndex;
//         if (eq >= FirstGlobalIndex && eq < FirstGlobalIndex + slots.Length)
//             PlayerPrefs.DeleteKey(EQUIPPED_KEY);

//         PlayerPrefs.Save();
//         Refresh();
//     }
// }


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Knife Shop - EK PAGE KE LIYE. Ye script har page par alag se lagao.
///
/// Har page par Inspector me ye set karo:
///   - pageIndex   : page 1 = 0, page 2 = 1, page 3 = 2 ...  (har page ka alag, warna save mix ho jayega)
///   - knifeSprites: sirf is page ki knives (max 9)
///   - coinCost / gemCost : is page ka random price (gemCost 0 = gems check nahi hoga)
///
/// Random button : CoinManager se coins/gems check -> amount kaato -> random roll
///                 -> sirf is page ki LOCKED knives me se ek unlock.
/// Knife touch   : owned ho to equip. Equip poore game me ek hi hota hai,
///                 isliye dusre pages ki knife equip hote hi unka look khud update ho jata hai.
/// Look          : Locked = icon poora BLACK | Owned (equip nahi) = thoda FADE | Equipped = poora CLEAR
///
/// Total coins/gems ke TMP texts CoinManager ke coinTexts / gemTexts me daalo.
/// </summary>
public class KnifeShop : MonoBehaviour
{
    [Serializable]
    public class KnifeSlot
    {
        public Button button;      // knife ka button
        public Image knifeIcon;    // knife ki image
        public Image background;   // slot ka background
    }

    [Header("Page Setup (har page par alag)")]
    [Tooltip("Page 1 = 0, Page 2 = 1, Page 3 = 2 ... Har page ka alag number rakho.")]
    public int pageIndex = 0;
    [Tooltip("Sirf is page ki knives ki sprites (slots se zyada nahi).")]
    public Sprite[] knifeSprites;
    [Tooltip("Sabse pehli knife (page 1 ki pehli) free milegi.")]
    public bool firstKnifeFree = true;

    [Header("Price (is page ka)")]
    public int coinCost = 500;
    [Tooltip("0 = gems check nahi hoga. 0 se zyada = gems bhi lagenge.")]
    public int gemCost = 0;

    [Header("UI - Slots (9 slots)")]
    public KnifeSlot[] slots = new KnifeSlot[9];

    [Header("UI - Random Button / Texts")]
    public Button randomButton;
    public TMP_Text costCoinsText;     // random button par coin price (optional)
    public TMP_Text costGemsText;      // random button par gem price (optional)
    public GameObject costGemsRoot;    // gem price icon+text, gemCost 0 ho to chhup jayega (optional)
    public TMP_Text messageText;       // "Coins kam hain" jaise messages (optional)

    [Header("Look")]
    [Range(0f, 1f)] public float fadedAlpha = 0.5f;   // owned par equip nahi wali knife kitni fade
    public Color lockedBackground = new Color(0.30f, 0.30f, 0.30f, 1f);
    public Color ownedBackground  = new Color(0.30f, 0.65f, 1.00f, 1f);
    public Color rollHighlight    = Color.white;

    [Header("Random Roll")]
    public int rollSteps = 14;             // roll me highlight kitni baar ghumegi
    public float rollStartDelay = 0.05f;   // shuru me tez
    public float rollEndDelay = 0.25f;     // end me dheemi

    // ---------------- Reward Button (coins / gems add) ----------------
    public enum RewardType
    {
        Coins,   // sirf coins
        Gems,    // sirf gems
        Both     // coins aur gems dono
    }

    [Header("Reward Button (coins / gems add karne ke liye)")]
    [Tooltip("Optional. Khali chhodo to reward button nahi chalega.")]
    public Button rewardButton;
    [Tooltip("Press karne par kya add hoga: Coins, Gems ya Both")]
    public RewardType rewardType = RewardType.Coins;
    [Min(0)] public int rewardCoinAmount = 1000;
    [Min(0)] public int rewardGemAmount = 50;

    const string OWNED_KEY    = "KnifeOwned_";
    const string EQUIPPED_KEY = "KnifeEquipped";

    // Koi bhi page knife equip kare to saare pages ko khabar
    public static event Action OnEquippedChanged;

    bool isRolling = false;
    int highlightSlot = -1;

    // Is page ki pehli knife ka global number (page 2 ki pehli knife = 9, page 3 = 18 ...)
    int FirstGlobalIndex => pageIndex * slots.Length;
    int GlobalIndex(int slotIndex) => FirstGlobalIndex + slotIndex;

    // ---------------- Save / Load ----------------
    bool IsOwned(int globalIndex)
    {
        if (firstKnifeFree && globalIndex == 0) return true;
        return PlayerPrefs.GetInt(OWNED_KEY + globalIndex, 0) == 1;
    }

    void SetOwned(int globalIndex)
    {
        PlayerPrefs.SetInt(OWNED_KEY + globalIndex, 1);
        PlayerPrefs.Save();
    }

    public static int EquippedIndex
    {
        get => PlayerPrefs.GetInt(EQUIPPED_KEY, 0);
        set { PlayerPrefs.SetInt(EQUIPPED_KEY, value); PlayerPrefs.Save(); }
    }

    // ---------------- Unity ----------------
    void Awake()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            int slotIndex = i; // closure ke liye local copy
            slots[i].button.onClick.AddListener(() => OnKnifeClicked(slotIndex));
        }

        randomButton.onClick.AddListener(OnRandomClicked);

        if (rewardButton != null)
            rewardButton.onClick.AddListener(GiveReward);
    }

    void OnDestroy()
    {
        if (rewardButton != null)
            rewardButton.onClick.RemoveListener(GiveReward);
    }

    void OnEnable()
    {
        OnEquippedChanged += Refresh;
        isRolling = false;
        highlightSlot = -1;
        Refresh();   // page khulte hi latest state dikhe
    }

    void OnDisable()
    {
        OnEquippedChanged -= Refresh;
        StopAllCoroutines();   // roll beech me ho aur page band ho jaye to
        isRolling = false;
        highlightSlot = -1;
    }

    // ---------------- Knife touch -> Equip ----------------
    void OnKnifeClicked(int slotIndex)
    {
        if (isRolling) return;

        if (slotIndex >= knifeSprites.Length) return;

        int globalIndex = GlobalIndex(slotIndex);

        if (!IsOwned(globalIndex))
        {
            ShowMessage("Knife locked hai! Random button se unlock karo.");
            return;
        }

        EquippedIndex = globalIndex;
        OnEquippedChanged?.Invoke();   // saare pages refresh (is page ka bhi)
    }

    // ---------------- Random button ----------------
    void OnRandomClicked()
    {
        if (isRolling) return;

        // Is page ki locked knives
        List<int> lockedOnPage = GetLockedOnPage();
        if (lockedOnPage.Count == 0)
        {
            ShowMessage("Is page ki saari knives unlock hain!");
            Refresh();
            return;
        }

        // STEP 1: CoinManager se coins (aur gems agar lagte hain) check
        if (CoinManager.GetCoins() < coinCost)
        {
            ShowMessage("Coins kam hain!");
            return;
        }

        if (gemCost > 0 && CoinManager.GetGems() < gemCost)
        {
            ShowMessage("Gems kam hain!");
            return;
        }

        // STEP 2: Check pass -> amount kaato (SpendCoins/SpendGems 0 par false dete hain, isliye >0 par hi call)
        if (coinCost > 0) CoinManager.SpendCoins(coinCost);
        if (gemCost > 0)  CoinManager.SpendGems(gemCost);

        // STEP 3: Random pick shuru
        StartCoroutine(RollRoutine(lockedOnPage));
    }

    // Is page ki locked knives (global index list)
    List<int> GetLockedOnPage()
    {
        List<int> list = new List<int>();
        for (int i = 0; i < knifeSprites.Length && i < slots.Length; i++)
        {
            int g = GlobalIndex(i);
            if (!IsOwned(g)) list.Add(g);
        }
        return list;
    }

    IEnumerator RollRoutine(List<int> lockedOnPage)
    {
        isRolling = true;
        randomButton.interactable = false;

        // Winner pehle hi chun lo
        int winnerPos = UnityEngine.Random.Range(0, lockedOnPage.Count);
        int winner = lockedOnPage[winnerPos];

        // Highlight is tarah ghumegi ki aakhri step par winner par ruke
        int count = lockedOnPage.Count;
        int offset = ((winnerPos - (rollSteps - 1)) % count + count) % count;

        for (int step = 0; step < rollSteps; step++)
        {
            int globalIdx = lockedOnPage[(step + offset) % count];
            highlightSlot = globalIdx - FirstGlobalIndex;
            UpdateSlots();

            float t = (float)step / Mathf.Max(1, rollSteps - 1);
            // Realtime: Time.timeScale 0 ho tab bhi chale
            yield return new WaitForSecondsRealtime(Mathf.Lerp(rollStartDelay, rollEndDelay, t));
        }

        // Unlock (equip nahi hoga, player khud touch karke equip karega)
        SetOwned(winner);
        highlightSlot = -1;
        isRolling = false;

        ShowMessage("Nayi knife mili!");
        Refresh();
    }

    // ---------------- UI ----------------
    void Refresh()
    {
        // Is page ka price (CoinManager ke format se: 1.5K, 2M etc.)
        if (costCoinsText) costCoinsText.text = CoinManager.FormatNumber(coinCost);
        if (costGemsText)  costGemsText.text  = CoinManager.FormatNumber(gemCost);
        if (costGemsRoot)  costGemsRoot.SetActive(gemCost > 0);

        UpdateSlots();

        // Is page me locked bachi hai to hi button chalega
        // (coins kam hone par bhi dabega, taaki "Coins kam hain" message dikhe)
        randomButton.interactable = !isRolling && GetLockedOnPage().Count > 0;
    }

    void UpdateSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            KnifeSlot s = slots[i];

            // Is slot ke liye sprite nahi di -> slot chhupa do
            if (i >= knifeSprites.Length)
            {
                s.background.gameObject.SetActive(false);   // poora slot (parent) chhupao
                continue;
            }

            s.background.gameObject.SetActive(true);
            s.knifeIcon.sprite = knifeSprites[i];

            int g = GlobalIndex(i);
            bool owned    = IsOwned(g);
            bool equipped = owned && EquippedIndex == g;

            if (!owned)
            {
                // LOCKED: poora black icon
                s.knifeIcon.color  = Color.black;
                s.background.color = lockedBackground;
            }
            else if (equipped)
            {
                // EQUIPPED: poora clear
                s.knifeIcon.color  = Color.white;
                s.background.color = ownedBackground;
            }
            else
            {
                // OWNED par equip nahi: thoda fade
                s.knifeIcon.color  = new Color(1f, 1f, 1f, fadedAlpha);
                s.background.color = new Color(ownedBackground.r, ownedBackground.g, ownedBackground.b, fadedAlpha);
            }

            // Random roll ke time wali highlight
            if (i == highlightSlot)
                s.background.color = rollHighlight;
        }
    }

    // ---------------- Reward Button ----------------
    // CoinManager.AddCoins / AddGems khud counting animation, text pop aur icon bounce chala deta hai
    void GiveReward()
    {
        switch (rewardType)
        {
            case RewardType.Coins:
                CoinManager.AddCoins(rewardCoinAmount);
                break;

            case RewardType.Gems:
                CoinManager.AddGems(rewardGemAmount);
                break;

            case RewardType.Both:
                CoinManager.AddCoins(rewardCoinAmount);
                CoinManager.AddGems(rewardGemAmount);
                break;
        }
    }

    void ShowMessage(string msg)
    {
        if (messageText) messageText.text = msg;
        Debug.Log(msg);
    }

    // Testing: sirf is page ka knife data hataye (coins/gems ko haath nahi lagata)
    [ContextMenu("Reset This Page Knife Save")]
    void ResetThisPage()
    {
        for (int i = 0; i < slots.Length; i++)
            PlayerPrefs.DeleteKey(OWNED_KEY + GlobalIndex(i));

        // Agar equipped knife isi page ki thi to default par wapas
        int eq = EquippedIndex;
        if (eq >= FirstGlobalIndex && eq < FirstGlobalIndex + slots.Length)
            PlayerPrefs.DeleteKey(EQUIPPED_KEY);

        PlayerPrefs.Save();
        Refresh();
    }
}