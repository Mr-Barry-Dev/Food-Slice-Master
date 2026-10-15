// using System.Collections;
// using System.Collections.Generic;
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

//     [Tooltip("Button click sound ke liye. Khali chhodo to code khud AudioSource bana lega.")]
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

//     [Tooltip("Ye buttons dabane par click sound bajega.")]
//     public Button[] soundButtons;

//     [Tooltip("ON: scene ke SAARE buttons (inactive panels ke bhi) par click sound apne aap lag jayega. Array me buttons daalne ki zarurat nahi.")]
//     public bool autoFindAllButtons = true;

//     // Jin buttons par sound listener lag chuka hai
//     private readonly List<Button> trackedButtons = new List<Button>();


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
//     // RESET GAME DATA
//     // ============================================================

//     [Header("===== RESET GAME DATA =====")]

//     [Tooltip("Reset button.")]
//     public Button resetButton;

//     [Tooltip("Optional: 'Are you sure?' panel. Khaali chhodo to seedha reset hoga.")]
//     public GameObject resetConfirmPanel;

//     [Tooltip("Confirm panel ka Yes button.")]
//     public Button resetYesButton;

//     [Tooltip("Confirm panel ka No button.")]
//     public Button resetNoButton;

//     [Tooltip("On rakho to reset ke baad Sound, Music aur Graphics Quality ki settings bachi rahengi.")]
//     public bool keepSoundMusicQuality = true;


//     // ============================================================
//     // RESET PANEL ANIMATION
//     // ============================================================

//     [Header("===== RESET PANEL ANIMATION =====")]

//     [Tooltip("Panel open/close animation ka time (seconds).")]
//     public float resetPanelAnimDuration = 0.3f;

//     [Tooltip("Panel is scale se shuru hoga aur close hote waqt isi tak chhota hoga. 0 = button ke andar se nikalta hua.")]
//     [Range(0f, 1f)]
//     public float resetPanelStartScale = 0f;

//     [Tooltip("On rakho to panel Reset button ki jagah se nikalega aur No dabane par wapas usi me jayega.")]
//     public bool resetPanelFromButton = true;

//     private Coroutine resetPanelRoutine;
//     private Vector3 resetPanelHomePos;
//     private bool resetPanelHomeCached = false;


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

//         // Sound source assign nahi hai to apna bana lo
//         if (soundSource == null)
//         {
//             soundSource = gameObject.AddComponent<AudioSource>();

//             Debug.Log(
//                 "SettingsManager: Sound Source assign nahi tha, isliye automatic AudioSource bana diya."
//             );
//         }

//         soundSource.playOnAwake = false;
//         soundSource.loop = false;
//         soundSource.mute = false;
//         soundSource.spatialBlend = 0f;

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

//         RegisterSoundButtons();

//         if (privacyButton != null)
//             privacyButton.onClick.AddListener(OpenPrivacyPolicy);

//         if (termsButton != null)
//             termsButton.onClick.AddListener(OpenTerms);

//         if (aboutButton != null)
//             aboutButton.onClick.AddListener(OpenAbout);

//         if (qualitySlider != null)
//             qualitySlider.onValueChanged.AddListener(SetGraphicsQuality);

//         // ----- RESET -----

//         if (resetConfirmPanel != null)
//             resetConfirmPanel.SetActive(false);

//         if (resetButton != null)
//             resetButton.onClick.AddListener(OnResetPressed);

//         if (resetYesButton != null)
//             resetYesButton.onClick.AddListener(ResetAllGameData);

//         if (resetNoButton != null)
//             resetNoButton.onClick.AddListener(CancelReset);
//     }


//     // ============================================================
//     // REGISTER BUTTON SOUND
//     // ============================================================

//     private void RegisterSoundButtons()
//     {
//         // Inspector me diye gaye buttons
//         if (soundButtons != null)
//         {
//             foreach (Button button in soundButtons)
//             {
//                 TrackButton(button);
//             }
//         }

//         // Scene ke saare buttons (inactive wale bhi)
//         if (autoFindAllButtons)
//         {
//             Button[] allButtons =
//                 FindObjectsByType<Button>(
//                     FindObjectsInactive.Include,
//                     FindObjectsSortMode.None
//                 );

//             foreach (Button button in allButtons)
//             {
//                 TrackButton(button);
//             }
//         }
//     }


//     private void TrackButton(Button button)
//     {
//         if (button == null)
//             return;

//         // Ek button par sound do baar na lage
//         if (trackedButtons.Contains(button))
//             return;

//         button.onClick.AddListener(PlayButtonSound);
//         trackedButtons.Add(button);
//     }


//     // ============================================================
//     // BUTTON SOUND
//     // ============================================================

