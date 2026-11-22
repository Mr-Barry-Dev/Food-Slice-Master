// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Collections;

// public class RandomPickReward : MonoBehaviour
// {
//     [Header("Cards")]
//     public Button[] buttons;

//     [Header("Reward UI")]
//     public Image[] rewardImages;
//     public TMP_Text[] rewardTexts;

//     [Header("Reward Sprites")]
//     public Sprite coinSprite;
//     public Sprite gemSprite;

//     [Header("Collect Buttons")]
//     public Button collectButton;
//     public Button doubleCollectButton;

//     [Header("Double Collect Text")]
//     public TMP_Text doubleCollectText;

//     [Header("Coin Reward Range")]
//     public int coinMin = 50;
//     public int coinMax = 200;

//     [Header("Gem Reward Range")]
//     public int gemMin = 10;
//     public int gemMax = 20;

//     [Header("Gem Chance (%)")]
//     [Range(0, 100)]
//     public int gemChance = 8;

//     [Header("Sounds")]
//     public AudioSource audioSource;
//     public AudioClip pickSound;
//     public AudioClip collectSound;
//     public AudioClip doubleCollectSound;

//     private int[] rewards;
//     private int[] rewardTypes;

//     private bool hasPicked = false;
//     private bool rewardCollected = false;

//     private int selectedReward = 0;
//     private int selectedType = 0;

//     private ColorBlock[] defaultColors;

//     private Coroutine flipCoroutine;

//     private bool giftRewardSessionFinished = false;

//     private void Start()
//     {
//         if (buttons == null || buttons.Length == 0)
//         {
//             Debug.LogError("RandomPickReward: Cards assign nahi hain!");
//             return;
//         }

//         rewards = new int[buttons.Length];
//         rewardTypes = new int[buttons.Length];
//         defaultColors = new ColorBlock[buttons.Length];

//         GenerateAllRewards();

//         for (int i = 0; i < buttons.Length; i++)
//         {
//             int index = i;

//             if (buttons[i] != null)
//             {
//                 defaultColors[i] = buttons[i].colors;
//                 buttons[i].interactable = true;
//                 buttons[i].transform.localScale = Vector3.one;

//                 buttons[i].onClick.AddListener(
//                     () => OnPick(index)
//                 );
//             }

//             if (rewardImages != null &&
//                 i < rewardImages.Length &&
//                 rewardImages[i] != null)
//             {
//                 rewardImages[i].gameObject.SetActive(false);
//             }

//             if (rewardTexts != null &&
//                 i < rewardTexts.Length &&
//                 rewardTexts[i] != null)
//             {
//                 rewardTexts[i].text = "";
//             }
//         }

//         // Dono buttons game start se visible rahenge.
//         if (collectButton != null)
//         {
//             collectButton.gameObject.SetActive(true);
//             collectButton.transform.localScale = Vector3.one;
//             collectButton.interactable = false;
//             collectButton.onClick.AddListener(OnCollect);
//         }

//         if (doubleCollectButton != null)
//         {
//             doubleCollectButton.gameObject.SetActive(true);
//             doubleCollectButton.transform.localScale = Vector3.one;
//             doubleCollectButton.interactable = false;
//             doubleCollectButton.onClick.AddListener(OnDoubleCollect);
//         }

//         if (doubleCollectText != null)
//         {
//             doubleCollectText.text = "";
//         }
//     }

//     // =========================================================
//     // REWARD GENERATION
//     // =========================================================

//     private void GenerateAllRewards()
//     {
//         for (int i = 0; i < buttons.Length; i++)
//         {
//             GenerateReward(i);
//         }
//     }

//     private void GenerateReward(int i)
//     {
//         int roll = Random.Range(0, 100);

//         if (roll < gemChance)
//         {
//             rewardTypes[i] = 1; // Gems
//             rewards[i] = Random.Range(gemMin, gemMax + 1);
//         }
//         else
//         {
//             rewardTypes[i] = 0; // Coins
//             rewards[i] = Random.Range(coinMin, coinMax + 1);
//         }
//     }

//     // =========================================================
//     // PICK CARD
//     // =========================================================

//     private void OnPick(int index)
//     {
//         if (hasPicked)
//             return;

