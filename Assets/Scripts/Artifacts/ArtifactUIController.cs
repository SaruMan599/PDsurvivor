using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArtifactUIController : MonoBehaviour
{
    [Header("UI-элементы")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Image backRarity;       // Белая Image, которую будем перекрашивать
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button acceptButton;

    private ArtifactData currentData;

    private void Awake()
    {
        panelRoot.SetActive(false);
        acceptButton.onClick.AddListener(OnAcceptClicked);
    }

    /// <summary>
    /// Показывает панель с артефактом (ScriptableObject).
    /// </summary>
    public void ShowArtifact(ArtifactData data)
    {
        currentData = data;

        backRarity.color      = GetColorByRarity(data.rarity);
        nameText.text         = data.artifactName;
        descriptionText.text  = data.description;
        iconImage.sprite      = data.icon;

        panelRoot.SetActive(true);
        panelRoot.SetActive(true);
        Time.timeScale = 0f;
    }

    private void OnAcceptClicked()
    {
        if (currentData == null || PlayerController.Instance == null) return;

        PlayerArtifactSystem.Instance.AddArtifact(currentData);
        panelRoot.SetActive(false);
        Time.timeScale = 1f;
    }

    private Color GetColorByRarity(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return Color.white;
            case Rarity.Rare:   return Color.blue;
            case Rarity.Epic:   return new Color(0.6f, 0f, 0.6f);
            default:            return Color.gray;
        }
    }
}
