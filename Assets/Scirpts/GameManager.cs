using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
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

    [Tooltip("GameManager2 se current level number lega.")]
    public GameManager2 gameManager2;

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

    [Tooltip("Win panel ka Next button - level +1 karega.")]
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
    // PANEL OPEN DELAY
    // ============================================================

    [Header("===== PANEL OPEN DELAY =====")]

    [Tooltip("Game Over panel show hone se pehle ka delay.")]
    public float gameOverDelay = 1f;

    [Tooltip("Win panel show hone se pehle ka delay.")]
    public float winDelay = 1f;


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
    // START
    // ============================================================

    private void Start()
    {
        // --------------------------------------------------------
        // HIDE GAME OVER
        // --------------------------------------------------------

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }


        // --------------------------------------------------------
        // HIDE WIN
        // --------------------------------------------------------

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }


        // --------------------------------------------------------
        // HIDE EXTRA PANEL
        // --------------------------------------------------------

        if (extraPanel != null)
        {
            extraPanel.SetActive(false);
        }


        // --------------------------------------------------------
        // RETRY BUTTON
        // --------------------------------------------------------

        if (retryButton != null)
        {
            retryButton.onClick.RemoveListener(RetryLevel);
            retryButton.onClick.AddListener(RetryLevel);
        }


        // --------------------------------------------------------
        // REVIVE BUTTON
        // --------------------------------------------------------

        if (reviveButton != null)
        {
            reviveButton.onClick.RemoveListener(ReviveGame);
            reviveButton.onClick.AddListener(ReviveGame);
        }


        // --------------------------------------------------------
        // NEXT BUTTON
        // --------------------------------------------------------

        if (nextButton != null)
        {
            nextButton.onClick.RemoveListener(GoToNextLevel);
            nextButton.onClick.AddListener(GoToNextLevel);
        }


        // --------------------------------------------------------
        // HOME BUTTONS
        // --------------------------------------------------------

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


        // --------------------------------------------------------
        // OPEN EXTRA PANEL BUTTON
        // --------------------------------------------------------

        if (openPanelButton != null)
        {
            openPanelButton.onClick.RemoveListener(OpenExtraPanel);
            openPanelButton.onClick.AddListener(OpenExtraPanel);
        }


        // --------------------------------------------------------
        // CLOSE EXTRA PANEL BUTTON
        // --------------------------------------------------------

        if (closePanelButton != null)
        {
            closePanelButton.onClick.RemoveListener(CloseExtraPanel);
            closePanelButton.onClick.AddListener(CloseExtraPanel);
        }


        // --------------------------------------------------------
        // INSTAGRAM BUTTON
        // --------------------------------------------------------

        if (instagramButton != null)
        {
            instagramButton.onClick.RemoveListener(OpenInstagram);
            instagramButton.onClick.AddListener(OpenInstagram);
        }


        // --------------------------------------------------------
        // GMAIL BUTTON
        // --------------------------------------------------------

        if (gmailButton != null)
        {
            gmailButton.onClick.RemoveListener(OpenGmail);
            gmailButton.onClick.AddListener(OpenGmail);
        }


        // --------------------------------------------------------
        // RATING BUTTON
        // --------------------------------------------------------

        if (ratingButton != null)
        {
            ratingButton.onClick.RemoveListener(OpenRating);
            ratingButton.onClick.AddListener(OpenRating);
        }


        // --------------------------------------------------------
        // EXIT BUTTON
        // --------------------------------------------------------

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(ExitGame);
            exitButton.onClick.AddListener(ExitGame);
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


        // --------------------------------------------------------
        // REVIVE BUTTON STATE
        // --------------------------------------------------------

        if (reviveButton != null)
        {
            bool canRevive =
                maxRevivesPerLevel <= 0 ||
                revivesUsed < maxRevivesPerLevel;

            reviveButton.gameObject.SetActive(canRevive);
        }


        // --------------------------------------------------------
        // HIDE WIN
        // --------------------------------------------------------

        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }


        // --------------------------------------------------------
        // SHOW GAME OVER AFTER DELAY
        // --------------------------------------------------------

        if (gameOverPanel != null)
        {
            gameOverRoutine = StartCoroutine(
                ShowPanelAfterDelay(
                    gameOverPanel,
                    gameOverDelay
                )
            );
        }
        else
        {
            Debug.LogWarning(
                "GameManager: Game Over Panel is NOT assigned."
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


        // --------------------------------------------------------
        // STOP PANEL ANIMATION (agar abhi chal rahi ho)
        // --------------------------------------------------------

        if (gameOverRoutine != null)
        {
            StopCoroutine(gameOverRoutine);
            gameOverRoutine = null;
        }


        Time.timeScale = 1f;

        gameOver = false;


        // --------------------------------------------------------
        // HIDE GAME OVER PANEL
        // --------------------------------------------------------

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            gameOverPanel.transform.localScale = Vector3.one;
        }


        // --------------------------------------------------------
        // SAME PHASE, SAME FRUITS - BAS NAYA KNIFE READY KARO
        // --------------------------------------------------------

        if (gameManager2 != null)
        {
            gameManager2.ReviveKnife();
        }
        else
        {
            Debug.LogWarning(
                "GameManager: GameManager2 is NOT assigned."
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


        // --------------------------------------------------------
        // HIDE GAME OVER
        // --------------------------------------------------------

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }


        // --------------------------------------------------------
        // SHOW WIN AFTER DELAY
        // --------------------------------------------------------

        if (winPanel != null)
        {
            StartCoroutine(
                ShowPanelAfterDelay(
                    winPanel,
                    winDelay
                )
            );
        }
        else
        {
            Debug.LogWarning(
                "GameManager: Win Panel is NOT assigned."
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
                "GameManager: Extra Panel is NOT assigned."
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
                "GameManager: Extra Panel is NOT assigned."
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


        // --------------------------------------------------------
        // OPEN PANEL
        // --------------------------------------------------------

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


        // --------------------------------------------------------
        // CLOSE PANEL
        // --------------------------------------------------------

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
                "GameManager: Home Scene Name is empty."
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
                "GameManager: Instagram URL is empty."
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
                "GameManager: Gmail address is empty."
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
                "GameManager: Rating URL is empty."
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
            "GameManager: Exit Game."
        );


        Application.Quit();
    }


    // ============================================================
    // DELAY THEN SHOW PANEL
    // ============================================================

    private IEnumerator ShowPanelAfterDelay(
        GameObject panel,
        float delay)
    {
        if (panel == null)
            yield break;


        // --------------------------------------------------------
        // WAIT
        // --------------------------------------------------------

        if (delay > 0f)
        {
            yield return new WaitForSecondsRealtime(delay);
        }


        // --------------------------------------------------------
        // PLAY PANEL ANIMATION
        // --------------------------------------------------------

        yield return StartCoroutine(
            AnimatePanel(panel)
        );
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


        // --------------------------------------------------------
        // START SCALE
        // --------------------------------------------------------

        rect.localScale =
            Vector3.one * startScale;


        float timer = 0f;


        // --------------------------------------------------------
        // ANIMATE
        // --------------------------------------------------------

        while (timer < animationDuration)
        {
            timer += Time.unscaledDeltaTime;


            float progress =
                Mathf.Clamp01(
                    timer /
                    animationDuration
                );


            // ----------------------------------------------------
            // SMOOTH EASE OUT
            // ----------------------------------------------------

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


        // --------------------------------------------------------
        // FINAL SCALE
        // --------------------------------------------------------

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
    // NEXT LEVEL
    // ============================================================

    public void GoToNextLevel()
    {
        Time.timeScale = 1f;

        int level = PlayerPrefs.GetInt("CurrentLevelNumber", 1);
        PlayerPrefs.SetInt("CurrentLevelNumber", level + 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    // ============================================================
    // SHOW RANDOM GAME OVER IMAGE
    // ============================================================

    private void ShowRandomGameOverImage()
    {
        if (gameOverImage == null)
        {
            Debug.LogWarning("GameManager: Game Over Image is NOT assigned.");
            return;
        }

        if (gameOverSprites == null || gameOverSprites.Length == 0)
        {
            Debug.LogWarning("GameManager: Game Over Sprites array is empty.");
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