//         if (index < 0 || index >= buttons.Length)
//             return;

//         hasPicked = true;
//         rewardCollected = false;

//         selectedReward = rewards[index];
//         selectedType = rewardTypes[index];

//         if (audioSource != null && pickSound != null)
//         {
//             audioSource.PlayOneShot(pickSound);
//         }

//         DisableAllCards();

//         // Card reveal hone tak collect buttons disabled rahenge.
//         if (collectButton != null)
//             collectButton.interactable = false;

//         if (doubleCollectButton != null)
//             doubleCollectButton.interactable = false;

//         flipCoroutine = StartCoroutine(FlipCard(index));
//     }

//     private IEnumerator FlipCard(int index)
//     {
//         Transform card = buttons[index].transform;

//         float duration = 0.25f;
//         float time = 0f;

//         while (time < duration)
//         {
//             float scale = Mathf.Lerp(1f, 0f, time / duration);

//             card.localScale = new Vector3(scale, 1f, 1f);

//             time += Time.deltaTime;
//             yield return null;
//         }

//         card.localScale = new Vector3(0f, 1f, 1f);

//         if (rewardImages != null &&
//             index < rewardImages.Length &&
//             rewardImages[index] != null)
//         {
//             rewardImages[index].sprite =
//                 selectedType == 0 ? coinSprite : gemSprite;

//             rewardImages[index].gameObject.SetActive(true);
//         }

//         if (rewardTexts != null &&
//             index < rewardTexts.Length &&
//             rewardTexts[index] != null)
//         {
//             rewardTexts[index].text = selectedReward.ToString();
//         }

//         time = 0f;

//         while (time < duration)
//         {
//             float scale = Mathf.Lerp(0f, 1f, time / duration);

//             card.localScale = new Vector3(scale, 1f, 1f);

//             time += Time.deltaTime;
//             yield return null;
//         }

//         card.localScale = Vector3.one;

//         if (doubleCollectText != null)
//         {
//             doubleCollectText.text = "2X " + selectedReward;
//         }

//         // Reward reveal hone ke baad buttons active honge.
//         // Dono buttons visible hi rahenge.
//         if (collectButton != null)
//             collectButton.interactable = true;

//         if (doubleCollectButton != null)
//             doubleCollectButton.interactable = (selectedType == 0);

//         flipCoroutine = null;
//     }

//     // =========================================================
//     // COLLECT NORMAL REWARD
//     // =========================================================

//     private void OnCollect()
// {
//     if (!hasPicked || rewardCollected || giftRewardSessionFinished)
//         return;

//     rewardCollected = true;
//     giftRewardSessionFinished = true;

//     DisableRewardButtons();

//     if (audioSource != null && collectSound != null)
//         audioSource.PlayOneShot(collectSound);

//     if (selectedType == 0)
//         CoinManager.AddCoins(selectedReward);
//     else
//         CoinManager.AddGems(selectedReward);

//     // Cards reset nahi karenge, isliye isi gift se dobara reward nahi milega.
// }

//     // =========================================================
//     // DOUBLE COLLECT
//     // =========================================================

//     private void OnDoubleCollect()
// {
//     if (!hasPicked || rewardCollected || giftRewardSessionFinished)
//         return;

//     if (selectedType != 0)
//         return;

//     rewardCollected = true;
//     giftRewardSessionFinished = true;

//     DisableRewardButtons();

//     if (audioSource != null && doubleCollectSound != null)
//         audioSource.PlayOneShot(doubleCollectSound);

//     CoinManager.AddCoins(selectedReward * 2);

//     // Isi gift se dobara collect ya double collect nahi hoga.
// }

//     // =========================================================
//     // BUTTON HELPERS
//     // =========================================================

//     private void DisableAllCards()
//     {
//         foreach (Button card in buttons)
//         {
//             if (card == null)
//                 continue;

//             card.interactable = false;

//             ColorBlock cb = card.colors;
//             cb.normalColor = new Color(0.6f, 0.6f, 0.6f);
//             cb.disabledColor = new Color(0.6f, 0.6f, 0.6f);

//             card.colors = cb;
//         }
//     }

//     private void DisableRewardButtons()
//     {
//         if (collectButton != null)
//             collectButton.interactable = false;

