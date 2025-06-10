// SettingsController.cs (обновлено для подсветки кнопок RU/EN)
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [Header("Sound Controls")]
    public Slider musicSlider;
    public Button musicMuteButton;
    public TMP_Text musicMuteButtonText;

    public Slider sfxSlider;
    public Button sfxMuteButton;
    public TMP_Text sfxMuteButtonText;

    [Header("Language Controls")]
    public Button buttonRU;
    public Button buttonEN;
    public TMP_Text ruButtonText;   // TMP_Text внутри кнопки RU
    public TMP_Text enButtonText;   // TMP_Text внутри кнопки EN

    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;

    private void Start()
    {
        float mv = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float sv = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        bool mm = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        bool sm = PlayerPrefs.GetInt("SFXMuted", 0) == 1;

        musicSlider.value = mv;
        sfxSlider.value = sv;

        UpdateMusicMuteText(mm);
        UpdateSFXMuteText(sm);

        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);

        musicMuteButton.onClick.AddListener(OnMusicMuteClicked);
        sfxMuteButton.onClick.AddListener(OnSFXMuteClicked);

        buttonRU.onClick.AddListener(() => OnLanguageButton("RU"));
        buttonEN.onClick.AddListener(() => OnLanguageButton("EN"));

        settingsPanel.SetActive(false);

        HighlightLanguageButton(LocalizationManager.Instance.CurrentLanguage);
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        pausePanel.SetActive(false);
        Debug.Log("OpenSettings() вызван");
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    private void OnMusicSliderChanged(float val)
    {
        AudioManager.Instance.SetMusicVolume(val);
        AudioManager.Instance.mainMixer.GetFloat("MusicVolume", out float dB);
        if (dB <= -79f && val > 0f)
        {
            AudioManager.Instance.ToggleMusicMute();
            UpdateMusicMuteText(false);
        }
    }

    private void OnSFXSliderChanged(float val)
    {
        AudioManager.Instance.SetSFXVolume(val);
        AudioManager.Instance.mainMixer.GetFloat("SFXVolume", out float dB2);
        if (dB2 <= -79f && val > 0f)
        {
            AudioManager.Instance.ToggleSFXMute();
            UpdateSFXMuteText(false);
        }
    }

    private void OnMusicMuteClicked()
    {
        AudioManager.Instance.ToggleMusicMute();
        AudioManager.Instance.mainMixer.GetFloat("MusicVolume", out float dB);
        bool isMuted = (dB <= -79f);
        UpdateMusicMuteText(isMuted);
    }

    private void OnSFXMuteClicked()
    {
        AudioManager.Instance.ToggleSFXMute();
        AudioManager.Instance.mainMixer.GetFloat("SFXVolume", out float dB2);
        bool isMuted = (dB2 <= -79f);
        UpdateSFXMuteText(isMuted);
    }

    private void UpdateMusicMuteText(bool isMuted)
    {
        string key = isMuted ? "Unmute" : "Mute";
        musicMuteButtonText.text = LocalizationManager.Instance.GetLocalizedValue(key);
    }

    private void UpdateSFXMuteText(bool isMuted)
    {
        string key = isMuted ? "Unmute" : "Mute";
        sfxMuteButtonText.text = LocalizationManager.Instance.GetLocalizedValue(key);
    }

    private void OnLanguageButton(string langCode)
    {
        LocalizationManager.Instance.SetLanguage(langCode);
        HighlightLanguageButton(LocalizationManager.Instance.CurrentLanguage);
    }

    private void HighlightLanguageButton(LocalizationManager.Language lang)
    {
        if (lang == LocalizationManager.Language.RU)
        {
            ruButtonText.color = Color.white;
            enButtonText.color = Color.red;
        }
        else
        {
            ruButtonText.color = Color.red;
            enButtonText.color = Color.white;
        }
    }
}
