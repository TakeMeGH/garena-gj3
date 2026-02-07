using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using GGJ.Code.Ability;

namespace GGJ.Code.UI
{
    public class AbilityCardUI : MonoBehaviour
    {
        [SerializeField]
        Image iconImage;

        [SerializeField]
        TextMeshProUGUI nameText;

        [SerializeField]
        TextMeshProUGUI levelText;

        [SerializeField]
        TextMeshProUGUI descriptionText;

        [SerializeField]
        Button chooseButton;

        SharedAbilityData _data;
        Action<SharedAbilityData> _onChooseCallback;

        public void Setup(SharedAbilityData data, Action<SharedAbilityData> onChoose)
        {
            _data = data;
            _onChooseCallback = onChoose;

            if (iconImage) iconImage.sprite = data.Icon;
            if (nameText) nameText.text = data.AbilityName;
            if (levelText) levelText.text = $"Level: {data.AbilityLevel} -> Level: {data.AbilityLevel - 1}";
            if (descriptionText)
            {
                string desc = data.Description;
                if (data.DynamicValues != null)
                {
                    for (int i = 0; i < data.DynamicValues.Length; i++)
                    {
                        desc = desc.Replace("{x" + i + "}", data.DynamicValues[i].OldValue.ToString("F1"));
                        desc = desc.Replace("{y" + i + "}", data.DynamicValues[i].NewValue.ToString("F1"));

                    }
                }
                descriptionText.text = desc;
            }

            chooseButton.onClick.RemoveAllListeners();
            chooseButton.onClick.AddListener(HandleChoose);
        }

        void HandleChoose()
        {
            _onChooseCallback?.Invoke(_data);
        }
    }
}