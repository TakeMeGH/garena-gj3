using GGJ.Code.Utils.Singleton;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ.Code.UI
{
    public class ProgressBarUI : Singleton<ProgressBarUI>
    {
        [SerializeField]
        Slider progressionSlider;

        [SerializeField]
        bool useLerp = true;

        [SerializeField]
        float lerpSpeed = 5f;

        float _targetFillAmount;

        void Update()
        {
            if (useLerp && progressionSlider)
            {
                progressionSlider.value =
                    Mathf.Lerp(progressionSlider.value, _targetFillAmount, Time.deltaTime * lerpSpeed);
            }
        }

        public void UpdateProgress(float value, float maxValue)
        {
            float fillAmount = 0f;
            if (maxValue > 0)
            {
                fillAmount = Mathf.Clamp01(value / maxValue);
            }

            if (useLerp)
            {
                _targetFillAmount = fillAmount;
            }
            else
            {
                if (progressionSlider)
                {
                    progressionSlider.value = fillAmount;
                }
            }
        }
    }
}