//         if (doubleCollectButton != null)
//             doubleCollectButton.interactable = false;
//     }

//     // =========================================================
//     // RESET FOR NEXT PICK
//     // =========================================================

//     private void ResetForNextPick()
//     {
//         hasPicked = false;
//         rewardCollected = false;

//         GenerateAllRewards();

//         for (int i = 0; i < buttons.Length; i++)
//         {
//             if (rewardImages != null &&
//                 i < rewardImages.Length &&
//                 rewardImages[i] != null)
//             {
//                 rewardImages[i].gameObject.SetActive(false);
//             }

//             if (rewardTexts != null &&
//                 i < rewardTexts.Length &&
//                 rewardTexts[i] != null)
//             {
//                 rewardTexts[i].text = "";
//             }

//             if (buttons[i] != null)
//             {
//                 buttons[i].interactable = true;
//                 buttons[i].colors = defaultColors[i];
//                 buttons[i].transform.localScale = Vector3.one;
//             }
//         }

//         if (doubleCollectText != null)
//         {
//             doubleCollectText.text = "";
//         }

//         // Buttons hamesha visible rahenge.
//         if (collectButton != null)
//         {
//             collectButton.gameObject.SetActive(true);
//             collectButton.interactable = false;
//             collectButton.transform.localScale = Vector3.one;
//         }

//         if (doubleCollectButton != null)
//         {
//             doubleCollectButton.gameObject.SetActive(true);
//             doubleCollectButton.interactable = false;
//             doubleCollectButton.transform.localScale = Vector3.one;
//         }
//     }
// }



