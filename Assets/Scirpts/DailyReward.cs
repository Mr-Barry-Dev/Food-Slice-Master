using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 9 Day Daily Reward
/// - Ek common Claim button, uske text me "Day N"
/// - Claim dabane par usi day ke panel ka gift image FLIP hota hai
///   aur panel ke andar coins/gems ka amount text dikhta hai
/// - Reward automatic CoinManager me add hota hai
/// - Ek bhi din miss hua to Day 1 se reset
/// - Har day me coins ya gems random (fix range) + % chance
/// </summary>
public class DailyReward : MonoBehaviour
{
    public enum RewardType { Coins, Gems }

    [Serializable]
    public class DayConfig
    {
        [Header("Coins range")]
        public int coinMin = 100;
        public int coinMax = 200;

        [Header("Gems range")]
        public int gemMin = 1;
        public int gemMax = 5;

        [Header("Chance (%)")]
        [Range(0, 100)] public int coinChance = 70;   // gems chance = 100 - coinChance
    }

    [Serializable]
    public class DaySlot
    {
        public RectTransform card;       // poora panel jo flip hoga (scale X 1 -> 0 -> 1)
        public Image background;         // panel ka background (claimed/locked color)
        public GameObject giftGroup;     // front: gift image (claim se pehle)
        public Image giftImage;          // gift image (locked par dark tint ke liye)
        public GameObject rewardGroup;   // back: reward icon + amount (claim ke baad)
        public Image rewardIcon;         // coin ya gem ka icon
        public TMP_Text amountText;      // "+150"
    }

    [Header("9 Days Config (exactly 9)")]
    [SerializeField] private DayConfig[] days = new DayConfig[9];

    [Header("9 Day Panels (exactly 9)")]
    [SerializeField] private DaySlot[] slots = new DaySlot[9];

    [Header("Claim Button")]
    [SerializeField] private Button claimButton;
    [SerializeField] private TMP_Text claimButtonText;     // "Day 5"
    [SerializeField] private TMP_Text timerText;           // optional

    [Header("Sprites")]
    [SerializeField] private Sprite coinSprite;
    [SerializeField] private Sprite gemSprite;

    [Header("Colors")]
    [SerializeField] private Color claimedColor = new Color(0.45f, 0.45f, 0.45f, 1f);
    [SerializeField] private Color currentColor = Color.white;
    [SerializeField] private Color lockedColor  = new Color(0.15f, 0.15f, 0.15f, 1f);

    [Header("Flip")]
    [SerializeField] private float flipDuration = 0.4f;    // poora flip (aadha band + aadha khulna)

    [Header("Sounds (sab optional)")]
    [SerializeField] private AudioSource audioSource;      // khali chhodo to script khud bana legi
    [SerializeField] private AudioClip claimClickSound;    // Claim button dabane par
    [SerializeField] private AudioClip flipSound;          // gift flip hote waqt
    [SerializeField] private AudioClip coinRewardSound;    // coins mile to
    [SerializeField] private AudioClip gemRewardSound;     // gems mile to
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 1f;

    public event Action<int, RewardType, int> OnRewardClaimed;   // (dayIndex, type, amount)

    private const int TotalDays = 9;
    private const string KeyLastDate  = "DR_LastClaimDate";
    private const string KeyLastIndex = "DR_LastClaimIndex";
    private const string KeyType      = "DR_Type_";     // + dayIndex
    private const string KeyAmount    = "DR_Amount_";   // + dayIndex

