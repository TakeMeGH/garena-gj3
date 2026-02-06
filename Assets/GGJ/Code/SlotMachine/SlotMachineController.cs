using System.Collections;
using UnityEngine;

namespace GGJ.Code.SlotMachine
{
    public class SlotMachineController : MonoBehaviour
    {
        [SerializeField]
        ReelSpinner[] reels;

        [SerializeField]
        float startDelay = 0.1f;

        [SerializeField]
        float stopDelay = 0.25f;

        [SerializeField]
        KeyCode spinKey = KeyCode.Space;

        [SerializeField]
        KeyCode stopKey = KeyCode.Return;

        bool _isSpinning;
        int _currentReelToStop;
        bool _isStopping;

        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(spinKey))
            {
                if (!_isSpinning)
                {
                    StartSpin();
                }
            }

            if (UnityEngine.Input.GetKeyDown(stopKey))
            {
                if (_isSpinning && _isStopping)
                {
                    StopNextReel();
                }
            }
        }

        void StartSpin()
        {
            if (_isSpinning || reels == null || reels.Length == 0)
            {
                return;
            }

            StartCoroutine(SpinRoutine());
        }

        void StopNextReel()
        {
            if (_currentReelToStop >= reels.Length) return;

            if (reels[_currentReelToStop])
            {
                reels[_currentReelToStop].StopSpin();
            }

            _currentReelToStop++;

            if (_currentReelToStop >= reels.Length)
            {
                StartCoroutine(ProcessResultsRoutine());
            }
        }

        IEnumerator SpinRoutine()
        {
            _isSpinning = true;
            _isStopping = false;
            _currentReelToStop = 0;

            foreach (ReelSpinner t in reels)
            {
                if (t)
                {
                    t.StartSpin();
                }

                if (startDelay > 0f)
                {
                    yield return new WaitForSeconds(startDelay);
                }
            }

            _isStopping = true;
        }

        IEnumerator ProcessResultsRoutine()
        {
            bool anySpinning = true;
            while (anySpinning)
            {
                anySpinning = false;
                foreach (ReelSpinner reel in reels)
                {
                    if (!reel || !reel.IsSpinning) continue;
                    anySpinning = true;
                    break;
                }

                yield return null;
            }

            Debug.Log("All reels stopped. Processing results...");

            foreach (ReelSpinner reel in reels)
            {
                if (!reel) continue;

                ReelSpinner.SymbolResult centerResult = reel.GetCenterSymbol();
                int offset = Random.Range(-1, 2);
                ReelSpinner.SymbolResult offsetResult = reel.GetSymbolAtIndex(centerResult.Index + offset);

                string symbolInfo = offsetResult.Symbol ? offsetResult.Symbol.name : "None";
                Debug.Log(
                    $"Processing reel: {reel.gameObject.name}, Center Index: {centerResult.Index}, Offset: {offset}, Target Index: {offsetResult.Index}, Symbol: {symbolInfo}");
                yield return new WaitForSeconds(0.2f);
            }

            _isSpinning = false;
            _isStopping = false;
            StartSpin();
        }
    }
}