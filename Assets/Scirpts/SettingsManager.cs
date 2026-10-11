// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;
// using TMPro;

// public class SettingsManager : MonoBehaviour
// {
//     // ============================================================
//     // AUDIO SOURCES
//     // ============================================================

//     [Header("===== AUDIO SOURCES =====")]

//     public AudioSource soundSource;
//     public AudioSource musicSource;

//     public AudioClip buttonClickClip;
//     public AudioClip backgroundMusic;

//     [Range(0f, 1f)]
//     public float buttonVolume = 1f;


//     // ============================================================
//     // BUTTON SOUND
//     // ============================================================

//     [Header("===== BUTTON SOUND =====")]

//     public Button[] soundButtons;


//     // ============================================================
//     // PLAY BUTTON
//     // ============================================================

//     [Header("===== PLAY BUTTON =====")]

//     public Button playButton;
//     public string gameSceneName = "GameScene";


//     // ============================================================
//     // PRIVACY, TERMS & ABOUT BUTTONS
//     // ============================================================

//     [Header("===== PRIVACY POLICY =====")]

//     public Button privacyButton;

//     public string privacyURL =
//         "https://bestjobsites7970.blogspot.com/2025/07/privacy-policy-this-privacy-policy.html";


//     [Header("===== TERMS AND CONDITIONS =====")]

//     public Button termsButton;

//     [Tooltip("Apni Terms & Conditions website ka URL daalo.")]
//     public string termsURL = "";


//     [Header("===== ABOUT =====")]

//     public Button aboutButton;

//     [Tooltip("Apna About / Developer page URL daalo.")]
//     public string aboutURL = "";


//     // ============================================================
//     // SOUND & MUSIC TOGGLES
//     // ============================================================

//     [Header("===== TOGGLES =====")]

//     public Toggle soundToggle;
//     public Toggle musicToggle;


//     // ============================================================
//     // SOUND SWITCH
//     // ============================================================

//     [Header("===== SOUND SWITCH =====")]

//     public RectTransform soundSwitchTrack;
//     public RectTransform soundHandle;
//     public Image soundSwitchBackground;

//     public float soundLeftX = -30f;
//     public float soundRightX = 30f;


//     // ============================================================
//     // MUSIC SWITCH
//     // ============================================================

//     [Header("===== MUSIC SWITCH =====")]

//     public RectTransform musicSwitchTrack;
//     public RectTransform musicHandle;
//     public Image musicSwitchBackground;

//     public float musicLeftX = -30f;
//     public float musicRightX = 30f;


//     // ============================================================
//     // SWITCH COLORS
//     // ============================================================

//     [Header("===== SWITCH COLORS =====")]

//     public Color switchOnColor =
//         new Color(0.15f, 0.9f, 0.25f);

//     public Color switchOffColor =
//         new Color(0.65f, 0.65f, 0.65f);


//     // ============================================================
//     // GRAPHICS QUALITY
//     // ============================================================

//     [Header("===== GRAPHICS QUALITY =====")]

//     [Tooltip("Settings panel ka Low-to-High quality slider.")]
//     public Slider qualitySlider;

//     [Tooltip("Optional: quality level ka naam dikhane wala TMP ya UI Text.")]
//     public TextMeshProUGUI qualityText;

//     private const string QUALITY_KEY = "FSM_QUALITY_LEVEL";

//     private bool isUpdatingQuality = false;


//     // ============================================================
//     // SAVED SETTINGS
//     // ============================================================

//     private const string SOUND_KEY = "FSM_SOUND_STATE";
//     private const string MUSIC_KEY = "FSM_MUSIC_STATE";

//     private bool soundEnabled = true;
//     private bool musicEnabled = true;

//     private Vector2 soundHandleCenter;
//     private Vector2 musicHandleCenter;

//     private bool soundCenterCached;
//     private bool musicCenterCached;


//     // ============================================================
//     // INITIALIZE
//     // ============================================================

//     private void Awake()
//     {
//         if (soundHandle != null)
//         {
//             soundHandleCenter = soundHandle.anchoredPosition;
//             soundCenterCached = true;
//         }

//         if (musicHandle != null)
//         {
//             musicHandleCenter = musicHandle.anchoredPosition;
//             musicCenterCached = true;
//         }

//         soundEnabled = PlayerPrefs.GetInt(SOUND_KEY, 1) == 1;
//         musicEnabled = PlayerPrefs.GetInt(MUSIC_KEY, 1) == 1;