using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class RandomPickReward : MonoBehaviour
{
    // Reward types
    private const int TYPE_COIN = 0;
    private const int TYPE_GEM = 1;
    private const int TYPE_KNIFE = 2;

    // KnifeShop ke saath same key (KnifeShop.OWNED_KEY)
    private const string KNIFE_OWNED_KEY = "KnifeOwned_";

    [Header("Cards")]
    public Button[] buttons;

    [Header("Reward UI")]
    public Image[] rewardImages;
    public TMP_Text[] rewardTexts;

    [Header("Reward Sprites")]
    public Sprite coinSprite;
    public Sprite gemSprite;

    [Header("Collect Buttons")]
    public Button collectButton;
    public Button doubleCollectButton;

    [Header("Double Collect Text")]
    public TMP_Text doubleCollectText;

    [Header("Coin Reward Range")]
    public int coinMin = 50;
    public int coinMax = 200;

    [Header("Gem Reward Range")]
    public int gemMin = 10;
    public int gemMax = 20;

    [Header("Gem Chance (%)")]
    [Range(0, 100)]
    public int gemChance = 8;

    [Header("Knife Reward (KnifeLibrary se)")]
    [Tooltip("Wahi KnifeLibrary asset jo shop me use hota hai.")]
    public KnifeLibrary knifeLibrary;

    [Tooltip("Har card par knife aane ka chance (%). Saari knives unlock hon to coins/gems hi aayenge.")]
    [Range(0, 100)]
    public int knifeChance = 5;

    [Tooltip("Kin PAGES ki knives gift se mil sakti hain. Page ka number likho: 1 = Page 1, 2 = Page 2, 3 = Page 3 ... KHALI chhodo to SAARE pages ki knives aa sakti hain.")]
    public int[] allowedKnifePages;

    [Tooltip("KnifeShop ki 'First Knife Free' jaisi setting. ON ho to knife #0 hamesha owned maani jayegi.")]
    public bool firstKnifeFree = true;

    [Tooltip("Knife prefab me se icon nahi mila to ye sprite dikhegi.")]
    public Sprite fallbackKnifeSprite;

    [Tooltip("Knife wale card ke text me ye likha aayega.")]
    public string knifeRewardLabel = "NEW KNIFE";

    [Tooltip("ON: knife collect hote hi equip bhi ho jayegi. OFF: sirf unlock (shop me jakar equip karni hogi).")]
    public bool equipOnUnlock = false;

    [Tooltip("Knife unlock hone par chalega (parameter = knife ka global index).")]
    public UnityEvent<int> onKnifeUnlocked;

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip pickSound;
    public AudioClip collectSound;
    public AudioClip doubleCollectSound;

    private int[] rewards;       // coin/gem: amount, knife: global knife index
    private int[] rewardTypes;

    private bool hasPicked = false;
    private bool rewardCollected = false;

    private int selectedReward = 0;
    private int selectedType = 0;

    private ColorBlock[] defaultColors;

    private Coroutine flipCoroutine;

    private bool giftRewardSessionFinished = false;

    private void Start()
    {
        if (buttons == null || buttons.Length == 0)
        {
            Debug.LogError("RandomPickReward: Cards assign nahi hain!");
            return;
        }

        rewards = new int[buttons.Length];
        rewardTypes = new int[buttons.Length];
        defaultColors = new ColorBlock[buttons.Length];

        GenerateAllRewards();

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;

            if (buttons[i] != null)
            {
                defaultColors[i] = buttons[i].colors;
                buttons[i].interactable = true;
                buttons[i].transform.localScale = Vector3.one;

                buttons[i].onClick.AddListener(
                    () => OnPick(index)
                );
            }

            if (rewardImages != null &&
                i < rewardImages.Length &&
                rewardImages[i] != null)
            {
                rewardImages[i].gameObject.SetActive(false);
            }

            if (rewardTexts != null &&
                i < rewardTexts.Length &&
                rewardTexts[i] != null)
            {
                rewardTexts[i].text = "";
            }
        }

        // Dono buttons game start se visible rahenge.
        if (collectButton != null)
        {
            collectButton.gameObject.SetActive(true);
            collectButton.transform.localScale = Vector3.one;
            collectButton.interactable = false;
            collectButton.onClick.AddListener(OnCollect);
        }

        if (doubleCollectButton != null)
        {
            doubleCollectButton.gameObject.SetActive(true);
            doubleCollectButton.transform.localScale = Vector3.one;
            doubleCollectButton.interactable = false;
            doubleCollectButton.onClick.AddListener(OnDoubleCollect);
        }

        if (doubleCollectText != null)
        {
            doubleCollectText.text = "";
        }
    }

    // =========================================================
    // KNIFE OWNED / UNLOCK (KnifeShop ke same PlayerPrefs key se)
    // global index = pageIndex * slotsPerPage + slot
    // =========================================================

    private bool IsKnifeOwned(int globalIndex)
    {
        if (firstKnifeFree && globalIndex == 0)
            return true;

        return PlayerPrefs.GetInt(KNIFE_OWNED_KEY + globalIndex, 0) == 1;
    }

    private void UnlockKnife(int globalIndex)
    {
        PlayerPrefs.SetInt(KNIFE_OWNED_KEY + globalIndex, 1);

        if (equipOnUnlock)
            KnifeShop.EquippedIndex = globalIndex;   // PlayerPrefs.Save() andar hi hota hai

        PlayerPrefs.Save();
    }

    // Library me jo knives hain aur abhi unlock NAHI hui, unke global indexes
    private List<int> GetAvailableKnifeIndexes()
    {
        List<int> list = new List<int>();

        if (knifeLibrary == null || knifeLibrary.pages == null)
            return list;

        int perPage = Mathf.Max(1, knifeLibrary.slotsPerPage);

        for (int p = 0; p < knifeLibrary.pages.Length; p++)
        {
            KnifeLibrary.KnifePage page = knifeLibrary.pages[p];

            if (page == null || page.knives == null)
                continue;

            // Is page ki knives gift se allowed nahi hain to skip
            if (!IsKnifePageAllowed(p))
                continue;

            for (int s = 0; s < page.knives.Length && s < perPage; s++)
            {
                if (page.knives[s] == null)
                    continue;

                int global = p * perPage + s;

                if (!IsKnifeOwned(global))
                    list.Add(global);
            }
        }

        return list;
    }

    // pageIndex 0-based hai (Page 1 = 0). Inspector me numbers 1-based hain (Page 1 = 1).
    private bool IsKnifePageAllowed(int pageIndex)
    {
        // Khali list = saare pages allowed
        if (allowedKnifePages == null || allowedKnifePages.Length == 0)
            return true;

        int pageNumber = pageIndex + 1;

        for (int i = 0; i < allowedKnifePages.Length; i++)
        {
            if (allowedKnifePages[i] == pageNumber)
                return true;
        }

        return false;
    }

    // Knife prefab se icon nikalo (SpriteRenderer ya UI Image se)
    private Sprite GetKnifeSprite(int globalIndex)
    {
        if (knifeLibrary != null)
        {
            int perPage = Mathf.Max(1, knifeLibrary.slotsPerPage);

            GameObject prefab = knifeLibrary.GetPrefab(
                globalIndex / perPage,
                globalIndex % perPage
            );

            if (prefab != null)
            {
                SpriteRenderer sr = prefab.GetComponentInChildren<SpriteRenderer>(true);

                if (sr != null && sr.sprite != null)
                    return sr.sprite;

                Image img = prefab.GetComponentInChildren<Image>(true);

                if (img != null && img.sprite != null)
                    return img.sprite;
            }
        }

        return fallbackKnifeSprite;
    }

    // =========================================================
    // REWARD GENERATION
    // =========================================================

    private void GenerateAllRewards()
    {
        // Ek hi set me same knife do cards par na aaye
        List<int> availableKnives = GetAvailableKnifeIndexes();

        for (int i = 0; i < buttons.Length; i++)
        {
            GenerateReward(i, availableKnives);
        }
    }

    private void GenerateReward(int i, List<int> availableKnives)
    {
        // 1) Pehle knife ka chance
        if (availableKnives.Count > 0 &&
            Random.Range(0, 100) < knifeChance)
        {
            int pick = Random.Range(0, availableKnives.Count);

            rewardTypes[i] = TYPE_KNIFE;
            rewards[i] = availableKnives[pick];

            availableKnives.RemoveAt(pick);
            return;
        }

        // 2) Warna gems / coins
        int roll = Random.Range(0, 100);

        if (roll < gemChance)
        {
            rewardTypes[i] = TYPE_GEM;
            rewards[i] = Random.Range(gemMin, gemMax + 1);
        }
        else
        {
            rewardTypes[i] = TYPE_COIN;
            rewards[i] = Random.Range(coinMin, coinMax + 1);
        }
    }

    // =========================================================
    // PICK CARD
    // =========================================================

    private void OnPick(int index)
    {
        if (hasPicked)
            return;

        if (index < 0 || index >= buttons.Length)
            return;

        hasPicked = true;
        rewardCollected = false;

        selectedReward = rewards[index];
        selectedType = rewardTypes[index];

        if (audioSource != null && pickSound != null)
        {
            audioSource.PlayOneShot(pickSound);
        }

        DisableAllCards();

        // Card reveal hone tak collect buttons disabled rahenge.
        if (collectButton != null)
            collectButton.interactable = false;

        if (doubleCollectButton != null)
            doubleCollectButton.interactable = false;

        flipCoroutine = StartCoroutine(FlipCard(index));
    }

    private IEnumerator FlipCard(int index)
    {
        Transform card = buttons[index].transform;

        float duration = 0.25f;
        float time = 0f;

        while (time < duration)
        {
            float scale = Mathf.Lerp(1f, 0f, time / duration);

            card.localScale = new Vector3(scale, 1f, 1f);

            time += Time.deltaTime;
            yield return null;
        }

        card.localScale = new Vector3(0f, 1f, 1f);

        if (rewardImages != null &&
            index < rewardImages.Length &&
            rewardImages[index] != null)
        {
            rewardImages[index].sprite = GetSelectedRewardSprite();

            rewardImages[index].gameObject.SetActive(true);
        }

        if (rewardTexts != null &&
            index < rewardTexts.Length &&
            rewardTexts[index] != null)
        {
            rewardTexts[index].text = GetSelectedRewardLabel();
        }

        time = 0f;

        while (time < duration)
        {
            float scale = Mathf.Lerp(0f, 1f, time / duration);

            card.localScale = new Vector3(scale, 1f, 1f);

            time += Time.deltaTime;
            yield return null;
        }

        card.localScale = Vector3.one;

        if (doubleCollectText != null)
        {
            // Double sirf coins par
            doubleCollectText.text =
                selectedType == TYPE_COIN ? "2X " + selectedReward : "";
        }

        // Reward reveal hone ke baad buttons active honge.
        // Dono buttons visible hi rahenge.
        if (collectButton != null)
            collectButton.interactable = true;

        // Double collect sirf coins par (gems / knife par nahi)
        if (doubleCollectButton != null)
            doubleCollectButton.interactable = (selectedType == TYPE_COIN);

        flipCoroutine = null;
    }

    private Sprite GetSelectedRewardSprite()
    {
        if (selectedType == TYPE_KNIFE)
            return GetKnifeSprite(selectedReward);

        return selectedType == TYPE_GEM ? gemSprite : coinSprite;
    }

    private string GetSelectedRewardLabel()
    {
        if (selectedType == TYPE_KNIFE)
            return knifeRewardLabel;

        return selectedReward.ToString();
    }

    // =========================================================
    // COLLECT NORMAL REWARD
    // =========================================================

    private void OnCollect()
    {
        if (!hasPicked || rewardCollected || giftRewardSessionFinished)
            return;

        rewardCollected = true;
        giftRewardSessionFinished = true;

        DisableRewardButtons();

        if (audioSource != null && collectSound != null)
            audioSource.PlayOneShot(collectSound);

        if (selectedType == TYPE_COIN)
        {
            CoinManager.AddCoins(selectedReward);
        }
        else if (selectedType == TYPE_GEM)
        {
            CoinManager.AddGems(selectedReward);
        }
        else
        {
            CollectKnife();
        }

        // Cards reset nahi karenge, isliye isi gift se dobara reward nahi milega.
    }

    private void CollectKnife()
    {
        UnlockKnife(selectedReward);

        if (onKnifeUnlocked != null)
            onKnifeUnlocked.Invoke(selectedReward);

        Debug.Log("Knife unlocked (global index): " + selectedReward);
    }

    // =========================================================
    // DOUBLE COLLECT
    // =========================================================

    private void OnDoubleCollect()
    {
        if (!hasPicked || rewardCollected || giftRewardSessionFinished)
            return;

        // Double sirf coins par
        if (selectedType != TYPE_COIN)
            return;

        rewardCollected = true;
        giftRewardSessionFinished = true;

        DisableRewardButtons();

        if (audioSource != null && doubleCollectSound != null)
            audioSource.PlayOneShot(doubleCollectSound);

        CoinManager.AddCoins(selectedReward * 2);

        // Isi gift se dobara collect ya double collect nahi hoga.
    }

    // =========================================================
    // BUTTON HELPERS
    // =========================================================

    private void DisableAllCards()
    {
        foreach (Button card in buttons)
        {
            if (card == null)
                continue;

            card.interactable = false;

            ColorBlock cb = card.colors;
            cb.normalColor = new Color(0.6f, 0.6f, 0.6f);
            cb.disabledColor = new Color(0.6f, 0.6f, 0.6f);

            card.colors = cb;
        }
    }

    private void DisableRewardButtons()
    {
        if (collectButton != null)
            collectButton.interactable = false;

        if (doubleCollectButton != null)
            doubleCollectButton.interactable = false;
    }

    // =========================================================
    // RESET FOR NEXT PICK
    // =========================================================

    private void ResetForNextPick()
    {
        hasPicked = false;
        rewardCollected = false;

        GenerateAllRewards();

        for (int i = 0; i < buttons.Length; i++)
        {
            if (rewardImages != null &&
                i < rewardImages.Length &&
                rewardImages[i] != null)
            {
                rewardImages[i].gameObject.SetActive(false);
            }

            if (rewardTexts != null &&
                i < rewardTexts.Length &&
                rewardTexts[i] != null)
            {
                rewardTexts[i].text = "";
            }

            if (buttons[i] != null)
            {
                buttons[i].interactable = true;
                buttons[i].colors = defaultColors[i];
                buttons[i].transform.localScale = Vector3.one;
            }
        }

        if (doubleCollectText != null)
        {
            doubleCollectText.text = "";
        }

        // Buttons hamesha visible rahenge.
        if (collectButton != null)
        {
            collectButton.gameObject.SetActive(true);
            collectButton.interactable = false;
            collectButton.transform.localScale = Vector3.one;
        }

        if (doubleCollectButton != null)
        {
            doubleCollectButton.gameObject.SetActive(true);
            doubleCollectButton.interactable = false;
            doubleCollectButton.transform.localScale = Vector3.one;
        }
    }
}