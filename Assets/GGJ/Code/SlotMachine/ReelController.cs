using System.Collections;
using System.Collections.Generic;
using GGJ.Code.Ability;
using GGJ.Code.Audio;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GGJ.Code.SlotMachine
{
    public class ReelController : SerializedMonoBehaviour
    {
        [Header("Layout")]
        [SerializeField]
        float topY = 1.5f;

        [SerializeField]
        float bottomY = -1.5f;

        [SerializeField]
        float symbolHeight = 1.0f;

        [SerializeField]
        int visibleSymbols = 3;

        [SerializeField]
        int bufferSymbols = 2;

        [Header("Spin")]
        [SerializeField]
        float spinSpeed = 8f;

        [SerializeField]
        float stopDuration = 0.6f;

        [SerializeField]
        float nextActionDelayDuration = 0.5f;

        [SerializeField]
        List<SymbolController> AllSymbolConfigurations = new();

        // [SerializeField]
        // List<SymbolController> symbolConfiguration = new();

        readonly List<SymbolDataPerIndex> _symbols = new();

        Coroutine _stopRoutine;

        public bool IsSpinning { get; private set; }
        public int VisibleSymbols => visibleSymbols;

        [System.Serializable]
        class SymbolDataPerIndex
        {
            public Transform SymbolTransform;
            public AbilityCardType SymbolType;

            public SymbolDataPerIndex(Transform symbolTransform, AbilityCardType symbolType)
            {
                SymbolTransform = symbolTransform;
                SymbolType = symbolType;
            }
        }

        void Start()
        {
            // int totalSymbols = Mathf.Max(visibleSymbols + bufferSymbols, 1);
            //
            // for (int i = 0; i < totalSymbols; i++)
            // {
            //     SymbolController symbolObject = Instantiate(symbolConfiguration[i], 
            //         transform, false);
            //
            //     float y = topY - i * symbolHeight;
            //     symbolObject.transform.localPosition = new Vector3(0f, y, 0f);
            //
            //     _symbols.Add(new SymbolDataPerIndex(symbolObject.transform,
            //         symbolObject.AbilityData.CardType));
            // }
        }

        public void SetToken(List<TurnBaseManager.TokenItem> tokenItem)
        {
            Debug.Log("SET TOKEN " + tokenItem.GetType());
            for (int i = _symbols.Count - 1; i >= 0; i--)
            {
                Destroy(_symbols[i].SymbolTransform.gameObject);
                _symbols.RemoveAt(i);
            }
            // int totalSymbols = Mathf.Max(visibleSymbols + bufferSymbols, 1);

            for (int i = 0; i < tokenItem.Count; i++)
            {
                for (int j = 0; j < AllSymbolConfigurations.Count; j++)
                {
                    if (tokenItem[i].ability.CardType == AllSymbolConfigurations[j].AbilityData.CardType)
                    {
                        SymbolController symbolObject = Instantiate(AllSymbolConfigurations[j],
                            transform, false);

                        float y = topY - i * symbolHeight;
                        symbolObject.transform.localPosition = new Vector3(0f, y, 0f);

                        _symbols.Add(new SymbolDataPerIndex(symbolObject.transform,
                            symbolObject.AbilityData.CardType));
                        break;
                    }
                }
            }
        }

        public void StartSpin()
        {
            if (IsSpinning)
            {
                return;
            }

            if (_stopRoutine != null)
            {
                StopCoroutine(_stopRoutine);
                _stopRoutine = null;
            }

            IsSpinning = true;
        }

        public void StopSpin()
        {
            if (!IsSpinning)
            {
                return;
            }

            if (_stopRoutine != null)
            {
                StopCoroutine(_stopRoutine);
            }

            _stopRoutine = StartCoroutine(StopRoutine());
        }

        void Update()
        {
            if (!IsSpinning)
            {
                return;
            }

            MoveSymbols(spinSpeed * Time.deltaTime);
        }

        void MoveSymbols(float distance)
        {
            if(_symbols.Count == 0) return;
            foreach (SymbolDataPerIndex symbol in _symbols)
            {
                Vector3 pos = symbol.SymbolTransform.localPosition;
                pos.y -= distance;
                symbol.SymbolTransform.localPosition = pos;
            }

            float recycleThreshold = bottomY - symbolHeight;
            while (_symbols[^1].SymbolTransform.localPosition.y < recycleThreshold)
            {
                int lastIndex = _symbols.Count - 1;
                SymbolDataPerIndex lastSymbol = _symbols[lastIndex];
                GameObject symbolObject = lastSymbol.SymbolTransform.gameObject;
                AbilityCardType symbolIndex = lastSymbol.SymbolType;
                _symbols.RemoveAt(lastIndex);

                float highestY = _symbols[0].SymbolTransform.localPosition.y;
                float newY = highestY + symbolHeight;
                symbolObject.transform.SetParent(transform, false);
                symbolObject.transform.localPosition = new Vector3(0f, newY, 0f);
                _symbols.Insert(0, new SymbolDataPerIndex(symbolObject.transform, symbolIndex));
            }
        }

        IEnumerator StopRoutine()
        {
            float elapsed = 0f;
            float startSpeed = spinSpeed;

            AudioManager.Instance.PlaySfx("ButtonStop");
            while (elapsed < stopDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / stopDuration);
                float currentSpeed = Mathf.Lerp(startSpeed, 0f, t);
                MoveSymbols(currentSpeed * Time.deltaTime);
                yield return null;
            }

            yield return new WaitForSeconds(nextActionDelayDuration);

            IsSpinning = false;
            _stopRoutine = null;
            SnapToGrid();
        }

        void SnapToGrid()
        {
            if (_symbols.Count == 0)
            {
                return;
            }

            Transform topSymbol = _symbols[0].SymbolTransform;
            float highestY = topSymbol.localPosition.y;

            // float gridIndex = Mathf.Round((highestY - topY) / symbolHeight);
            float nearestGridY = topY;
            float offset = highestY - nearestGridY;

            if (Mathf.Abs(offset) < 0.0001f)
            {
                return;
            }

            foreach (SymbolDataPerIndex symbol in _symbols)
            {
                Vector3 pos = symbol.SymbolTransform.localPosition;
                pos.y -= offset;
                symbol.SymbolTransform.localPosition = pos;
            }
        }

        public struct SymbolResult
        {
            public GameObject Symbol;
            public int Index;
            public AbilityCardType SymbolType;
        }

        public SymbolResult GetSymbolAtIndex(int gridIndex)
        {
            if (_symbols == null || _symbols.Count == 0)
                return new SymbolResult { Symbol = null, Index = -1, SymbolType = 0 };

            int count = _symbols.Count;
            int wrappedGridIndex = (gridIndex % count + count) % count;

            return new SymbolResult
            {
                Symbol = _symbols[wrappedGridIndex].SymbolTransform.gameObject,
                Index = wrappedGridIndex,
                SymbolType = _symbols[wrappedGridIndex].SymbolType
            };
        }

        // public SymbolResult GetCenterSymbol()
        // {
        //     if (_symbols == null || _symbols.Count == 0)
        //         return new SymbolResult { Symbol = null, Index = -1, SymbolType = -1 };
        //
        //     float centerY = (topY + bottomY) / 2f;
        //     Transform closest = null;
        //     int closestGridIndex = -1;
        //     int closestType = -1;
        //     float minDistance = float.MaxValue;
        //
        //     bool isEven = (visibleSymbols + bufferSymbols) % 2 == 0;
        //
        //     for (int i = 0; i < _symbols.Count; i++)
        //     {
        //         SymbolDataPerIndex data = _symbols[i];
        //         Transform symbol = data.SymbolTransform;
        //         float diff = symbol.localPosition.y - centerY;
        //         float distance = Mathf.Abs(diff);
        //
        //         if (distance < minDistance - 0.001f)
        //         {
        //             minDistance = distance;
        //             closest = symbol;
        //             closestGridIndex = i;
        //             closestType = data.SymbolType;
        //         }
        //         else if (isEven && Mathf.Abs(distance - minDistance) < 0.001f)
        //         {
        //             if (diff > 0)
        //             {
        //                 closest = symbol;
        //                 closestGridIndex = i;
        //                 closestType = data.SymbolType;
        //             }
        //         }
        //     }
        //
        //     if (!closest) return new SymbolResult { Symbol = null, Index = -1, SymbolType = -1 };
        //
        //     return new SymbolResult { Symbol = closest.gameObject, Index = closestGridIndex, SymbolType = closestType };
        // }
    }
}