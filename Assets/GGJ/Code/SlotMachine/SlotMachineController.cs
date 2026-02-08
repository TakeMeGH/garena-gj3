using System;
using System.Collections;
using System.Collections.Generic;
using GGJ.Code.Ability;
using GGJ.Code.Audio;
using GGJ.Code.UI;
using UnityEngine;

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
        public int LastCalculatedDamage { get; private set; }

        public event Action<SlotMachineController> OnProcessingStarted;
        public event Action<SlotMachineController> OnProcessingCompleted;

        readonly List<SymbolController> _outlinedSymbol = new();

        public void StartSpin(TurnBaseManager.TokenItem[][] tokenItem)
        {
            if (_isSpinning || reels == null || reels.Length == 0)
            {
                return;
            }
            
            InitReels(tokenItem);

            foreach (SymbolController symbolController in _outlinedSymbol)
            {
                symbolController.EnableOutline(false);
            }

            StartCoroutine(SpinRoutine());
        }

        void InitReels(TurnBaseManager.TokenItem[][] tokenItem)
        {
            for (int j = 0; j < 4; j++)
            {
                List<TurnBaseManager.TokenItem> reelTokens = new();
                for (int i = 0; i < 4; i++)
                {
                    reelTokens.Add(tokenItem[i][j]);
                }

                reels[j].SetToken(reelTokens);
            }
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
            LastCalculatedDamage = 0;
            Debug.Log("All reels stopped. Processing results...");

            ReelController.SymbolResult[] finalResults = new ReelController.SymbolResult[reels.Length];
            for (int j = 0; j < reels[0].VisibleSymbols; j++)
            {
                for (int i = 0; i < reels.Length; i++)
                {
                    if (!reels[i]) continue;

                    finalResults[i] = reels[i].GetSymbolAtIndex(j);
                    ReelController.SymbolResult result = finalResults[i];
                    if (!result.Symbol || !result.Symbol.TryGetComponent(out SymbolController symbolController))
                        continue;
                    symbolController.EnableOutline(true);
                    _outlinedSymbol.Add(symbolController);
                    int damage = GetSymbolDamage(result);
                    TextPopupManager.Instance.CreateCustomCriticalPopup(
                        result.Symbol.transform.position +
                        new Vector3(0, 0.5f, 0), damage.ToString(),
                        Color.white * 3);

                    AudioManager.Instance.PlaySfx("CoinDrop");
                    yield return new WaitForSeconds(0.2f);
                }

                LastCalculatedDamage += CalculateDamageFromRow(finalResults);
                Debug.Log($"Slot row damage: {LastCalculatedDamage}");
            }

            _isSpinning = false;
            _isStopping = false;

            OnProcessingCompleted?.Invoke(this);
        }

        int CalculateDamageFromRow(ReelController.SymbolResult[] results)
        {
            if (results == null || results.Length == 0) return 0;

            int totalDamage = 0;
            int runDamage = 0;
            int runLength = 0;
            AbilityCardType previousType = AbilityCardType.None;
            bool hasPrevious = false;

            int i = -1;
            int cntCrit = 2;
            foreach (ReelController.SymbolResult result in results)
            {
                i++;
                if (result.SymbolType < 0)
                {
                    if (runLength > 0)
                    {
                        totalDamage += runDamage * runLength;
                    }

                    runDamage = 0;
                    runLength = 0;
                    hasPrevious = false;
                    continue;
                }

                int damage = GetSymbolDamage(result);
                if (!hasPrevious || result.SymbolType != previousType)
                {
                    runDamage += damage;
                    runLength++;
                }
                else
                {
                    if (runLength > 1)
                    {
                        TextPopupManager.Instance.CreateCustomCriticalPopup(
                            results[i - 1].Symbol.transform.position +
                            new Vector3(0, cntCrit * 0.5f, 0), "Critical " + (runDamage * runLength),
                            Color.red * 3);
                        cntCrit++;
                    }

                    totalDamage += runDamage * runLength;
                    runDamage = damage;
                    runLength = 1;
                }

                runLength += GetExtraMultiplier(result);

                previousType = result.SymbolType;
                hasPrevious = true;
            }

            if (runLength > 0)
            {
                if (runLength > 1)
                {
                    TextPopupManager.Instance.CreateCustomCriticalPopup(
                        results[^1].Symbol.transform.position +
                        new Vector3(0, cntCrit * 0.5f, 0), "Critical " + (runDamage * runLength),
                        Color.red * 3);
                    cntCrit++;
                }

                totalDamage += runDamage * runLength;
            }

            Debug.Log("Run damage: " + runDamage + " Run length: " + runLength + " Total damage: " + totalDamage + "");

            return totalDamage;
        }

        static int GetSymbolDamage(ReelController.SymbolResult result)
        {
            if (result.Symbol &&
                result.Symbol.TryGetComponent(out SymbolController symbolController) &&
                symbolController.AbilityData)
            {
                return symbolController.AbilityData.Damage;
            }

            return 0;
        }

        static int GetExtraMultiplier(ReelController.SymbolResult result)
        {
            if (result.Symbol &&
                result.Symbol.TryGetComponent(out SymbolController symbolController) &&
                symbolController.AbilityData)
            {
                return symbolController.AbilityData.ExtraMultiplier;
            }

            return 0;
        }
    }
}