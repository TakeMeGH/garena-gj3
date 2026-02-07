using System;
using System.Collections;
using System.Collections.Generic;
using GGJ.Code.Audio;
using GGJ.Code.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGJ.Code.SlotMachine
{
    public class SlotMachineController : MonoBehaviour
    {
        [SerializeField]
        ReelController[] reels;

        [SerializeField]
        float startDelay = 0.1f;

        [SerializeField]
        float stopDelay = 0.25f;

        Player _player;
        bool _isSpinning;
        int _currentReelToStop;
        bool _isStopping;

        public bool IsSpinning => _isSpinning;
        public bool IsStopping => _isStopping;

        public event System.Action<SlotMachineController> OnProcessingStarted;
        public event System.Action<SlotMachineController> OnProcessingCompleted;

        List<SymbolController> _outlinedSymbol = new();

        public void StartSpin()
        {
            if (_isSpinning || reels == null || reels.Length == 0)
            {
                return;
            }

            foreach (SymbolController symbolController in _outlinedSymbol)
            {
                symbolController.EnableOutline(false);
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

            foreach (ReelController t in reels)
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
                foreach (ReelController reel in reels)
                {
                    if (!reel || !reel.IsSpinning) continue;
                    anySpinning = true;
                    break;
                }

                yield return null;
            }

            OnProcessingStarted?.Invoke(this);

            Debug.Log("All reels stopped. Processing results...");

            int[] offsets = new int[reels.Length];
            ReelController.SymbolResult[] finalResults = new ReelController.SymbolResult[reels.Length];

            for (int i = 0; i < reels.Length; i++)
            {
                if (!reels[i]) continue;

                ReelController.SymbolResult centerResult = reels[i].GetCenterSymbol();
                offsets[i] = Random.Range(-1, 2);
                finalResults[i] = reels[i].GetSymbolAtIndex(centerResult.Index + offsets[i]);
            }

            for (int i = 0; i < reels.Length; i++)
            {
                ReelController reel = reels[i];
                if (!reel) continue;

                ReelController.SymbolResult result = finalResults[i];
                if (result.Symbol && result.Symbol.TryGetComponent(out SymbolController symbolController))
                {
                    symbolController.EnableOutline(true);
                    _outlinedSymbol.Add(symbolController);
                    switch (symbolController.SymbolType)
                    {
                        case SymbolType.Whip:
                            GetPlayer().Whip();
                            break;
                        case SymbolType.MagicWand:
                            GetPlayer().MagicWand();
                            break;
                        case SymbolType.Garlic:
                            GetPlayer().Garlic();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                // AudioManager.Instance.PlaySfx("ButtonStop");
                AudioManager.Instance.PlaySfx("CoinDrop");

                // Debug.Log(
                //     $"Processing reel: {reel.gameObject.name}, Offset: {offsets[i]}, Target Index: {result.Index}, Symbol: {symbolInfo}, Type: {result.SymbolType}");

                if (i + 2 < reels.Length && finalResults[i].SymbolType != -1 &&
                    finalResults[i].SymbolType == finalResults[i + 1].SymbolType &&
                    finalResults[i].SymbolType == finalResults[i + 2].SymbolType)
                {
                    Debug.Log("CRIT! Same symbol type detected on consecutive reels!");
                    TextPopupManager.Instance.CreateCriticalPopup(finalResults[i].Symbol.transform.position +
                                                                  new Vector3(0, -0.5f, -0.5f));
                    AudioManager.Instance.PlaySfx("CriticalHit");
                }

                yield return new WaitForSeconds(0.2f);
            }

            _isSpinning = false;
            _isStopping = false;

            OnProcessingCompleted?.Invoke(this);
        }

        Player GetPlayer()
        {
            if (_player != null) return _player;
            _player = FindObjectOfType<Player>();
            return _player;
        }
    }
}