//     public void PlayButtonSound()
//     {
//         if (!soundEnabled)
//             return;

//         if (buttonClickClip == null)
//         {
//             Debug.LogWarning(
//                 "SettingsManager: Button Click Clip assign nahi hai."
//             );

//             return;
//         }

//         if (soundSource != null && soundSource.isActiveAndEnabled)
//         {
//             soundSource.PlayOneShot(buttonClickClip, buttonVolume);
//         }
//         else if (Camera.main != null)
//         {
//             // Sound Source inactive ho to bhi sound bajega
//             AudioSource.PlayClipAtPoint(
//                 buttonClickClip,
//                 Camera.main.transform.position,
//                 buttonVolume
//             );
//         }
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
//     // RESET ALL GAME DATA
//     // ============================================================

//     private void OnResetPressed()
//     {
//         if (resetConfirmPanel != null)
//             AnimateResetPanel(true);
//         else
//             ResetAllGameData();
//     }


//     private void CancelReset()
//     {
//         if (resetConfirmPanel != null)
//             AnimateResetPanel(false);
//     }


//     private void AnimateResetPanel(bool opening)
//     {
//         if (resetPanelRoutine != null)
//             StopCoroutine(resetPanelRoutine);

//         resetPanelRoutine = StartCoroutine(ResetPanelRoutine(opening));
//     }


//     private IEnumerator ResetPanelRoutine(bool opening)
//     {
//         RectTransform rect = resetConfirmPanel.GetComponent<RectTransform>();

//         if (rect == null)
//         {
//             resetConfirmPanel.SetActive(opening);
//             yield break;
//         }

//         // Panel ki original (design wali) position pehli baar yaad rakho
//         if (!resetPanelHomeCached)
//         {
//             resetPanelHomePos = rect.position;
//             resetPanelHomeCached = true;
//         }

//         // Panel kahan se nikalega / kahan jayega
//         Vector3 buttonPos = resetPanelHomePos;

//         if (resetPanelFromButton && resetButton != null)
//             buttonPos = resetButton.transform.position;

//         CanvasGroup group = resetConfirmPanel.GetComponent<CanvasGroup>();

//         if (group == null)
//             group = resetConfirmPanel.AddComponent<CanvasGroup>();

//         // Start values
//         Vector3 fromPos;
//         float fromScale;
//         float fromAlpha;

//         if (opening && !resetConfirmPanel.activeSelf)
//         {
//             resetConfirmPanel.SetActive(true);

//             fromPos = buttonPos;
//             fromScale = resetPanelStartScale;
//             fromAlpha = 0f;
//         }
//         else
//         {
//             // Animation beech me ruki ho to wahin se aage badho
//             fromPos = rect.position;
//             fromScale = rect.localScale.x;
//             fromAlpha = group.alpha;
//         }

//         // End values
//         Vector3 toPos = opening ? resetPanelHomePos : buttonPos;
//         float toScale = opening ? 1f : resetPanelStartScale;
//         float toAlpha = opening ? 1f : 0f;

//         rect.position = fromPos;
//         rect.localScale = Vector3.one * fromScale;
//         group.alpha = fromAlpha;

//         group.interactable = false;
//         group.blocksRaycasts = false;

//         float duration = Mathf.Max(0.01f, resetPanelAnimDuration);
//         float timer = 0f;

//         while (timer < duration)
//         {
//             timer += Time.unscaledDeltaTime;

//             float p = Mathf.Clamp01(timer / duration);

//             // Open: ease out, Close: ease in
//             float eased = opening
//                 ? 1f - Mathf.Pow(1f - p, 3f)
//                 : p * p * p;

//             rect.position = Vector3.Lerp(fromPos, toPos, eased);
//             rect.localScale = Vector3.one * Mathf.Lerp(fromScale, toScale, eased);
//             group.alpha = Mathf.Lerp(fromAlpha, toAlpha, eased);

//             yield return null;
//         }

//         rect.position = toPos;
//         rect.localScale = Vector3.one * toScale;
//         group.alpha = toAlpha;

//         if (opening)
//         {
//             group.interactable = true;
//             group.blocksRaycasts = true;
//         }
//         else
//         {
//             resetConfirmPanel.SetActive(false);

//             // Agli baar ke liye original state wapas set karo
//             rect.position = resetPanelHomePos;
//             rect.localScale = Vector3.one;
//             group.alpha = 1f;
//             group.interactable = true;
//             group.blocksRaycasts = true;
//         }

//         resetPanelRoutine = null;
//     }