//         if (soundSource != null)
//         {
//             soundSource.playOnAwake = false;
//             soundSource.loop = false;
//         }

//         if (musicSource != null)
//         {
//             musicSource.playOnAwake = false;
//             musicSource.loop = true;

//             if (backgroundMusic != null)
//                 musicSource.clip = backgroundMusic;
//         }

//         if (soundToggle != null)
//             soundToggle.SetIsOnWithoutNotify(soundEnabled);

//         if (musicToggle != null)
//             musicToggle.SetIsOnWithoutNotify(musicEnabled);

//         UpdateSoundSwitch();
//         UpdateMusicSwitch();

//         ApplyMusicState();

//         // Restore saved graphics quality.
//         LoadQualitySettings();
//     }


//     // ============================================================
//     // CONNECT BUTTONS
//     // ============================================================

//     private void Start()
//     {
//         if (soundToggle != null)
//             soundToggle.onValueChanged.AddListener(SetSoundEnabled);

//         if (musicToggle != null)
//             musicToggle.onValueChanged.AddListener(SetMusicEnabled);

//         if (soundButtons != null)
//         {
//             foreach (Button button in soundButtons)
//             {
//                 if (button != null)
//                     button.onClick.AddListener(PlayButtonSound);
//             }
//         }

//         if (playButton != null)
//             playButton.onClick.AddListener(PlayGame);

//         if (privacyButton != null)
//             privacyButton.onClick.AddListener(OpenPrivacyPolicy);

//         if (termsButton != null)
//             termsButton.onClick.AddListener(OpenTerms);

//         if (aboutButton != null)
//             aboutButton.onClick.AddListener(OpenAbout);

//         if (qualitySlider != null)
//             qualitySlider.onValueChanged.AddListener(SetGraphicsQuality);
//     }


//     // ============================================================
//     // BUTTON SOUND
//     // ============================================================

//     public void PlayButtonSound()
//     {
//         if (!soundEnabled)
//             return;

//         if (soundSource == null || buttonClickClip == null)
//             return;

//         soundSource.PlayOneShot(buttonClickClip, buttonVolume);
//     }


//     // ============================================================
//     // SOUND ON / OFF
//     // ============================================================

//     public void SetSoundEnabled(bool enabled)
//     {
//         soundEnabled = enabled;

//         PlayerPrefs.SetInt(SOUND_KEY, enabled ? 1 : 0);
//         PlayerPrefs.Save();

//         UpdateSoundSwitch();
//     }


//     // ============================================================
//     // MUSIC ON / OFF
//     // ============================================================

//     public void SetMusicEnabled(bool enabled)
//     {
//         musicEnabled = enabled;

//         PlayerPrefs.SetInt(MUSIC_KEY, enabled ? 1 : 0);
//         PlayerPrefs.Save();

//         ApplyMusicState();
//         UpdateMusicSwitch();
//     }


//     // ============================================================
//     // BACKGROUND MUSIC
//     // ============================================================

//     private void ApplyMusicState()
//     {
//         if (musicSource == null)
//             return;

//         if (musicEnabled)
//         {
//             if (backgroundMusic != null)
//                 musicSource.clip = backgroundMusic;

//             musicSource.loop = true;

//             if (musicSource.clip != null && !musicSource.isPlaying)
//                 musicSource.Play();
//         }
//         else
//         {
//             musicSource.Stop();
//         }
//     }


//     // ============================================================
//     // SWITCH POSITION
//     // ============================================================

//     private void SetSwitchPosition(
//         RectTransform handle,
//         bool isOn,
//         float leftX,
//         float rightX,
//         Vector2 centerPosition)
//     {
//         if (handle == null)
//             return;

//         float offsetX = isOn ? rightX : leftX;

//         handle.anchoredPosition = new Vector2(
//             centerPosition.x + offsetX,
//             centerPosition.y
//         );
//     }


//     private void UpdateSoundSwitch()
//     {
//         if (soundToggle != null)
//             soundToggle.SetIsOnWithoutNotify(soundEnabled);

//         if (soundCenterCached)
//         {
//             SetSwitchPosition(
//                 soundHandle,
//                 soundEnabled,
//                 soundLeftX,
//                 soundRightX,
//                 soundHandleCenter
//             );
//         }

