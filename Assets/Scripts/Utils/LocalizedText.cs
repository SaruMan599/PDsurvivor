using UnityEngine;
using TMPro;

public class LocalizedText : MonoBehaviour
{
    public string key; // тот же, что в словаре LocalizationManager

    private TextMeshProUGUI tmp;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        if (tmp != null && LocalizationManager.Instance != null)
        {
            tmp.text = LocalizationManager.Instance.GetLocalizedValue(key);
        }
    }
}
