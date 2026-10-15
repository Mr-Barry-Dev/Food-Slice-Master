using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RandomPickReward : MonoBehaviour
{
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

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip pickSound;
    public AudioClip collectSound;
    public AudioClip doubleCollectSound;

    private int[] rewards;
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
    // REWARD GENERATION
    // =========================================================

    private void GenerateAllRewards()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            GenerateReward(i);
        }
    }

    private void GenerateReward(int i)
    {
        int roll = Random.Range(0, 100);

        if (roll < gemChance)
        {
            rewardTypes[i] = 1; // Gems
            rewards[i] = Random.Range(gemMin, gemMax + 1);
        }
        else
        {
            rewardTypes[i] = 0; // Coins
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
            rewardImages[index].sprite =
                selectedType == 0 ? coinSprite : gemSprite;

            rewardImages[index].gameObject.SetActive(true);
        }

        if (rewardTexts != null &&
            index < rewardTexts.Length &&
            rewardTexts[index] != null)
        {
            rewardTexts[index].text = selectedReward.ToString();
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
            doubleCollectText.text = "2X " + selectedReward;
        }

        // Reward reveal hone ke baad buttons active honge.
        // Dono buttons visible hi rahenge.
        if (collectButton != null)
            collectButton.interactable = true;

        if (doubleCollectButton != null)
            doubleCollectButton.interactable = (selectedType == 0);

        flipCoroutine = null;
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

    if (selectedType == 0)
        CoinManager.AddCoins(selectedReward);
    else
        CoinManager.AddGems(selectedReward);

    // Cards reset nahi karenge, isliye isi gift se dobara reward nahi milega.
}

    // =========================================================
    // DOUBLE COLLECT
    // =========================================================

    private void OnDoubleCollect()
{
    if (!hasPicked || rewardCollected || giftRewardSessionFinished)
        return;

    if (selectedType != 0)
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