//         if (soundSwitchBackground != null)
//         {
//             soundSwitchBackground.color =
//                 soundEnabled ? switchOnColor : switchOffColor;
//         }
//     }


//     private void UpdateMusicSwitch()
//     {
//         if (musicToggle != null)
//             musicToggle.SetIsOnWithoutNotify(musicEnabled);

//         if (musicCenterCached)
//         {
//             SetSwitchPosition(
//                 musicHandle,
//                 musicEnabled,
//                 musicLeftX,
//                 musicRightX,
//                 musicHandleCenter
//             );
//         }

//         if (musicSwitchBackground != null)
//         {
//             musicSwitchBackground.color =
//                 musicEnabled ? switchOnColor : switchOffColor;
//         }
//     }


//     // ============================================================
//     // PRIVACY POLICY
//     // ============================================================

//     public void OpenPrivacyPolicy()
//     {
//         OpenWebLink(privacyURL, "Privacy Policy");
//     }


//     // ============================================================
//     // TERMS & CONDITIONS
//     // ============================================================

//     public void OpenTerms()
//     {
//         OpenWebLink(termsURL, "Terms & Conditions");
//     }


//     // ============================================================
//     // ABOUT
//     // ============================================================

//     public void OpenAbout()
//     {
//         OpenWebLink(aboutURL, "About");
//     }


//     private void OpenWebLink(string url, string linkName)
//     {
//         if (string.IsNullOrWhiteSpace(url) ||
//             !url.StartsWith("https://"))
//         {
//             Debug.LogWarning(
//                 linkName + " ka valid HTTPS URL Inspector mein assign karo."
//             );
//             return;
//         }

//         Application.OpenURL(url);
//     }


//     // ============================================================
//     // LOAD SAVED GRAPHICS QUALITY
//     // ============================================================

//     private void LoadQualitySettings()
//     {
//         if (qualitySlider == null)
//             return;

//         int qualityCount = QualitySettings.names.Length;

//         if (qualityCount == 0)
//             return;

//         // Use the saved level if available; otherwise current level.
//         int defaultLevel = QualitySettings.GetQualityLevel();

//         int savedLevel = PlayerPrefs.GetInt(
//             QUALITY_KEY,
//             defaultLevel
//         );

//         savedLevel = Mathf.Clamp(savedLevel, 0, qualityCount - 1);

//         ApplyQualityLevel(savedLevel, false);

//         // Slider range is normalized from 0 to 1.
//         qualitySlider.minValue = 0f;
//         qualitySlider.maxValue = 1f;
//         qualitySlider.wholeNumbers = false;

//         float sliderValue = qualityCount > 1
//             ? (float)savedLevel / (qualityCount - 1)
//             : 0f;

//         isUpdatingQuality = true;
//         qualitySlider.SetValueWithoutNotify(sliderValue);
//         isUpdatingQuality = false;

//         UpdateQualityText(savedLevel);
//     }


//     // ============================================================
//     // CHANGE GRAPHICS QUALITY
//     // ============================================================

//     public void SetGraphicsQuality(float sliderValue)
//     {
//         if (isUpdatingQuality || qualitySlider == null)
//             return;

//         int qualityCount = QualitySettings.names.Length;

//         if (qualityCount == 0)
//             return;

//         int selectedLevel;

//         if (qualityCount == 1)
//         {
//             selectedLevel = 0;
//         }
//         else
//         {
//             selectedLevel = Mathf.RoundToInt(
//                 sliderValue * (qualityCount - 1)
//             );
//         }

//         selectedLevel = Mathf.Clamp(
//             selectedLevel,
//             0,
//             qualityCount - 1
//         );

//         ApplyQualityLevel(selectedLevel, true);
//     }


//     private void ApplyQualityLevel(int level, bool save)
//     {
//         int qualityCount = QualitySettings.names.Length;

//         if (qualityCount == 0)
//             return;

//         level = Mathf.Clamp(level, 0, qualityCount - 1);

//         QualitySettings.SetQualityLevel(level, true);

//         if (save)
//         {
//             PlayerPrefs.SetInt(QUALITY_KEY, level);
//             PlayerPrefs.Save();
//         }

//         UpdateQualityText(level);
//     }


//     private void UpdateQualityText(int level)
//     {
//         if (qualityText == null)
//             return;

//         string[] qualityNames = QualitySettings.names;

//         if (level < 0 || level >= qualityNames.Length)
//             return;

//         qualityText.text = qualityNames[level];
//     }


