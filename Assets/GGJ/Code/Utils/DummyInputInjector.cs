using GGJ.Code.Ability;
using GGJ.Code.UI;
using UnityEngine;

namespace GGJ.Code.Utils
{
    public class DummyInputInjector : MonoBehaviour
    {
        float _currentProgress = 0;
        float _maxProgress = 100;

        void Update()
        {
            CheckInjectDowngrade();
            UpdateProgressBar();
        }

        void CheckInjectDowngrade()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.I))
            {
                LevelDownSelectorUI.Instance.Show(AbilityShopManager.Instance.GenerateShopOptions());
            }
        }

        void UpdateProgressBar()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.J))
            {
                _currentProgress += 10;
                ProgressBarUI.Instance.UpdateProgress(_currentProgress, _maxProgress);
                if (_currentProgress >= _maxProgress)
                {
                    LevelDownSelectorUI.Instance.Show(AbilityShopManager.Instance.GenerateShopOptions());
                    _currentProgress -= _maxProgress;
                }
            }
        }
    }
}
