using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// HomeGameManager
/// Buttons, scene names, game IDs aur level texts ki arrays rakhta hai.
/// Button[i] press hone par sceneNames[i] wala scene load hota hai.
/// levelTexts[i] me gameIds[i] ka saved level number dikhta hai.
///
/// Setup:
/// 1. Is script ko Home scene ke kisi GameObject (jaise "HomeGameManager") par lagao.
/// 2. Inspector me "Buttons" array me saare buttons drag karo.
/// 3. "Scene Names" array me utne hi scene names likho (same order me).
/// 4. "Game Ids" array me har scene ka wahi ID likho jo us scene ke OtherGameManager me "Game Id" me hai.
/// 5. "Level Texts" array me har button ka level TMP_Text drag karo (same order me).
/// 6. "Play Button" me play button drag karo aur "Game Scene Name" me scene ka naam likho.
/// 7. "GameManager2 Level Text" me us game ka level TMP_Text drag karo (purane GameManager2 wale game ke liye).
/// 8. File > Build Settings me saare scenes add karna mat bhoolna.
/// </summary>
public class HomeGameManager : MonoBehaviour
{
    [Header("Buttons (order same rakhna)")]
    [SerializeField] private Button[] buttons;

    [Header("Scene Names (buttons ke order me)")]
    [SerializeField] private string[] sceneNames;

    [Header("Game Ids (buttons ke order me - OtherGameManager ke Game Id jaisa)")]
    [SerializeField] private string[] gameIds;

    [Header("Level Texts (buttons ke order me)")]
    [SerializeField] private TMP_Text[] levelTexts;

    [Header("Play Button")]
    [Tooltip("Play button - gameSceneName wala scene load karega.")]
    [SerializeField] private Button playButton;

    [Tooltip("Play dabane par jo scene khulega (Build Settings me add hona chahiye).")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("GameManager2 (purana game) ka Level Text")]
    [Tooltip("GameManager2 wale game ka level yahan dikhega (PlayerPrefs 'CurrentLevelNumber' se).")]
    [SerializeField] private TMP_Text gameManager2LevelText;

    [Tooltip("Level text ka format. {0} ki jagah level number aayega.")]
    [SerializeField] private string levelTextFormat = "Level {0}";

    // GameManager / GameManager2 yahi key use karte hain
    private const string GAME_MANAGER2_LEVEL_KEY = "CurrentLevelNumber";

    private void Start()
    {
        if (buttons.Length != sceneNames.Length)
        {
            Debug.LogWarning("HomeGameManager: Buttons aur Scene Names ki count barabar nahi hai!");
        }

        if (buttons.Length != gameIds.Length)
        {
            Debug.LogWarning("HomeGameManager: Buttons aur Game Ids ki count barabar nahi hai!");
        }

        if (buttons.Length != levelTexts.Length)
        {
            Debug.LogWarning("HomeGameManager: Buttons aur Level Texts ki count barabar nahi hai!");
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;

            // Closure issue se bachne ke liye local copy
            int index = i;
            buttons[i].onClick.AddListener(() => OnButtonPressed(index));
        }

        if (playButton != null)
        {
            playButton.onClick.RemoveListener(PlayGame);
            playButton.onClick.AddListener(PlayGame);
        }

        RefreshLevelTexts();
    }

    private void OnEnable()
    {
        // Home scene wapas khulne par bhi texts update rahein
        RefreshLevelTexts();
    }

    // Har button ka level text uske gameId ke saved level se set karta hai
    public void RefreshLevelTexts()
    {
        // GameManager2 ka level text
        if (gameManager2LevelText != null)
        {
            int gm2Level = PlayerPrefs.GetInt(GAME_MANAGER2_LEVEL_KEY, 1);
            gameManager2LevelText.text = string.Format(levelTextFormat, gm2Level);
        }

        if (levelTexts == null || gameIds == null) return;

        for (int i = 0; i < levelTexts.Length; i++)
        {
            if (levelTexts[i] == null) continue;

            if (i >= gameIds.Length || string.IsNullOrEmpty(gameIds[i]))
            {
                Debug.LogWarning("HomeGameManager: Is level text ke liye Game Id nahi hai! Index: " + i);
                continue;
            }

            int level = OtherGameManager.GetLevelForGame(gameIds[i]);
            levelTexts[i].text = string.Format(levelTextFormat, level);
        }
    }

    private void OnButtonPressed(int index)
    {
        if (index < 0 || index >= sceneNames.Length)
        {
            Debug.LogError("HomeGameManager: Is button ke liye koi scene assign nahi hai! Index: " + index);
            return;
        }

        string sceneName = sceneNames[index];

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("HomeGameManager: Scene name khali hai! Index: " + index);
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    // Play button: gameSceneName wala scene load karta hai
    public void PlayGame()
    {
        if (string.IsNullOrWhiteSpace(gameSceneName))
        {
            Debug.LogWarning("HomeGameManager: Game Scene Name assign karo.");
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    private void OnDestroy()
    {
        if (playButton != null)
            playButton.onClick.RemoveListener(PlayGame);

        foreach (Button btn in buttons)
        {
            if (btn != null) btn.onClick.RemoveAllListeners();
        }
    }
}