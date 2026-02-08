using GGJ.Code.Audio;
using UnityEngine;

namespace GGJ.Code.SlotMachine
{
    public class SlotMachineManager : MonoBehaviour
    {
        [SerializeField]
        SlotMachineController[] machines;

        [SerializeField]
        KeyCode stopKey = KeyCode.Return;

        [SerializeField]
        TurnBaseManager turnBaseManager;

        int _currentMachineIndex;
        bool _isLastMachine;
        int _totalTurnDamage;

        void Awake()
        {
            if (machines == null || machines.Length == 0) return;

            foreach (SlotMachineController machine in machines)
            {
                if (machine == null) continue;
                machine.OnProcessingStarted += HandleMachineStarted;
                machine.OnProcessingCompleted += HandleMachineCompleted;
            }
        }

        void Start()
        {
            if (turnBaseManager == null)
            {
                turnBaseManager = FindObjectOfType<TurnBaseManager>();
            }
        }

        void OnDestroy()
        {
            if (machines == null) return;
            foreach (SlotMachineController machine in machines)
            {
                if (machine == null) continue;
                machine.OnProcessingStarted -= HandleMachineStarted;
                machine.OnProcessingCompleted -= HandleMachineCompleted;
            }
        }

        void Update()
        {
            if (machines == null || machines.Length == 0) return;
            if (_isLastMachine) return;

            if (!UnityEngine.Input.GetKeyDown(stopKey)) return;
            SlotMachineController currentMachine = machines[_currentMachineIndex];
            if (currentMachine && currentMachine.IsSpinning && currentMachine.IsStopping)
            {
                currentMachine.HandleStopInput();
            }
        }

        void HandleMachineStarted(SlotMachineController machine)
        {
            if (machines == null || machines.Length == 0) return;

            _isLastMachine = false;
            if (_currentMachineIndex == machines.Length - 1)
            {
                AudioManager.Instance.StopLoopedSfx("SlotMachineRolling");
                _isLastMachine = _currentMachineIndex == machines.Length - 1;
            }

            Debug.Log($"Machine {machine.gameObject.name} finished processing.");

            _currentMachineIndex = (_currentMachineIndex + 1) % machines.Length;
        }

        void HandleMachineCompleted(SlotMachineController machine)
        {
            if (machine)
            {
                _totalTurnDamage += machine.LastCalculatedDamage;
            }

            if (machine == machines[^1])
            {
                ApplyTurnDamage();
            }
        }

        public void BeginPlayerTurn()
        {
            if (machines == null || machines.Length == 0) return;
            _totalTurnDamage = 0;
            _currentMachineIndex = 0;
            _isLastMachine = false;
            RestartAllMachine();
        }

        void RestartAllMachine()
        {
            Invoke(nameof(DelayStartSpinSfx), 0.25f);
            foreach (SlotMachineController machine in machines)
            {
                machine.StartSpin(turnBaseManager.tokensInDeck);
            }
        }

        void DelayStartSpinSfx()
        {
            AudioManager.Instance.PlaySfx("StartSlot");
            AudioManager.Instance.PlayLoopedSfx("SlotMachineRolling");

        }

        void ApplyTurnDamage()
        {
            if (turnBaseManager)
            {
                if (_totalTurnDamage > 0)
                {
                    turnBaseManager.PlayerAttackAnimation(_totalTurnDamage);
                } else
                {
                    Debug.LogWarning("This shouldn't happen!!");
                }
                // turnBaseManager.PlayerTurnDone();
                
            } else
            {
                Debug.LogWarning("This shouldn't happen!!");
            }
        }
    }
}
