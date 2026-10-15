using UnityEngine;
using UnityEngine.SceneManagement;

// Poore game ke liye Sound / Music ki global setting.
// Is script ko kisi GameObject par lagane ki zarurat NAHI hai,
// ye har scene load hote hi apne aap chalti hai.
public static class GameAudio
{
    // SettingsManager wali purani keys, taaki saved settings na khoyen
    public const string SOUND_KEY = "FSM_SOUND_STATE";
    public const string MUSIC_KEY = "FSM_MUSIC_STATE";


    // ============================================================
    // CURRENT STATE (PlayerPrefs se, har scene me same)
    // ============================================================

    public static bool SoundEnabled
    {
        get { return PlayerPrefs.GetInt(SOUND_KEY, 1) == 1; }
    }

    public static bool MusicEnabled
    {
        get { return PlayerPrefs.GetInt(MUSIC_KEY, 1) == 1; }
    }


    // ============================================================
    // CHANGE STATE
    // ============================================================

    public static void SetSoundEnabled(bool enabled)
    {
        PlayerPrefs.SetInt(SOUND_KEY, enabled ? 1 : 0);
        PlayerPrefs.Save();

        ApplyToScene();
    }

    public static void SetMusicEnabled(bool enabled)
    {
        PlayerPrefs.SetInt(MUSIC_KEY, enabled ? 1 : 0);
        PlayerPrefs.Save();

        ApplyToScene();
    }


    // ============================================================
    // APPLY TO ALL AUDIO SOURCES IN SCENE
    // Loop wale AudioSource = Music, baaki sab = Sound effects
    // ============================================================

    public static void ApplyToScene()
    {
        bool sound = SoundEnabled;
        bool music = MusicEnabled;

        AudioSource[] sources =
            Object.FindObjectsByType<AudioSource>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (AudioSource source in sources)
        {
            if (source == null)
                continue;

            bool isMusic = source.loop;

            source.mute = isMusic ? !music : !sound;
        }
    }


    // ============================================================
    // AUTO START (har scene load par setting apply)
    // ============================================================

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        ApplyToScene();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyToScene();
    }
}