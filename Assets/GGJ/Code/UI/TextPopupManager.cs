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
    }
}