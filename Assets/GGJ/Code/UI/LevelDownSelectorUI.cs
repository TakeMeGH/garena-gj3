using UnityEngine;
using GGJ.Code.Ability;
using GGJ.Code.Utils.Singleton;

namespace GGJ.Code.UI
{
    public class LevelDownSelectorUI : Singleton<LevelDownSelectorUI>
    {
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
            SetCanvasGroupState(false);
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
            Debug.Log($"Ability Chosen: {sharedAbility.AbilityName}");
            // Handle the level down logic here or fire an event
            Hide();
        }
    }
}