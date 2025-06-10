// AudioManager.cs
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    public AudioMixer mainMixer;      // ваш AudioMixer
    private const string MIXER_MUSIC_PARAM = "MusicVolume";
    private const string MIXER_SFX_PARAM   = "SFXVolume";

    private bool isMusicMuted = false;
    private bool isSFXMuted   = false;

    private const string PREF_MUSIC_VOLUME = "MusicVolume";
    private const string PREF_SFX_VOLUME   = "SFXVolume";
    private const string PREF_MUSIC_MUTE   = "MusicMuted";
    private const string PREF_SFX_MUTE     = "SFXMuted";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }

    private void LoadSettings()
    {
        // Получаем сохранённые значения [0..1], по умолчанию 0.5
        float mv = PlayerPrefs.GetFloat(PREF_MUSIC_VOLUME, 0.5f);
        float sv = PlayerPrefs.GetFloat(PREF_SFX_VOLUME,   0.5f);
        isMusicMuted = PlayerPrefs.GetInt(PREF_MUSIC_MUTE, 0) == 1;
        isSFXMuted   = PlayerPrefs.GetInt(PREF_SFX_MUTE,   0) == 1;

        SetMusicVolume(mv);
        SetSFXVolume(sv);

        ApplyMusicMute(isMusicMuted);
        ApplySFXMute(isSFXMuted);
    }

    /// <summary>
    /// Принимает значение 0..1 и переводит его в децибелы (-80 … 0).
    /// </summary>
    private float LinearToDecibel(float linear)
    {
        if (linear <= 0.0001f) return -80f;
        return Mathf.Log10(linear) * 20f;
    }

    #region Public Methods for Settings

    public void SetMusicVolume(float volume)
    {
        float dB = LinearToDecibel(volume);
        mainMixer.SetFloat(MIXER_MUSIC_PARAM, dB);
        PlayerPrefs.SetFloat(PREF_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        float dB = LinearToDecibel(volume);
        mainMixer.SetFloat(MIXER_SFX_PARAM, dB);
        PlayerPrefs.SetFloat(PREF_SFX_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public void ToggleMusicMute()
    {
        isMusicMuted = !isMusicMuted;
        ApplyMusicMute(isMusicMuted);
        PlayerPrefs.SetInt(PREF_MUSIC_MUTE, isMusicMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleSFXMute()
    {
        isSFXMuted = !isSFXMuted;
        ApplySFXMute(isSFXMuted);
        PlayerPrefs.SetInt(PREF_SFX_MUTE, isSFXMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    #endregion

    #region Private Helpers

    private void ApplyMusicMute(bool mute)
    {
        mainMixer.SetFloat(MIXER_MUSIC_PARAM, mute ? -80f : LinearToDecibel(PlayerPrefs.GetFloat(PREF_MUSIC_VOLUME, 0.5f)));
    }

    private void ApplySFXMute(bool mute)
    {
        mainMixer.SetFloat(MIXER_SFX_PARAM, mute ? -80f : LinearToDecibel(PlayerPrefs.GetFloat(PREF_SFX_VOLUME, 0.5f)));
    }

    #endregion

    // Пример для проигрывания одного SFX:
    public void PlaySFX(AudioClip clip)
    {
        // Создайте временный AudioSource или используйте пул (упрощённо):
        if (!isSFXMuted && clip != null)
        {
            GameObject go = new GameObject("TempSFX");
            var src = go.AddComponent<AudioSource>();
            src.outputAudioMixerGroup = mainMixer.FindMatchingGroups("SFX")[0];
            src.clip = clip;
            src.Play();
            Destroy(go, clip.length + 0.1f);
        }
    }
}