//     // ============================================================
//     // PLAY GAME SCENE
//     // ============================================================

//     public void PlayGame()
//     {
//         if (string.IsNullOrWhiteSpace(gameSceneName))
//         {
//             Debug.LogWarning(
//                 "SettingsManager: Game Scene Name assign karo."
//             );
//             return;
//         }

//         SceneManager.LoadScene(gameSceneName);
//     }


//     // ============================================================
//     // CLEANUP
//     // ============================================================

//     private void OnDestroy()
//     {
//         if (soundToggle != null)
//             soundToggle.onValueChanged.RemoveListener(SetSoundEnabled);

//         if (musicToggle != null)
//             musicToggle.onValueChanged.RemoveListener(SetMusicEnabled);

//         if (qualitySlider != null)
//             qualitySlider.onValueChanged.RemoveListener(SetGraphicsQuality);

//         if (soundButtons != null)
//         {
//             foreach (Button button in soundButtons)
//             {
//                 if (button != null)
//                     button.onClick.RemoveListener(PlayButtonSound);
//             }
//         }

//         if (playButton != null)
//             playButton.onClick.RemoveListener(PlayGame);

//         if (privacyButton != null)
//             privacyButton.onClick.RemoveListener(OpenPrivacyPolicy);

//         if (termsButton != null)
//             termsButton.onClick.RemoveListener(OpenTerms);

//         if (aboutButton != null)
//             aboutButton.onClick.RemoveListener(OpenAbout);
//     }
// }