//     public void ResetAllGameData()
//     {
//         // Settings pehle se yaad rakho
//         int sound = PlayerPrefs.GetInt(SOUND_KEY, 1);
//         int music = PlayerPrefs.GetInt(MUSIC_KEY, 1);
//         bool hadQuality = PlayerPrefs.HasKey(QUALITY_KEY);
//         int quality = PlayerPrefs.GetInt(QUALITY_KEY, 0);

//         // Sab kuch delete: level, coins, gems, knife skins, etc.
//         // PlayerPrefs poore game ke saare scenes ke liye ek hi hota hai.
//         PlayerPrefs.DeleteAll();

//         if (keepSoundMusicQuality)
//         {
//             PlayerPrefs.SetInt(SOUND_KEY, sound);
//             PlayerPrefs.SetInt(MUSIC_KEY, music);

//             if (hadQuality)
//                 PlayerPrefs.SetInt(QUALITY_KEY, quality);
//         }

//         PlayerPrefs.Save();

//         Time.timeScale = 1f;

//         // Scene reload, taaki coin/gem texts aur baaki UI fresh data dikhayein
//         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

//         foreach (Button button in trackedButtons)
//         {
//             if (button != null)
//                 button.onClick.RemoveListener(PlayButtonSound);
//         }

//         trackedButtons.Clear();

//         if (privacyButton != null)
//             privacyButton.onClick.RemoveListener(OpenPrivacyPolicy);

//         if (termsButton != null)
//             termsButton.onClick.RemoveListener(OpenTerms);

//         if (aboutButton != null)
//             aboutButton.onClick.RemoveListener(OpenAbout);

//         if (resetButton != null)
//             resetButton.onClick.RemoveListener(OnResetPressed);

//         if (resetYesButton != null)
//             resetYesButton.onClick.RemoveListener(ResetAllGameData);

//         if (resetNoButton != null)
//             resetNoButton.onClick.RemoveListener(CancelReset);
//     }
// }