    private int currentIndex;
    private bool canClaim;
    private bool isAnimating;

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        audioSource.PlayOneShot(clip, soundVolume);
    }

    private void Start()
    {
        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(Claim);
        Refresh();
    }

    // Panel dubara khulne par (CloseBtn se band karke) state fresh ho jaye.
    // Flip ke beech panel band hua to isAnimating atka na rahe.
    private void OnEnable()
    {
        isAnimating = false;
        if (claimButton != null) Refresh();
    }

    private void Update()
    {
        if (isAnimating) return;

        if (!canClaim)
        {
            if (timerText != null)
            {
                TimeSpan left = DateTime.Today.AddDays(1) - DateTime.Now;
                timerText.text = $"Next reward in {left.Hours:00}:{left.Minutes:00}:{left.Seconds:00}";
            }

            // Raat 12 ke baad naya din unlock
            if (DateTime.Now.Date > GetLastClaimDate()) Refresh();
        }
        else if (timerText != null && timerText.text.Length > 0)
        {
            timerText.text = "";
        }
    }

    // ---------------- State ----------------

    private DateTime GetLastClaimDate()
    {
        string s = PlayerPrefs.GetString(KeyLastDate, "");
        return long.TryParse(s, out long ticks) ? new DateTime(ticks).Date : DateTime.MinValue;
    }

    private void CalculateState()
    {
        int lastIndex = PlayerPrefs.GetInt(KeyLastIndex, -1);

        if (lastIndex < 0)                       // pehli baar
        {
            currentIndex = 0;
            canClaim = true;
            return;
        }

        int diffDays = (DateTime.Now.Date - GetLastClaimDate()).Days;

        if (diffDays <= 0)                       // aaj claim ho chuka
        {
            currentIndex = lastIndex;
            canClaim = false;
        }
        else if (diffDays == 1)                  // lagataar agla din
        {
            currentIndex = (lastIndex + 1) % TotalDays;
            canClaim = true;
            if (currentIndex == 0) ClearSavedRewards();   // naya cycle
        }
        else                                     // din miss -> RESET
        {
            currentIndex = 0;
            canClaim = true;
            ClearSavedRewards();
        }
    }

    private void ClearSavedRewards()
    {
        for (int i = 0; i < TotalDays; i++)
        {
            PlayerPrefs.DeleteKey(KeyType + i);
            PlayerPrefs.DeleteKey(KeyAmount + i);
        }
        PlayerPrefs.DeleteKey(KeyLastIndex);
        PlayerPrefs.DeleteKey(KeyLastDate);
        PlayerPrefs.Save();
    }

    private void Refresh()
    {
        CalculateState();

        for (int i = 0; i < TotalDays && i < slots.Length; i++)
        {
            DaySlot s = slots[i];
            if (s == null) continue;

            bool claimed = i < currentIndex || (i == currentIndex && !canClaim);
            bool current = i == currentIndex && canClaim;

            Color c = current ? currentColor : (claimed ? claimedColor : lockedColor);
            if (s.background != null) s.background.color = c;
            if (s.giftImage != null) s.giftImage.color = c;

            if (s.card != null) s.card.localScale = Vector3.one;

            if (claimed) ShowRewardFace(i);
            else ShowGiftFace(i);
        }

        if (claimButtonText != null)
            claimButtonText.text = "Day " + (currentIndex + 1);

        claimButton.interactable = canClaim && !isAnimating;
    }

    private void ShowGiftFace(int i)
    {
        DaySlot s = slots[i];
        if (s.giftGroup != null) s.giftGroup.SetActive(true);
        if (s.rewardGroup != null) s.rewardGroup.SetActive(false);
    }

    private void ShowRewardFace(int i)
    {
        DaySlot s = slots[i];

        RewardType type = (RewardType)PlayerPrefs.GetInt(KeyType + i, (int)RewardType.Coins);
        int amount = PlayerPrefs.GetInt(KeyAmount + i, 0);

        if (s.rewardIcon != null) s.rewardIcon.sprite = type == RewardType.Coins ? coinSprite : gemSprite;
        if (s.amountText != null) s.amountText.text = "+" + amount;

        if (s.giftGroup != null) s.giftGroup.SetActive(false);
        if (s.rewardGroup != null) s.rewardGroup.SetActive(true);
    }

    // ---------------- Claim ----------------

    private void Claim()
    {
        if (!canClaim || isAnimating) return;

        PlaySound(claimClickSound);

        int dayIndex = currentIndex;
        RollReward(days[dayIndex], out RewardType type, out int amount);

        // Save
        PlayerPrefs.SetInt(KeyLastIndex, dayIndex);
        PlayerPrefs.SetString(KeyLastDate, DateTime.Now.Date.Ticks.ToString());
        PlayerPrefs.SetInt(KeyType + dayIndex, (int)type);
        PlayerPrefs.SetInt(KeyAmount + dayIndex, amount);
        PlayerPrefs.Save();

        // Reward flip khulte hi CoinManager me add hoga (counting + pop animation ke sath)
        hasPendingReward = true;
        pendingType = type;
        pendingAmount = amount;

        OnRewardClaimed?.Invoke(dayIndex, type, amount);

        StartCoroutine(FlipRoutine(dayIndex));
    }

    private static void RollReward(DayConfig cfg, out RewardType type, out int amount)
    {
        int roll = UnityEngine.Random.Range(0, 100);   // 0..99

        if (roll < cfg.coinChance)
        {
            type = RewardType.Coins;
            amount = UnityEngine.Random.Range(cfg.coinMin, cfg.coinMax + 1);
        }
        else
        {
            type = RewardType.Gems;
            amount = UnityEngine.Random.Range(cfg.gemMin, cfg.gemMax + 1);
        }
    }

    private bool hasPendingReward;
    private RewardType pendingType;
    private int pendingAmount;

    // CoinManager ke static methods use hote hain (counting + pop animation unke andar hai)
    private void FlushPendingReward()
    {
        if (!hasPendingReward) return;
        hasPendingReward = false;

        if (pendingType == RewardType.Coins) CoinManager.AddCoins(pendingAmount);
        else CoinManager.AddGems(pendingAmount);
    }

    // Flip ke beech panel band/scene change ho jaye to bhi reward zaroor mile
    private void OnDisable()
    {
        FlushPendingReward();
    }

    // ---------------- Flip ----------------

    private IEnumerator FlipRoutine(int dayIndex)
    {
        isAnimating = true;
        claimButton.interactable = false;

        DaySlot s = slots[dayIndex];
        float half = flipDuration * 0.5f;

        PlaySound(flipSound);

        // 1) Gift side band hoti hai (scale X 1 -> 0)
        yield return ScaleX(s.card, 1f, 0f, half);

        // 2) Beech me side badalti hai: gift hata, reward dikha
        ShowRewardFace(dayIndex);
        if (s.background != null) s.background.color = claimedColor;

        // Reward ab CoinManager me add: coins counting + pop animation chalega
        PlaySound(pendingType == RewardType.Coins ? coinRewardSound : gemRewardSound);
        FlushPendingReward();

        // 3) Reward side khulti hai (scale X 0 -> 1)
        yield return ScaleX(s.card, 0f, 1f, half);

        isAnimating = false;
        Refresh();
    }

    private IEnumerator ScaleX(RectTransform rt, float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);
            rt.localScale = new Vector3(Mathf.Lerp(from, to, p), 1f, 1f);
            yield return null;
        }
        rt.localScale = new Vector3(to, 1f, 1f);
    }

    // ---------------- Testing ----------------

    [ContextMenu("Reset Daily Reward Data")]
    private void ResetData()
    {
        ClearSavedRewards();
        if (Application.isPlaying) Refresh();
    }

    private void OnValidate()
    {
        if (days == null || days.Length != TotalDays)
        {
            var old = days;
            days = new DayConfig[TotalDays];
            for (int i = 0; i < TotalDays; i++)
                days[i] = (old != null && i < old.Length && old[i] != null) ? old[i] : new DayConfig();
        }

        foreach (var d in days)
        {
            if (d == null) continue;
            if (d.coinMax < d.coinMin) d.coinMax = d.coinMin;
            if (d.gemMax < d.gemMin) d.gemMax = d.gemMin;
        }
    }
}