using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    // ============================================================
    // AUDIO SOURCES
    // ============================================================

    [Header("===== AUDIO SOURCES =====")]

    public AudioSource soundSource;
    public AudioSource musicSource;

    public AudioClip buttonClickClip;
    public AudioClip backgroundMusic;

    [Range(0f, 1f)]
    public float buttonVolume = 1f;


    // ============================================================
    // BUTTON SOUND
    // ============================================================

    [Header("===== BUTTON SOUND =====")]

    public Button[] soundButtons;


    // ============================================================
    // PLAY BUTTON
    // ============================================================

    [Header("===== PLAY BUTTON =====")]

    public Button playButton;
    public string gameSceneName = "GameScene";


    // ============================================================
    // PRIVACY, TERMS & ABOUT BUTTONS
    // ============================================================

    [Header("===== PRIVACY POLICY =====")]

    public Button privacyButton;

    public string privacyURL =
        "https://bestjobsites7970.blogspot.com/2025/07/privacy-policy-this-privacy-policy.html";


    [Header("===== TERMS AND CONDITIONS =====")]

    public Button termsButton;

    [Tooltip("Apni Terms & Conditions website ka URL daalo.")]
    public string termsURL = "";


    [Header("===== ABOUT =====")]

    public Button aboutButton;

    [Tooltip("Apna About / Developer page URL daalo.")]
    public string aboutURL = "";


    // ============================================================
    // SOUND & MUSIC TOGGLES
    // ============================================================

    [Header("===== TOGGLES =====")]

    public Toggle soundToggle;
    public Toggle musicToggle;


    // ============================================================
    // SOUND SWITCH
    // ============================================================

    [Header("===== SOUND SWITCH =====")]

    public RectTransform soundSwitchTrack;
    public RectTransform soundHandle;
    public Image soundSwitchBackground;

    public float soundLeftX = -30f;
    public float soundRightX = 30f;


    // ============================================================
    // MUSIC SWITCH
    // ============================================================

    [Header("===== MUSIC SWITCH =====")]

    public RectTransform musicSwitchTrack;
    public RectTransform musicHandle;
    public Image musicSwitchBackground;

    public float musicLeftX = -30f;
    public float musicRightX = 30f;


    // ============================================================
    // SWITCH COLORS
    // ============================================================

    [Header("===== SWITCH COLORS =====")]

    public Color switchOnColor =
        new Color(0.15f, 0.9f, 0.25f);

    public Color switchOffColor =
        new Color(0.65f, 0.65f, 0.65f);


    // ============================================================
    // GRAPHICS QUALITY
    // ============================================================

    [Header("===== GRAPHICS QUALITY =====")]

    [Tooltip("Settings panel ka Low-to-High quality slider.")]
    public Slider qualitySlider;

    [Tooltip("Optional: quality level ka naam dikhane wala TMP ya UI Text.")]
    public TextMeshProUGUI qualityText;

    private const string QUALITY_KEY = "FSM_QUALITY_LEVEL";

    private bool isUpdatingQuality = false;


    // ============================================================
    // RESET GAME DATA
    // ============================================================

    [Header("===== RESET GAME DATA =====")]

    [Tooltip("Reset button.")]
    public Button resetButton;

    [Tooltip("Optional: 'Are you sure?' panel. Khaali chhodo to seedha reset hoga.")]
    public GameObject resetConfirmPanel;

    [Tooltip("Confirm panel ka Yes button.")]
    public Button resetYesButton;

    [Tooltip("Confirm panel ka No button.")]
    public Button resetNoButton;

    [Tooltip("On rakho to reset ke baad Sound, Music aur Graphics Quality ki settings bachi rahengi.")]
    public bool keepSoundMusicQuality = true;


    // ============================================================
    // SAVED SETTINGS
    // ============================================================

    private const string SOUND_KEY = "FSM_SOUND_STATE";
    private const string MUSIC_KEY = "FSM_MUSIC_STATE";

    private bool soundEnabled = true;
    private bool musicEnabled = true;

    private Vector2 soundHandleCenter;
    private Vector2 musicHandleCenter;

    private bool soundCenterCached;
    private bool musicCenterCached;


    // ============================================================
    // INITIALIZE
    // ============================================================

    private void Awake()
    {
        if (soundHandle != null)
        {
            soundHandleCenter = soundHandle.anchoredPosition;
            soundCenterCached = true;
        }

        if (musicHandle != null)
        {
            musicHandleCenter = musicHandle.anchoredPosition;
            musicCenterCached = true;
        }

        soundEnabled = PlayerPrefs.GetInt(SOUND_KEY, 1) == 1;
        musicEnabled = PlayerPrefs.GetInt(MUSIC_KEY, 1) == 1;

        if (soundSource != null)
        {
            soundSource.playOnAwake = false;
            soundSource.loop = false;
        }

        if (musicSource != null)
        {
            musicSource.playOnAwake = false;
            musicSource.loop = true;

            if (backgroundMusic != null)
                musicSource.clip = backgroundMusic;
        }

        if (soundToggle != null)
            soundToggle.SetIsOnWithoutNotify(soundEnabled);

        if (musicToggle != null)
            musicToggle.SetIsOnWithoutNotify(musicEnabled);

        UpdateSoundSwitch();
        UpdateMusicSwitch();

        ApplyMusicState();

        // Restore saved graphics quality.
        LoadQualitySettings();
    }


    // ============================================================
    // CONNECT BUTTONS
    // ============================================================

    private void Start()
    {
        if (soundToggle != null)
            soundToggle.onValueChanged.AddListener(SetSoundEnabled);

        if (musicToggle != null)
            musicToggle.onValueChanged.AddListener(SetMusicEnabled);

        if (soundButtons != null)
        {
            foreach (Button button in soundButtons)
            {
                if (button != null)
                    button.onClick.AddListener(PlayButtonSound);
            }
        }

        if (playButton != null)
            playButton.onClick.AddListener(PlayGame);

        if (privacyButton != null)
            privacyButton.onClick.AddListener(OpenPrivacyPolicy);

        if (termsButton != null)
            termsButton.onClick.AddListener(OpenTerms);

        if (aboutButton != null)
            aboutButton.onClick.AddListener(OpenAbout);

        if (qualitySlider != null)
            qualitySlider.onValueChanged.AddListener(SetGraphicsQuality);

        // ----- RESET -----

        if (resetConfirmPanel != null)
            resetConfirmPanel.SetActive(false);

        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetPressed);

        if (resetYesButton != null)
            resetYesButton.onClick.AddListener(ResetAllGameData);

        if (resetNoButton != null)
            resetNoButton.onClick.AddListener(CancelReset);
    }


    // ============================================================
    // BUTTON SOUND
    // ============================================================

    public void PlayButtonSound()
    {
        if (!soundEnabled)
            return;

        if (soundSource == null || buttonClickClip == null)
            return;

        soundSource.PlayOneShot(buttonClickClip, buttonVolume);
    }


    // ============================================================
    // SOUND ON / OFF
    // ============================================================

    public void SetSoundEnabled(bool enabled)
    {
        soundEnabled = enabled;

        PlayerPrefs.SetInt(SOUND_KEY, enabled ? 1 : 0);
        PlayerPrefs.Save();

        UpdateSoundSwitch();
    }


    // ============================================================
    // MUSIC ON / OFF
    // ============================================================

    public void SetMusicEnabled(bool enabled)
    {
        musicEnabled = enabled;

        PlayerPrefs.SetInt(MUSIC_KEY, enabled ? 1 : 0);
        PlayerPrefs.Save();

        ApplyMusicState();
        UpdateMusicSwitch();
    }


    // ============================================================
    // BACKGROUND MUSIC
    // ============================================================

    private void ApplyMusicState()
    {
        if (musicSource == null)
            return;

        if (musicEnabled)
        {
            if (backgroundMusic != null)
                musicSource.clip = backgroundMusic;

            musicSource.loop = true;

            if (musicSource.clip != null && !musicSource.isPlaying)
                musicSource.Play();
        }
        else
        {
            musicSource.Stop();
        }
    }


    // ============================================================
    // SWITCH POSITION
    // ============================================================

    private void SetSwitchPosition(
        RectTransform handle,
        bool isOn,
        float leftX,
        float rightX,
        Vector2 centerPosition)
    {
        if (handle == null)
            return;

        float offsetX = isOn ? rightX : leftX;

        handle.anchoredPosition = new Vector2(
            centerPosition.x + offsetX,
            centerPosition.y
        );
    }


    private void UpdateSoundSwitch()
    {
        if (soundToggle != null)
            soundToggle.SetIsOnWithoutNotify(soundEnabled);

        if (soundCenterCached)
        {
            SetSwitchPosition(
                soundHandle,
                soundEnabled,
                soundLeftX,
                soundRightX,
                soundHandleCenter
            );
        }

        if (soundSwitchBackground != null)
        {
            soundSwitchBackground.color =
                soundEnabled ? switchOnColor : switchOffColor;
        }
    }


    private void UpdateMusicSwitch()
    {
        if (musicToggle != null)
            musicToggle.SetIsOnWithoutNotify(musicEnabled);

        if (musicCenterCached)
        {
            SetSwitchPosition(
                musicHandle,
                musicEnabled,
                musicLeftX,
                musicRightX,
                musicHandleCenter
            );
        }

        if (musicSwitchBackground != null)
        {
            musicSwitchBackground.color =
                musicEnabled ? switchOnColor : switchOffColor;
        }
    }


    // ============================================================
    // PRIVACY POLICY
    // ============================================================

    public void OpenPrivacyPolicy()
    {
        OpenWebLink(privacyURL, "Privacy Policy");
    }


    // ============================================================
    // TERMS & CONDITIONS
    // ============================================================

    public void OpenTerms()
    {
        OpenWebLink(termsURL, "Terms & Conditions");
    }


    // ============================================================
    // ABOUT
    // ============================================================

    public void OpenAbout()
    {
        OpenWebLink(aboutURL, "About");
    }


    private void OpenWebLink(string url, string linkName)
    {
        if (string.IsNullOrWhiteSpace(url) ||
            !url.StartsWith("https://"))
        {
            Debug.LogWarning(
                linkName + " ka valid HTTPS URL Inspector mein assign karo."
            );
            return;
        }

        Application.OpenURL(url);
    }


    // ============================================================
    // LOAD SAVED GRAPHICS QUALITY
    // ============================================================

    private void LoadQualitySettings()
    {
        if (qualitySlider == null)
            return;

        int qualityCount = QualitySettings.names.Length;

        if (qualityCount == 0)
            return;

        // Use the saved level if available; otherwise current level.
        int defaultLevel = QualitySettings.GetQualityLevel();

        int savedLevel = PlayerPrefs.GetInt(
            QUALITY_KEY,
            defaultLevel
        );

        savedLevel = Mathf.Clamp(savedLevel, 0, qualityCount - 1);

        ApplyQualityLevel(savedLevel, false);

        // Slider range is normalized from 0 to 1.
        qualitySlider.minValue = 0f;
        qualitySlider.maxValue = 1f;
        qualitySlider.wholeNumbers = false;

        float sliderValue = qualityCount > 1
            ? (float)savedLevel / (qualityCount - 1)
            : 0f;

        isUpdatingQuality = true;
        qualitySlider.SetValueWithoutNotify(sliderValue);
        isUpdatingQuality = false;

        UpdateQualityText(savedLevel);
    }


    // ============================================================
    // CHANGE GRAPHICS QUALITY
    // ============================================================

    public void SetGraphicsQuality(float sliderValue)
    {
        if (isUpdatingQuality || qualitySlider == null)
            return;

        int qualityCount = QualitySettings.names.Length;

        if (qualityCount == 0)
            return;

        int selectedLevel;

        if (qualityCount == 1)
        {
            selectedLevel = 0;
        }
        else
        {
            selectedLevel = Mathf.RoundToInt(
                sliderValue * (qualityCount - 1)
            );
        }

        selectedLevel = Mathf.Clamp(
            selectedLevel,
            0,
            qualityCount - 1
        );

        ApplyQualityLevel(selectedLevel, true);
    }


    private void ApplyQualityLevel(int level, bool save)
    {
        int qualityCount = QualitySettings.names.Length;

        if (qualityCount == 0)
            return;

        level = Mathf.Clamp(level, 0, qualityCount - 1);

        QualitySettings.SetQualityLevel(level, true);

        if (save)
        {
            PlayerPrefs.SetInt(QUALITY_KEY, level);
            PlayerPrefs.Save();
        }

        UpdateQualityText(level);
    }


    private void UpdateQualityText(int level)
    {
        if (qualityText == null)
            return;

        string[] qualityNames = QualitySettings.names;

        if (level < 0 || level >= qualityNames.Length)
            return;

        qualityText.text = qualityNames[level];
    }


    // ============================================================
    // RESET ALL GAME DATA
    // ============================================================

    private void OnResetPressed()
    {
        if (resetConfirmPanel != null)
            resetConfirmPanel.SetActive(true);
        else
            ResetAllGameData();
    }


    private void CancelReset()
    {
        if (resetConfirmPanel != null)
            resetConfirmPanel.SetActive(false);
    }


    public void ResetAllGameData()
    {
        // Settings pehle se yaad rakho
        int sound = PlayerPrefs.GetInt(SOUND_KEY, 1);
        int music = PlayerPrefs.GetInt(MUSIC_KEY, 1);
        bool hadQuality = PlayerPrefs.HasKey(QUALITY_KEY);
        int quality = PlayerPrefs.GetInt(QUALITY_KEY, 0);

        // Sab kuch delete: level, coins, gems, knife skins, etc.
        // PlayerPrefs poore game ke saare scenes ke liye ek hi hota hai.
        PlayerPrefs.DeleteAll();

        if (keepSoundMusicQuality)
        {
            PlayerPrefs.SetInt(SOUND_KEY, sound);
            PlayerPrefs.SetInt(MUSIC_KEY, music);

            if (hadQuality)
                PlayerPrefs.SetInt(QUALITY_KEY, quality);
        }

        PlayerPrefs.Save();

        Time.timeScale = 1f;

        // Scene reload, taaki coin/gem texts aur baaki UI fresh data dikhayein
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    // ============================================================
    // PLAY GAME SCENE
    // ============================================================

    public void PlayGame()
    {
        if (string.IsNullOrWhiteSpace(gameSceneName))
        {
            Debug.LogWarning(
                "SettingsManager: Game Scene Name assign karo."
            );
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }


    // ============================================================
    // CLEANUP
    // ============================================================

    private void OnDestroy()
    {
        if (soundToggle != null)
            soundToggle.onValueChanged.RemoveListener(SetSoundEnabled);

        if (musicToggle != null)
            musicToggle.onValueChanged.RemoveListener(SetMusicEnabled);

        if (qualitySlider != null)
            qualitySlider.onValueChanged.RemoveListener(SetGraphicsQuality);

        if (soundButtons != null)
        {
            foreach (Button button in soundButtons)
            {
                if (button != null)
                    button.onClick.RemoveListener(PlayButtonSound);
            }
        }

        if (playButton != null)
            playButton.onClick.RemoveListener(PlayGame);

        if (privacyButton != null)
            privacyButton.onClick.RemoveListener(OpenPrivacyPolicy);

        if (termsButton != null)
            termsButton.onClick.RemoveListener(OpenTerms);

        if (aboutButton != null)
            aboutButton.onClick.RemoveListener(OpenAbout);

        if (resetButton != null)
            resetButton.onClick.RemoveListener(OnResetPressed);

        if (resetYesButton != null)
            resetYesButton.onClick.RemoveListener(ResetAllGameData);

        if (resetNoButton != null)
            resetNoButton.onClick.RemoveListener(CancelReset);
    }
}