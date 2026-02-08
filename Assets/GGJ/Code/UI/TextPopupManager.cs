using GGJ.Code.Utils.Singleton;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace GGJ.Code.UI
{
    public class TextPopupManager : Singleton<TextPopupManager>
    {
        [SerializeField]
        MMF_Player floatingTextFeedback;

        [SerializeField]
        MMF_Player criticalTextFeedback;

        public void CreateDamagePopup(Vector3 position, float damageAmount)
        {
            floatingTextFeedback.PlayFeedbacks(position, damageAmount);
        }

        public void CreateCriticalPopup(Vector3 position)
        {
            criticalTextFeedback.PlayFeedbacks(position);
        }

        public void CreateCustomCriticalPopup(Vector3 position, string customText,
            Color colorTarget = default)
        {
            MMF_FloatingText floatingText = criticalTextFeedback.GetFeedbackOfType<MMF_FloatingText>();

            Color hdrTarget = colorTarget;

            Gradient gradient = new Gradient();

            GradientColorKey[] colorKey = new GradientColorKey[2];
            colorKey[0] = new GradientColorKey(hdrTarget, 0f);
            colorKey[1] = new GradientColorKey(hdrTarget, 1f);

            GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
            alphaKey[0] = new GradientAlphaKey(1f, 0f);
            alphaKey[1] = new GradientAlphaKey(0f, 1f);

            gradient.SetKeys(colorKey, alphaKey);

            floatingText.ForceColor = true;
            floatingText.AnimateColorGradient = gradient;
            //
            floatingText.Value = customText;
            criticalTextFeedback.PlayFeedbacks(position);
        }
    }
}