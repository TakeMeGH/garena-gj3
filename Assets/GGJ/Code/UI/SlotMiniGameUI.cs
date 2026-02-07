using GGJ.Code.SlotMachine;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ.Code.UI
{
    public class SlotMiniGameUI : MonoBehaviour
    {
        [Header("Bar Peak")]
        [SerializeField]
        GameObject barGroup;

        [SerializeField]
        Slider barSlider;

        [Header("Bow And Arrow")]
        [SerializeField]
        GameObject bowGroup;

        [SerializeField]
        RectTransform bowArea;

        [SerializeField]
        RectTransform bowMarker;

        public void Show(SlotMiniGame miniGame)
        {
            if (barGroup) barGroup.SetActive(miniGame.UsesBar);
            if (bowGroup) bowGroup.SetActive(miniGame.UsesTarget);
        }

        public void Hide()
        {
            if (barGroup) barGroup.SetActive(false);
            if (bowGroup) bowGroup.SetActive(false);
        }

        public void UpdateUI(SlotMiniGame miniGame)
        {
            if (miniGame == null) return;

            if (miniGame.UsesBar)
            {
                UpdateBar(miniGame.BarValue01);
            }

            if (miniGame.UsesTarget)
            {
                UpdateBow(miniGame.TargetPosition01);
            }
        }

        void UpdateBar(float value01)
        {
            if (!barSlider) return;
            barSlider.value = Mathf.Clamp01(value01);
        }

        void UpdateBow(Vector2 normalizedPosition)
        {
            if (!bowArea || !bowMarker) return;
            float radius = Mathf.Min(bowArea.rect.width, bowArea.rect.height) * 0.5f;
            bowMarker.anchoredPosition = normalizedPosition * radius;
        }
    }
}
