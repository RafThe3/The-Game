using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsChanger : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI valueText;
    [Min(0), Range(0, 3), SerializeField] private int moveDecimalPlaces = 1;
    [SerializeField] private string settingType = string.Empty;

    private float decimalPlaces = 1;

    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat(settingType, 1);
        decimalPlaces = Mathf.Pow(10, moveDecimalPlaces);
    }

    private void Update()
    {
        AdjustSettings();
    }

    private void AdjustSettings()
    {
        if (moveDecimalPlaces % 10 != 0)
        {
            throw new ArgumentException("Decimal place must match the decimal place value");
        }

        if (settingType == string.Empty)
        {
            throw new ArgumentException("Setting type requires a name.");
        }

        if (!slider.wholeNumbers)
        {
            slider.value = Mathf.Round(slider.value * decimalPlaces) / decimalPlaces;
        }

        PlayerPrefs.SetFloat(settingType, slider.value);
        valueText.text = $"{PlayerPrefs.GetFloat(settingType) * decimalPlaces}";
    }

    public void AddSliderValue()
    {
        slider.value++;
    }

    public void DecreaseSliderValue()
    {
        slider.value--;
    }
}
