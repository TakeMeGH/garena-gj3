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

        int _currentMachineIndex;
        bool _isLastMachine;

        void Start()
        {
            if (machines == null || machines.Length == 0) return;

            foreach (SlotMachineController machine in machines)
            {
                if (machine == null) continue;
                machine.OnProcessingStarted += HandleMachineStarted;
                machine.OnProcessingCompleted += HandleMachineCompleted;
            }

            RestartAllMachine();
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
            if (machine == machines[^1])
            {
                RestartAllMachine();
                _isLastMachine = false;
            }
        }

        void RestartAllMachine()
        {
            Invoke(nameof(DelayStartSpinSFX), 0.25f);
            foreach (SlotMachineController machine in machines)
            {
                machine.StartSpin();
            }
        }

        void DelayStartSpinSFX()
        {
            AudioManager.Instance.PlaySfx("StartSlot");
            AudioManager.Instance.PlayLoopedSfx("SlotMachineRolling");

        }
    }
}