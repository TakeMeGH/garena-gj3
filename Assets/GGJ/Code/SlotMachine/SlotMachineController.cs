using System;
using System.Collections;
using System.Collections.Generic;
using GGJ.Code.Audio;
using GGJ.Code.UI;
using UnityEngine;
using UnityEngine.InputSystem;
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

        [SerializeField]
        SlotMiniGameUI miniGameUI;

        Player _player;
        bool _isSpinning;
        int _currentReelToStop;
        bool _isStopping;

        public bool IsSpinning => _isSpinning;
        public bool IsStopping => _isStopping;

        public event Action<SlotMachineController> OnProcessingStarted;
        public event Action<SlotMachineController> OnProcessingCompleted;

        readonly List<SymbolController> _outlinedSymbol = new();

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

            bool hasMatch = TryGetMatchRun(finalResults, out SymbolType matchSymbolType, out int matchCount,
                out int matchStartIndex);

            for (int i = 0; i < reels.Length; i++)
            {
                ReelController reel = reels[i];
                if (!reel) continue;

                ReelController.SymbolResult result = finalResults[i];
                if (result.Symbol && result.Symbol.TryGetComponent(out SymbolController symbolController))
                {
                    symbolController.EnableOutline(true);
                    _outlinedSymbol.Add(symbolController);
                }

                AudioManager.Instance.PlaySfx("CoinDrop");

                yield return new WaitForSeconds(0.2f);
            }

            if (hasMatch)
            {
                Debug.Log($"Match detected: {matchCount} in a row. Starting minigame.");
                if (matchStartIndex >= 0 && matchStartIndex < finalResults.Length &&
                    finalResults[matchStartIndex].Symbol)
                {
                    TextPopupManager.Instance.CreateCriticalPopup(
                        finalResults[matchStartIndex].Symbol.transform.position +
                        new Vector3(0, -0.5f, -0.5f));
                }

                AudioManager.Instance.PlaySfx("CriticalHit");
                yield return RunMiniGame(matchSymbolType, matchCount);
            }

            _isSpinning = false;
            _isStopping = false;

            OnProcessingCompleted?.Invoke(this);
        }

        Player GetPlayer()
        {
            if (_player) return _player;
            _player = FindFirstObjectByType<Player>();
            return _player;
        }

        static bool TryGetMatchRun(ReelController.SymbolResult[] results, out SymbolType matchSymbolType,
            out int matchCount,
            out int matchStartIndex)
        {
            const int minMatch = 3;
            const int maxMatch = 5;
            matchSymbolType = default;
            matchCount = 0;
            matchStartIndex = -1;

            if (results == null || results.Length < minMatch)
            {
                return false;
            }

            int currentCount = 0;
            int bestCount = 0;
            int lastType = -1;
            int currentStartIndex = 0;
            int bestStartIndex = -1;

            for (int i = 0; i < results.Length; i++)
            {
                int symbolType = results[i].SymbolType;
                if (symbolType < 0)
                {
                    currentCount = 0;
                    lastType = -1;
                    continue;
                }

                if (symbolType == lastType)
                {
                    currentCount++;
                }
                else
                {
                    currentCount = 1;
                    lastType = symbolType;
                    currentStartIndex = i;
                }

                if (currentCount >= minMatch && currentCount > bestCount)
                {
                    bestCount = currentCount;
                    matchSymbolType = (SymbolType)symbolType;
                    bestStartIndex = currentStartIndex;
                }
            }

            if (bestCount < minMatch) return false;

            matchCount = Mathf.Clamp(bestCount, minMatch, maxMatch);
            matchStartIndex = bestStartIndex;
            return true;
        }

        IEnumerator RunMiniGame(SymbolType symbolType, int matchCount)
        {
            SlotMiniGame miniGame = CreateMiniGame(matchCount);
            miniGame.Start();
            if (miniGameUI) miniGameUI.Show(miniGame);
            Debug.Log($"Minigame {miniGame.Name} started. Press Enter.");

            while (!IsEnterPressed())
            {
                miniGame.Tick(Time.deltaTime);
                if (miniGameUI) miniGameUI.UpdateUI(miniGame);
                yield return null;
            }

            float score = miniGame.Complete();
            Debug.Log($"Minigame {miniGame.Name} score: {score:F0}%");
            if (miniGameUI) miniGameUI.Hide();

            GetPlayer().ApplyPowerUp(symbolType, score, matchCount);
        }

        static SlotMiniGame CreateMiniGame(int matchCount)
        {
            int roll = Random.Range(0, 2);
            return roll switch
            {
                0 => new BarPeakMiniGame(),
                _ => new BowAndArrowMiniGame()
            };
        }

        static bool IsEnterPressed()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return false;
            return keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame;
        }
    }
}
