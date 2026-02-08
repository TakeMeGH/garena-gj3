using System.Collections.Generic;
using UnityEngine;
using GGJ.Code.Utils.Singleton;

namespace GGJ.Code.Ability
{
    public class AbilityShopManager : Singleton<AbilityShopManager>
    {
        [SerializeField]
        List<SharedAbilityData> abilityPool = new();

        [SerializeField, Min(1)]
        int shopOptionCount = 3;

        [SerializeField]
        bool allowDuplicates;

        [Header("Rarity Weights")]
        [SerializeField, Min(0)]
        int commonWeight = 70;

        [SerializeField, Min(0)]
        int uncommonWeight = 25;

        [SerializeField, Min(0)]
        int rareWeight = 5;

        readonly List<SharedAbilityData> _currentOptions = new();

        public IReadOnlyList<SharedAbilityData> CurrentOptions => _currentOptions;

        public SharedAbilityData[] GetShopOptions()
        {
            if (_currentOptions.Count == 0)
            {
                GenerateShopOptions();
            }

            return _currentOptions.ToArray();
        }

        public SharedAbilityData[] GenerateShopOptions()
        {
            _currentOptions.Clear();

            List<SharedAbilityData> validPool = new();
            foreach (SharedAbilityData ability in abilityPool)
            {
                if (ability != null)
                {
                    validPool.Add(ability);
                }
            }

            if (validPool.Count == 0)
            {
                Debug.LogWarning("AbilityShopManager has no abilities assigned.");
                return _currentOptions.ToArray();
            }

            int maxOptions = allowDuplicates ? shopOptionCount : Mathf.Min(shopOptionCount, validPool.Count);
            HashSet<SharedAbilityData> used = allowDuplicates ? null : new HashSet<SharedAbilityData>();

            for (int i = 0; i < maxOptions; i++)
            {
                AbilityRarity rolledRarity = RollRarity();
                SharedAbilityData picked = PickAbility(validPool, rolledRarity, used);
                if (picked == null)
                {
                    break;
                }

                _currentOptions.Add(picked);
                used?.Add(picked);
            }

            return _currentOptions.ToArray();
        }

        AbilityRarity RollRarity()
        {
            int safeCommon = Mathf.Max(0, commonWeight);
            int safeUncommon = Mathf.Max(0, uncommonWeight);
            int safeRare = Mathf.Max(0, rareWeight);
            int total = safeCommon + safeUncommon + safeRare;

            if (total <= 0)
            {
                return (AbilityRarity)Random.Range(0, 3);
            }

            int roll = Random.Range(0, total);
            if (roll < safeCommon) return AbilityRarity.Common;
            if (roll < safeCommon + safeUncommon) return AbilityRarity.Uncommon;
            return AbilityRarity.Rare;
        }

        SharedAbilityData PickAbility(List<SharedAbilityData> pool, AbilityRarity rarity, HashSet<SharedAbilityData> used)
        {
            List<SharedAbilityData> candidates = new();
            foreach (SharedAbilityData ability in pool)
            {
                if (ability.AbilityRarity == rarity && (used == null || !used.Contains(ability)))
                {
                    candidates.Add(ability);
                }
            }

            if (candidates.Count == 0)
            {
                candidates.Clear();
                foreach (SharedAbilityData ability in pool)
                {
                    if (used == null || !used.Contains(ability))
                    {
                        candidates.Add(ability);
                    }
                }
            }

            if (candidates.Count == 0)
            {
                return null;
            }

            return candidates[Random.Range(0, candidates.Count)];
        }
    }
}
