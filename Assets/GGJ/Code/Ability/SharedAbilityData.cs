using UnityEngine;

namespace GGJ.Code.Ability
{
    public enum AbilityCardType
    {
        None,
        Common1,
        Common2,
        Common3,
        Uncommon1 = 100,
        Uncommon2,
        Rare1 = 200
    }

    public enum AbilityRarity
    {
        Common,
        Uncommon,
        Rare
    }

    [CreateAssetMenu(fileName = "NewSharedAbilityData", menuName = "GGJ/Ability/SharedAbilityData")]
    public class SharedAbilityData : ScriptableObject
    {
        [SerializeField]
        AbilityCardType cardType;

        [SerializeField]
        Sprite icon;
        
        public Texture AbilityTexture;

        [SerializeField]
        string abilityName;

        [SerializeField]
        AbilityRarity abilityRarity;

        [SerializeField, TextArea]
        string description;

        [SerializeField]
        int damage;

        [SerializeField]
        int extraMultiplier;

        public Sprite Icon => icon;
        public string AbilityName => abilityName;
        public AbilityRarity AbilityRarity => abilityRarity;
        public string Description => description;
        public int Damage => damage;
        public int ExtraMultiplier => extraMultiplier;
        public AbilityCardType CardType => cardType;
    }
}