using System.Collections;
using System.Collections.Generic;
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

    [Tooltip("Button click sound ke liye. Khali chhodo to code khud AudioSource bana lega.")]
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

    [Tooltip("Ye buttons dabane par click sound bajega.")]
    public Button[] soundButtons;

    [Tooltip("ON: scene ke SAARE buttons (inactive panels ke bhi) par click sound apne aap lag jayega. Array me buttons daalne ki zarurat nahi.")]
    public bool autoFindAllButtons = true;

    // Jin buttons par sound listener lag chuka hai
    private readonly List<Button> trackedButtons = new List<Button>();


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
    // RESET PANEL ANIMATION
    // ============================================================

    [Header("===== RESET PANEL ANIMATION =====")]

    [Tooltip("Panel open/close animation ka time (seconds).")]
    public float resetPanelAnimDuration = 0.3f;

    [Tooltip("Panel is scale se shuru hoga aur close hote waqt isi tak chhota hoga. 0 = button ke andar se nikalta hua.")]
    [Range(0f, 1f)]
    public float resetPanelStartScale = 0f;

    [Tooltip("On rakho to panel Reset button ki jagah se nikalega aur No dabane par wapas usi me jayega.")]
    public bool resetPanelFromButton = true;

    private Coroutine resetPanelRoutine;
    private Vector3 resetPanelHomePos;
    private bool resetPanelHomeCached = false;


    // ============================================================
    // SAVED SETTINGS
    // ============================================================

    private const string SOUND_KEY = GameAudio.SOUND_KEY;
    private const string MUSIC_KEY = GameAudio.MUSIC_KEY;

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

        // Sound source assign nahi hai to apna bana lo
        if (soundSource == null)
        {
            soundSource = gameObject.AddComponent<AudioSource>();

            Debug.Log(
                "SettingsManager: Sound Source assign nahi tha, isliye automatic AudioSource bana diya."
            );
        }

        soundSource.playOnAwake = false;
        soundSource.loop = false;
        soundSource.spatialBlend = 0f;

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

        RegisterSoundButtons();

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

        // Is scene ke saare AudioSources par saved Sound/Music setting lagao
        GameAudio.ApplyToScene();
    }


    // ============================================================
    // REGISTER BUTTON SOUND
    // ============================================================

    private void RegisterSoundButtons()
    {
        // Inspector me diye gaye buttons
        if (soundButtons != null)
        {
            foreach (Button button in soundButtons)
            {
                TrackButton(button);
            }
        }

        // Scene ke saare buttons (inactive wale bhi)
        if (autoFindAllButtons)
        {
            Button[] allButtons =
                FindObjectsByType<Button>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None
                );

            foreach (Button button in allButtons)
            {
                TrackButton(button);
            }
        }
    }


    private void TrackButton(Button button)
    {
        if (button == null)
            return;

        // Ek button par sound do baar na lage
        if (trackedButtons.Contains(button))
            return;

        button.onClick.AddListener(PlayButtonSound);
        trackedButtons.Add(button);
    }


    // ============================================================
    // BUTTON SOUND
    // ============================================================

    public void PlayButtonSound()
    {
        if (!soundEnabled)
            return;

        if (buttonClickClip == null)
        {
            Debug.LogWarning(
                "SettingsManager: Button Click Clip assign nahi hai."
            );

            return;
        }

        if (soundSource != null && soundSource.isActiveAndEnabled)
        {
            soundSource.PlayOneShot(buttonClickClip, buttonVolume);
        }
        else if (Camera.main != null)
        {
            // Sound Source inactive ho to bhi sound bajega
            AudioSource.PlayClipAtPoint(
                buttonClickClip,
                Camera.main.transform.position,
                buttonVolume
            );
        }
    }


    // ============================================================
    // SOUND ON / OFF
    // ============================================================

    public void SetSoundEnabled(bool enabled)
    {
        soundEnabled = enabled;

        // Global save + saare AudioSources par apply
        GameAudio.SetSoundEnabled(enabled);

        UpdateSoundSwitch();
    }


    // ============================================================
    // MUSIC ON / OFF
    // ============================================================

    public void SetMusicEnabled(bool enabled)
    {
        musicEnabled = enabled;

        // Global save + saare AudioSources par apply
        GameAudio.SetMusicEnabled(enabled);

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
            AnimateResetPanel(true);
        else
            ResetAllGameData();
    }


    private void CancelReset()
    {
        if (resetConfirmPanel != null)
            AnimateResetPanel(false);
    }


    private void AnimateResetPanel(bool opening)
    {
        if (resetPanelRoutine != null)
            StopCoroutine(resetPanelRoutine);

        resetPanelRoutine = StartCoroutine(ResetPanelRoutine(opening));
    }


    private IEnumerator ResetPanelRoutine(bool opening)
    {
        RectTransform rect = resetConfirmPanel.GetComponent<RectTransform>();

        if (rect == null)
        {
            resetConfirmPanel.SetActive(opening);
            yield break;
        }

        // Panel ki original (design wali) position pehli baar yaad rakho
        if (!resetPanelHomeCached)
        {
            resetPanelHomePos = rect.position;
            resetPanelHomeCached = true;
        }

        // Panel kahan se nikalega / kahan jayega
        Vector3 buttonPos = resetPanelHomePos;

        if (resetPanelFromButton && resetButton != null)
            buttonPos = resetButton.transform.position;

        CanvasGroup group = resetConfirmPanel.GetComponent<CanvasGroup>();

        if (group == null)
            group = resetConfirmPanel.AddComponent<CanvasGroup>();

        // Start values
        Vector3 fromPos;
        float fromScale;
        float fromAlpha;

        if (opening && !resetConfirmPanel.activeSelf)
        {
            resetConfirmPanel.SetActive(true);

            fromPos = buttonPos;
            fromScale = resetPanelStartScale;
            fromAlpha = 0f;
        }
        else
        {
            // Animation beech me ruki ho to wahin se aage badho
            fromPos = rect.position;
            fromScale = rect.localScale.x;
            fromAlpha = group.alpha;
        }

        // End values
        Vector3 toPos = opening ? resetPanelHomePos : buttonPos;
        float toScale = opening ? 1f : resetPanelStartScale;
        float toAlpha = opening ? 1f : 0f;

        rect.position = fromPos;
        rect.localScale = Vector3.one * fromScale;
        group.alpha = fromAlpha;

        group.interactable = false;
        group.blocksRaycasts = false;

        float duration = Mathf.Max(0.01f, resetPanelAnimDuration);
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float p = Mathf.Clamp01(timer / duration);

            // Open: ease out, Close: ease in
            float eased = opening
                ? 1f - Mathf.Pow(1f - p, 3f)
                : p * p * p;

            rect.position = Vector3.Lerp(fromPos, toPos, eased);
            rect.localScale = Vector3.one * Mathf.Lerp(fromScale, toScale, eased);
            group.alpha = Mathf.Lerp(fromAlpha, toAlpha, eased);

            yield return null;
        }

        rect.position = toPos;
        rect.localScale = Vector3.one * toScale;
        group.alpha = toAlpha;

        if (opening)
        {
            group.interactable = true;
            group.blocksRaycasts = true;
        }
        else
        {
            resetConfirmPanel.SetActive(false);

            // Agli baar ke liye original state wapas set karo
            rect.position = resetPanelHomePos;
            rect.localScale = Vector3.one;
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        resetPanelRoutine = null;
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

        foreach (Button button in trackedButtons)
        {
            if (button != null)
                button.onClick.RemoveListener(PlayButtonSound);
        }

        trackedButtons.Clear();

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