using System;
using UnityEngine;
using GGJ.Code.Ability;
using GGJ.Code.Utils.Singleton;

namespace GGJ.Code.UI
{
    public class LevelDownSelectorUI : Singleton<LevelDownSelectorUI>
    {
        public bool IsOpen { get; private set; }
        public event Action Closed;

        [SerializeField]
        CanvasGroup canvasGroup;

        [SerializeField]
        AbilityCardUI[] cards;

        protected override void OwnAwake()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            Hide();
        }
        
        public void Show(SharedAbilityData[] abilities)
        {
            IsOpen = true;
            if (abilities.Length != cards.Length)
            {
                Debug.LogWarning("Abilities count does not match cards count!");
            }

            for (int i = 0; i < cards.Length; i++)
            {
                if (i < abilities.Length)
                {
                        cards[i].gameObject.SetActive(true);
                    cards[i].Setup(abilities[i], OnAbilityChosen);
                }
                else
                {
                    cards[i].gameObject.SetActive(false);
                }
            }

            SetCanvasGroupState(true);
        }

        void Hide()
        {
            if (!IsOpen) return;
            IsOpen = false;
            SetCanvasGroupState(false);
            Closed?.Invoke();
        }

        void SetCanvasGroupState(bool state)
        {
            if (canvasGroup == null) return;
            canvasGroup.alpha = state ? 1f : 0f;
            canvasGroup.interactable = state;
            canvasGroup.blocksRaycasts = state;
        }

        void OnAbilityChosen(SharedAbilityData sharedAbility)
        {
            TurnBaseManager turnBaseManager = TurnBaseManager.Instance;
            if (turnBaseManager == null)
            {
                Debug.LogWarning("TurnBaseManager not found for purchase.");
                return;
            }

            // if (!turnBaseManager.SpendCoins(sharedAbility.Cost))
            // {
            //     Debug.Log($"Not enough coins to buy {sharedAbility.AbilityName}.");
            //     return;
            // }

            Debug.Log($"Ability Purchased: {sharedAbility.AbilityName}");
            // Handle the level down logic here or fire an event
            Hide();
        }
    }
}
