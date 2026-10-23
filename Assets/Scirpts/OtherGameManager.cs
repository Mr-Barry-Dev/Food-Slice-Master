using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class OtherGameManager : MonoBehaviour
{
    // ============================================================
    // PANELS
    // ============================================================

    [Header("===== PANELS =====")]

    [Tooltip("Game Over panel.")]
    public GameObject gameOverPanel;


    // ============================================================
    // GAME OVER RANDOM IMAGES
    // ============================================================

    [Header("===== GAME OVER RANDOM IMAGES =====")]

    [Tooltip("Game Over panel ke andar ka UI Image, jisme random sprite dikhega.")]
    public Image gameOverImage;

    [Tooltip("Game over par inme se ek random image show hogi.")]
    public Sprite[] gameOverSprites;

    [Tooltip("Ek hi image lagatar repeat na ho.")]
    public bool avoidSameImageTwice = true;

    private int lastGameOverImageIndex = -1;


    // ============================================================
    // LEVEL STATUS TEXTS
    // ============================================================

    [Header("===== LEVEL STATUS TEXTS =====")]

    [Tooltip("Incomplete level ka TMP Text.")]
    public TMP_Text incompleteLevelText;

    [Tooltip("Complete level ka TMP Text.")]
    public TMP_Text completeLevelText;

    [Tooltip("OtherGameManager2 se current level number lega.")]
    public OtherGameManager2 gameManager2;

    [Tooltip("Win panel.")]
    public GameObject winPanel;


    // ============================================================
    // EXTRA PANEL
    // ============================================================

    [Header("===== EXTRA PANEL =====")]

    [Tooltip("Button press karne par ye panel open hoga.")]
    public GameObject extraPanel;

    [Tooltip("Extra panel open karne wala button.")]
    public Button openPanelButton;

    [Tooltip("Extra panel close karne wala button.")]
    public Button closePanelButton;


    // ============================================================
    // PANEL BUTTONS
    // ============================================================

    [Header("===== PANEL BUTTONS =====")]

    [Tooltip("Retry button - current level restart karega.")]
    public Button retryButton;

    [Tooltip("Multiple Home buttons assign kar sakte ho.")]
    public Button[] homeButtons;

    [Tooltip("Win panel ka Next button - next level load karega.")]
    public Button nextButton;


    // ============================================================
    // REVIVE
    // ============================================================

    [Header("===== REVIVE =====")]

    [Tooltip("Revive button - jis phase me game over hua wahi se continue.")]
    public Button reviveButton;

    [Tooltip("Ek level me kitni baar revive kar sakte ho (0 = unlimited).")]
    public int maxRevivesPerLevel = 1;

    private int revivesUsed = 0;

    private Coroutine gameOverRoutine;


    // ============================================================
    // SOCIAL / OTHER BUTTONS
    // ============================================================

    [Header("===== SOCIAL / OTHER BUTTONS =====")]

    [Tooltip("Instagram button.")]
    public Button instagramButton;

    [Tooltip("Gmail button.")]
    public Button gmailButton;

    [Tooltip("Rating button.")]
    public Button ratingButton;

    [Tooltip("Exit button.")]
    public Button exitButton;


    // ============================================================
    // LINKS
    // ============================================================

    [Header("===== LINKS =====")]

    [Tooltip("Instagram profile link.")]
    public string instagramURL =
        "https://www.instagram.com/barryon_games/";

    [Tooltip("Gmail address.")]
    public string gmailAddress =
        "dr.strangh2002@gmail.com";

    [Tooltip("Google Play Store rating link.")]
    public string ratingURL =
        "https://play.google.com/store/apps/details?id=com.BarryOnGames.ThrowMaster&hl=en_IN";


    // ============================================================
    // HOME SCENE
    // ============================================================

    [Header("===== HOME SCENE =====")]

    [Tooltip("Home scene ka exact naam.")]
    public string homeSceneName = "Home";


    // ============================================================
    // GAME ID
    // ============================================================

    [Header("===== GAME ID =====")]

    [Tooltip("Har scene/game ka alag ID (jaise Game1, Game2). Isi ID se us game ka level number AUR gift progress save hota hai. HomeGameManager me same ID dalna.")]
    public string gameId = "Game1";


    // ============================================================
    // PANEL OPEN DELAY
    // ============================================================

    [Header("===== PANEL OPEN DELAY =====")]

    [Tooltip("Game Over panel show hone se pehle ka delay.")]
    public float gameOverDelay = 1f;

    [Tooltip("Win panel show hone se pehle ka delay.")]
    public float winDelay = 1f;


    // ============================================================
    // SOUNDS
    // ============================================================

    [Header("===== SOUNDS =====")]

    [Tooltip("Game Over panel open hone par ye sound bajega.")]
    public AudioClip gameOverClip;

    [Tooltip("Win panel open hone par ye sound bajega.")]
    public AudioClip winClip;

    [Range(0f, 1f)]
    public float panelSoundVolume = 1f;

    [Tooltip("Optional. Khali chhodo to Main Camera ki position par play hoga.")]
    public AudioSource audioSource;


    // ============================================================
    // PANEL ANIMATION
    // ============================================================

    [Header("===== PANEL ANIMATION =====")]

    [Tooltip("Panel animation duration.")]
    public float animationDuration = 0.35f;

    [Tooltip("Starting scale of panel.")]
    public float startScale = 0.7f;

    [Tooltip("Final scale of panel.")]
    public float finalScale = 1f;


    // ============================================================
    // EXTRA PANEL ANIMATION
    // ============================================================

    [Header("===== EXTRA PANEL ANIMATION =====")]

    [Tooltip("Extra panel animation duration.")]
    public float extraPanelAnimationDuration = 0.3f;

    [Tooltip("Extra panel starting scale.")]
    public float extraPanelStartScale = 0.7f;

    [Tooltip("Extra panel final scale.")]
    public float extraPanelFinalScale = 1f;


    // ============================================================
    // GAME STATE
    // ============================================================

    private bool gameOver = false;

    private bool levelWon = false;


    // ============================================================
    // LEVEL COMPLETE REWARDS
    // ============================================================

    [Header("===== LEVEL COMPLETE REWARDS =====")]

    [Tooltip("Coins reward show karne wala TMP Text.")]
    public TMP_Text coinsRewardText;

    [Tooltip("Gems reward show karne wala TMP Text.")]
    public TMP_Text gemsRewardText;

    [Header("===== RANDOM REWARD SETTINGS =====")]

    public int minCoinsReward = 30;
    public int maxCoinsReward = 60;

    public int minGemsReward = 10;
    public int maxGemsReward = 20;

    private bool rewardGiven = false;


    // ============================================================
    // GIFT PANEL
    // ============================================================

    [Header("===== GIFT PANEL =====")]

    [Tooltip("Win panel ke baad apne aap khulne wala Gift panel (GiftProgress script isi par ya iske andar lagao). Scene me inactive rakho.")]
    public GameObject giftPanel;

    [Tooltip("Win panel poora dikhne ke itne seconds baad Gift panel khulega.")]
    public float giftPanelDelay = 2f;

    [Tooltip("Gift panel ka Close button. Dabane par gift panel band hoga aur win panel wapas khulega.")]
    public Button giftCloseButton;

    [Tooltip("Optional: khali chhodo to giftPanel me se apne aap mil jayega.")]
    public GiftProgress giftProgress;


    // ============================================================
    // START
    // ============================================================

    private void Start()
    {
        // ----- GIFT PROGRESS: is game ke ID ke hisab se alag save/load -----
        // (Ye sabse pehle hona chahiye, GiftProgress ke Start/Play se pehle)
        GiftProgress.SetActiveGame(gameId);


        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }


        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }


        if (extraPanel != null)
        {
            extraPanel.SetActive(false);
        }


        if (retryButton != null)
        {
            retryButton.onClick.RemoveListener(RetryLevel);
            retryButton.onClick.AddListener(RetryLevel);
        }


        if (reviveButton != null)
        {
            reviveButton.onClick.RemoveListener(ReviveGame);
            reviveButton.onClick.AddListener(ReviveGame);
        }


        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(GoToNextLevel);
            nextButton.onClick.AddListener(GoToNextLevel);
        }


        if (homeButtons != null)
        {
            foreach (Button button in homeButtons)
            {
                if (button != null)
                {
                    button.onClick.RemoveListener(GoToHome);
                    button.onClick.AddListener(GoToHome);
                }
            }
        }


        if (openPanelButton != null)
        {
            openPanelButton.onClick.RemoveListener(OpenExtraPanel);
            openPanelButton.onClick.AddListener(OpenExtraPanel);
        }


        if (closePanelButton != null)
        {
            closePanelButton.onClick.RemoveListener(CloseExtraPanel);
            closePanelButton.onClick.AddListener(CloseExtraPanel);
        }


        if (instagramButton != null)
        {
            instagramButton.onClick.RemoveListener(OpenInstagram);
            instagramButton.onClick.AddListener(OpenInstagram);
        }


        if (gmailButton != null)
        {
            gmailButton.onClick.RemoveListener(OpenGmail);
            gmailButton.onClick.AddListener(OpenGmail);
        }


        if (ratingButton != null)
        {
            ratingButton.onClick.RemoveListener(OpenRating);
            ratingButton.onClick.AddListener(OpenRating);
        }


        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(ExitGame);
            exitButton.onClick.AddListener(ExitGame);
        }


        // ----- GIFT PANEL -----

        if (giftPanel != null)
        {
            giftPanel.SetActive(false);

            if (giftProgress == null)
            {
                giftProgress =
                    giftPanel.GetComponentInChildren<GiftProgress>(true);
            }
        }


        if (giftCloseButton != null)
        {
            giftCloseButton.onClick.RemoveListener(CloseGiftPanel);
            giftCloseButton.onClick.AddListener(CloseGiftPanel);
        }


        // New Gift Panel band hone par bhi gift panel off, win panel on
        if (giftProgress != null)
        {
            giftProgress.onNewGiftPanelClosed.RemoveListener(CloseGiftPanel);
            giftProgress.onNewGiftPanelClosed.AddListener(CloseGiftPanel);

            // 100% se kam ho to animation ke baad gift panel apne aap band, win panel on
            giftProgress.onAutoClose.RemoveListener(CloseGiftPanel);
            giftProgress.onAutoClose.AddListener(CloseGiftPanel);
        }
    }


    // ============================================================
    // SHOW GAME OVER
    // ============================================================

    public void ShowGameOver()
    {
        if (gameOver || levelWon)
            return;


        gameOver = true;

        ShowRandomGameOverImage();


        if (reviveButton != null)
        {
            bool canRevive =
                maxRevivesPerLevel <= 0 ||
                revivesUsed < maxRevivesPerLevel;

            reviveButton.gameObject.SetActive(canRevive);
        }


        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }


        if (gameOverPanel != null)
        {
            gameOverRoutine = StartCoroutine(
                ShowPanelAfterDelay(
                    gameOverPanel,
                    gameOverDelay,
                    gameOverClip
                )
            );
        }
        else
        {
            Debug.LogWarning(
                "OtherGameManager: Game Over Panel is NOT assigned."
            );
        }
    }


    // ============================================================
    // REVIVE GAME
    // ============================================================

    public void ReviveGame()
    {
        if (!gameOver || levelWon)
            return;


        if (maxRevivesPerLevel > 0 && revivesUsed >= maxRevivesPerLevel)
            return;


        revivesUsed++;


        if (gameOverRoutine != null)
        {
            StopCoroutine(gameOverRoutine);
            gameOverRoutine = null;
        }


        Time.timeScale = 1f;

        gameOver = false;


        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            gameOverPanel.transform.localScale = Vector3.one;
        }


        // SAME PHASE, SAME FRUITS - BAS NAYA KNIFE READY KARO

        if (gameManager2 != null)
        {
            gameManager2.ReviveKnife();
        }
        else
        {
            Debug.LogWarning(
                "OtherGameManager: OtherGameManager2 is NOT assigned."
            );
        }
    }


    // ============================================================
    // SHOW WIN
    // ============================================================

    public void ShowWin()
    {
        if (gameOver || levelWon)
            return;


        levelWon = true;


        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }


        if (winPanel != null)
        {
            StartCoroutine(WinThenGiftRoutine());
        }
        else
        {
            Debug.LogWarning(
                "OtherGameManager: Win Panel is NOT assigned."
            );
        }
    }


    // ============================================================
    // OPEN EXTRA PANEL
    // ============================================================

    public void OpenExtraPanel()
    {
        if (extraPanel == null)
        {
            Debug.LogWarning(
                "OtherGameManager: Extra Panel is NOT assigned."
            );

            return;
        }


        StopCoroutine(nameof(AnimateExtraPanel));


        StartCoroutine(
            AnimateExtraPanel(
                extraPanel,
                true
            )
        );
    }


    // ============================================================
    // CLOSE EXTRA PANEL
    // ============================================================

    public void CloseExtraPanel()
    {
        if (extraPanel == null)
        {
            Debug.LogWarning(
                "OtherGameManager: Extra Panel is NOT assigned."
            );

            return;
        }


        StopCoroutine(nameof(AnimateExtraPanel));


        StartCoroutine(
            AnimateExtraPanel(
                extraPanel,
                false
            )
        );
    }


    // ============================================================
    // EXTRA PANEL ANIMATION
    // ============================================================

    private IEnumerator AnimateExtraPanel(
        GameObject panel,
        bool opening)
    {
        if (panel == null)
            yield break;


        RectTransform rect =
            panel.GetComponent<RectTransform>();


        if (rect == null)
            yield break;


        if (opening)
        {
            panel.SetActive(true);

            rect.localScale =
                Vector3.one * extraPanelStartScale;


            float timer = 0f;


            while (timer < extraPanelAnimationDuration)
            {
                timer += Time.unscaledDeltaTime;


                float progress =
                    Mathf.Clamp01(
                        timer /
                        extraPanelAnimationDuration
                    );


                float smoothProgress =
                    1f - Mathf.Pow(1f - progress, 3f);


                float scale =
                    Mathf.Lerp(
                        extraPanelStartScale,
                        extraPanelFinalScale,
                        smoothProgress
                    );


                rect.localScale =
                    Vector3.one * scale;


                yield return null;
            }


            rect.localScale =
                Vector3.one * extraPanelFinalScale;
        }
        else
        {
            float timer = 0f;


            Vector3 currentScale =
                rect.localScale;


            while (timer < extraPanelAnimationDuration)
            {
                timer += Time.unscaledDeltaTime;


                float progress =
                    Mathf.Clamp01(
                        timer /
                        extraPanelAnimationDuration
                    );


                float smoothProgress =
                    1f - Mathf.Pow(1f - progress, 3f);


                float scale =
                    Mathf.Lerp(
                        currentScale.x,
                        extraPanelStartScale,
                        smoothProgress
                    );


                rect.localScale =
                    Vector3.one * scale;


                yield return null;
            }


            rect.localScale =
                Vector3.one * extraPanelStartScale;


            panel.SetActive(false);
        }
    }


    // ============================================================
    // RETRY CURRENT LEVEL
    // ============================================================

    public void RetryLevel()
    {
        Time.timeScale = 1f;


        Scene currentScene =
            SceneManager.GetActiveScene();


        SceneManager.LoadScene(
            currentScene.name
        );
    }


    // ============================================================
    // GO TO HOME
    // ============================================================

    public void GoToHome()
    {
        Time.timeScale = 1f;


        if (string.IsNullOrEmpty(homeSceneName))
        {
            Debug.LogWarning(
                "OtherGameManager: Home Scene Name is empty."
            );

            return;
        }


        SceneManager.LoadScene(
            homeSceneName
        );
    }


    // ============================================================
    // OPEN INSTAGRAM
    // ============================================================

    public void OpenInstagram()
    {
        if (string.IsNullOrEmpty(instagramURL))
        {
            Debug.LogWarning(
                "OtherGameManager: Instagram URL is empty."
            );

            return;
        }


        Application.OpenURL(instagramURL);
    }


    // ============================================================
    // OPEN GMAIL
    // ============================================================

    public void OpenGmail()
    {
        if (string.IsNullOrEmpty(gmailAddress))
        {
            Debug.LogWarning(
                "OtherGameManager: Gmail address is empty."
            );

            return;
        }


        string mailURL =
            "mailto:" + gmailAddress;


        Application.OpenURL(mailURL);
    }


    // ============================================================
    // OPEN RATING
    // ============================================================

    public void OpenRating()
    {
        if (string.IsNullOrEmpty(ratingURL))
        {
            Debug.LogWarning(
                "OtherGameManager: Rating URL is empty."
            );

            return;
        }


        Application.OpenURL(ratingURL);
    }


    // ============================================================
    // EXIT GAME
    // ============================================================

    public void ExitGame()
    {
        Time.timeScale = 1f;


        Debug.Log(
            "OtherGameManager: Exit Game."
        );


        Application.Quit();
    }


    // ============================================================
    // DELAY THEN SHOW PANEL (+ SOUND)
    // ============================================================

    private IEnumerator ShowPanelAfterDelay(
        GameObject panel,
        float delay,
        AudioClip clip = null)
    {
        if (panel == null)
            yield break;


        if (delay > 0f)
        {
            yield return new WaitForSecondsRealtime(delay);
        }


        // Panel khulte hi sound
        PlayPanelSound(clip);


        yield return StartCoroutine(
            AnimatePanel(panel)
        );
    }


    // ============================================================
    // PLAY PANEL SOUND
    // ============================================================

    private void PlayPanelSound(AudioClip clip)
    {
        if (clip == null)
            return;


        // Settings me Sound OFF hai to kuch na bajao
        if (!GameAudio.SoundEnabled)
            return;


        if (audioSource != null)
        {
            audioSource.PlayOneShot(
                clip,
                panelSoundVolume
            );
        }
        else if (Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(
                clip,
                Camera.main.transform.position,
                panelSoundVolume
            );
        }
    }


    // ============================================================
    // PANEL ANIMATION
    // ============================================================

    private IEnumerator AnimatePanel(
        GameObject panel)
    {
        if (panel == null)
            yield break;


        panel.SetActive(true);


        RectTransform rect =
            panel.GetComponent<RectTransform>();


        if (rect == null)
            yield break;


        rect.localScale =
            Vector3.one * startScale;


        float timer = 0f;


        while (timer < animationDuration)
        {
            timer += Time.unscaledDeltaTime;


            float progress =
                Mathf.Clamp01(
                    timer /
                    animationDuration
                );


            float smoothProgress =
                1f - Mathf.Pow(1f - progress, 3f);


            float scale =
                Mathf.Lerp(
                    startScale,
                    finalScale,
                    smoothProgress
                );


            rect.localScale =
                Vector3.one * scale;


            yield return null;
        }


        rect.localScale =
            Vector3.one * finalScale;
    }


    // ============================================================
    // GAME OVER STATE
    // ============================================================

    public bool IsGameOver()
    {
        return gameOver;
    }


    // ============================================================
    // WIN STATE
    // ============================================================

    public bool IsLevelWon()
    {
        return levelWon;
    }


    // ============================================================
    // GAME ENDED
    // ============================================================

    public bool IsGameEnded()
    {
        return gameOver || levelWon;
    }


    // ============================================================
    // WIN PANEL -> (2 sec baad) GIFT PANEL
    // ============================================================

    private IEnumerator WinThenGiftRoutine()
    {
        // Win panel poora show hone tak ruko
        yield return StartCoroutine(
            ShowPanelAfterDelay(
                winPanel,
                winDelay,
                winClip
            )
        );


        if (giftPanel == null)
            yield break;


        if (giftPanelDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(giftPanelDelay);
        }


        OpenGiftPanel();
    }


    // Gift panel pehle poora khule, USKE BAAD percentage + color animation chale
    private IEnumerator OpenGiftPanelRoutine()
    {
        yield return StartCoroutine(
            AnimatePanel(giftPanel)
        );


        // Panel khulte waqt band ho gaya ho to kuch mat karo
        if (giftPanel == null || !giftPanel.activeSelf)
            yield break;


        if (giftProgress != null)
        {
            giftProgress.Play();
        }
    }


    // Win panel off, gift panel on
    private void OpenGiftPanel()
    {
        if (giftPanel == null)
            return;


        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }


        StartCoroutine(OpenGiftPanelRoutine());
    }


    // Gift panel off, win panel wapas on
    public void CloseGiftPanel()
    {
        if (giftPanel == null || !giftPanel.activeSelf)
            return;


        giftPanel.SetActive(false);

        giftPanel.transform.localScale = Vector3.one;


        if (winPanel != null)
        {
            StartCoroutine(
                AnimatePanel(winPanel)
            );
        }
    }


    // ============================================================
    // UPDATE LEVEL STATUS TEXT
    // ============================================================

    public void UpdateLevelStatusText(int levelNumber, bool completed)
    {
        if (incompleteLevelText != null)
        {
            incompleteLevelText.gameObject.SetActive(!completed);

            incompleteLevelText.text =
                "Level " + levelNumber + "\nIncomplete";
        }

        if (completeLevelText != null)
        {
            completeLevelText.gameObject.SetActive(completed);

            completeLevelText.text =
                "Level " + levelNumber;
        }
    }


    // ============================================================
    // GIVE LEVEL COMPLETE REWARDS
    // ============================================================

    public void GiveLevelCompleteRewards()
    {
        if (rewardGiven)
            return;

        rewardGiven = true;

        // Level complete hote hi next level save ho jaye
        // (Home dabao ya Next, dono me progress safe rahe)
        SaveNextLevel();

        // Gift progress me 1 level add (4 level = 100%)
        // Ye is game (gameId) ke apne progress me add hoga
        GiftProgress.SetActiveGame(gameId);
        GiftProgress.AddLevelProgress();

        int coins = Random.Range(
            minCoinsReward,
            maxCoinsReward + 1
        );

        int gems = Random.Range(
            minGemsReward,
            maxGemsReward + 1
        );

        CoinManager.AddCoins(coins);
        CoinManager.AddGems(gems);

        Debug.Log("Saved Total -> Coins: " + CoinManager.GetCoins() + ", Gems: " + CoinManager.GetGems());

        if (coinsRewardText != null)
        {
            coinsRewardText.text = "+" + coins;
            coinsRewardText.gameObject.SetActive(true);
        }

        if (gemsRewardText != null)
        {
            gemsRewardText.text = "+" + gems;
            gemsRewardText.gameObject.SetActive(true);
        }
    }


    // ============================================================
    // SAVE NEXT LEVEL (level win hone par ek hi baar)
    // ============================================================

    private void SaveNextLevel()
    {
        int level = GetCurrentLevel();

        PlayerPrefs.SetInt(GetLevelKey(gameId), level + 1);
        PlayerPrefs.Save();
    }


    // ============================================================
    // NEXT LEVEL
    // ============================================================

    public void GoToNextLevel()
    {
        Time.timeScale = 1f;

        // Level pehle hi win par save ho chuka hai, yahan sirf scene reload
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    // ============================================================
    // LEVEL KEY / LEVEL NUMBER (gameId ke hisab se)
    // ============================================================

    // PlayerPrefs key - har gameId ka alag
    public static string GetLevelKey(string id)
    {
        return "CurrentLevelNumber_" + id;
    }

    // Kisi bhi gameId ka saved level number (HomeGameManager bhi isse use karta hai)
    public static int GetLevelForGame(string id)
    {
        return PlayerPrefs.GetInt(GetLevelKey(id), 1);
    }

    // Is scene ka current level number
    public int GetCurrentLevel()
    {
        return GetLevelForGame(gameId);
    }


    // ============================================================
    // SHOW RANDOM GAME OVER IMAGE
    // ============================================================

    private void ShowRandomGameOverImage()
    {
        if (gameOverImage == null)
        {
            Debug.LogWarning("OtherGameManager: Game Over Image is NOT assigned.");
            return;
        }

        if (gameOverSprites == null || gameOverSprites.Length == 0)
        {
            Debug.LogWarning("OtherGameManager: Game Over Sprites array is empty.");
            return;
        }

        int index = Random.Range(0, gameOverSprites.Length);

        if (avoidSameImageTwice && gameOverSprites.Length > 1)
        {
            int tries = 0;

            while (index == lastGameOverImageIndex && tries < 10)
            {
                index = Random.Range(0, gameOverSprites.Length);
                tries++;
            }
        }

        lastGameOverImageIndex = index;

        if (gameOverSprites[index] != null)
        {
            gameOverImage.sprite = gameOverSprites[index];
            gameOverImage.enabled = true;
        }
    }
}