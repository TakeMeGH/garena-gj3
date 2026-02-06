using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

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

        [SerializeField]
        bool autoStart = true;

        [SerializeField]
        bool useInternalInput = true;

        bool _isSpinning;
        int _currentReelToStop;
        bool _isStopping;

        public bool IsSpinning => _isSpinning;
        public bool IsStopping => _isStopping;

        public event System.Action OnProcessingStarted;

        void Start()
        {
            if (autoStart)
            {
                StartSpin();
            }
        }

        void Update()
        {
            if (!useInternalInput) return;

            if (UnityEngine.Input.GetKeyDown(stopKey))
            {
                HandleStopInput();
            }
        }

        public void StartSpin()
        {
            if (_isSpinning || reels == null || reels.Length == 0)
            {
                return;
            }

            StartCoroutine(SpinRoutine());
        }

        public void HandleStopInput()
        {
            if (_isSpinning && _isStopping)
            {
                StopNextReel();
            }
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

            OnProcessingStarted?.Invoke();

            Debug.Log("All reels stopped. Processing results...");

            int[] offsets = new int[reels.Length];
            ReelSpinner.SymbolResult[] finalResults = new ReelSpinner.SymbolResult[reels.Length];

            for (int i = 0; i < reels.Length; i++)
            {
                if (!reels[i]) continue;

                ReelSpinner.SymbolResult centerResult = reels[i].GetCenterSymbol();
                offsets[i] = Random.Range(-1, 2);
                finalResults[i] = reels[i].GetSymbolAtIndex(centerResult.Index + offsets[i]);
            }

            for (int i = 0; i < reels.Length; i++)
            {
                ReelSpinner reel = reels[i];
                if (!reel) continue;

                ReelSpinner.SymbolResult result = finalResults[i];
                string symbolInfo = result.Symbol ? result.Symbol.name : "None";

                Debug.Log(
                    $"Processing reel: {reel.gameObject.name}, Offset: {offsets[i]}, Target Index: {result.Index}, Symbol: {symbolInfo}, Type: {result.SymbolType}");

                if (i + 2 < reels.Length && finalResults[i].SymbolType != -1 &&
                    finalResults[i].SymbolType == finalResults[i + 1].SymbolType &&
                    finalResults[i].SymbolType == finalResults[i + 2].SymbolType)
                {
                    Debug.Log("CRIT! Same symbol type detected on consecutive reels!");
                }

                yield return new WaitForSeconds(0.2f);
            }

            _isSpinning = false;
            _isStopping = false;

            if (autoStart)
            {
                StartSpin();
            }
        }
    }
}