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
    [Tooltip("Which decimal place to convert into an integer (10 - tenths, 100 - hundredths, etc.")]
    [Min(0), SerializeField] private int moveToDecimalPlace = 100;
    [SerializeField] private string settingType = string.Empty;

    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat(settingType, 1);
    }

    private void Update()
    {
        AdjustSettings();
    }

    private void AdjustSettings()
    {
        if (moveToDecimalPlace % 10 != 0)
        {
            throw new ArgumentException("Decimal place must match the decimal place value");
        }

        if (settingType == string.Empty)
        {
            throw new ArgumentException("Setting type requires a name.");
        }

        if (!slider.wholeNumbers)
        {
            slider.value = Mathf.Round(slider.value * moveToDecimalPlace) / moveToDecimalPlace;
        }

        PlayerPrefs.SetFloat(settingType, slider.value);
        valueText.text = $"{PlayerPrefs.GetFloat(settingType) * moveToDecimalPlace}";
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
