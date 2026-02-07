using UnityEngine;

namespace GGJ.Code.Ability
{
    [CreateAssetMenu(fileName = "NewSharedAbilityData", menuName = "GGJ/Ability/SharedAbilityData")]
    public class SharedAbilityData : ScriptableObject
    {
        [SerializeField]
        Sprite icon;

        [SerializeField]
        string abilityName;

        [SerializeField]
        int abilityLevel;

        [SerializeField, TextArea]
        string description;

        [Header("Dynamic Values (Optional)")]
        [SerializeField]
        DescriptionValueData[] dynamicValues;

        public Sprite Icon => icon;
        public string AbilityName => abilityName;
        public int AbilityLevel => abilityLevel;
        public string Description => description;
        public DescriptionValueData[] DynamicValues => dynamicValues;
    }

    [System.Serializable]
    public class DescriptionValueData
    {
        public float OldValue;
        public float NewValue;
    }
}