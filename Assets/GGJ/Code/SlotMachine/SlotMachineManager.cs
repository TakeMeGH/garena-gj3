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
        bool _isTransitioning;

        void Start()
        {
            if (machines == null || machines.Length == 0) return;

            foreach (var machine in machines)
            {
                if (machine == null) continue;
                machine.OnProcessingStarted += HandleMachineStarted;
            }

            StartCurrentMachine();
        }

        void OnDestroy()
        {
            if (machines == null) return;
            foreach (var machine in machines)
            {
                if (machine == null) continue;
                machine.OnProcessingStarted -= HandleMachineStarted;
            }
        }

        void Update()
        {
            if (machines == null || machines.Length == 0) return;
            if (_isTransitioning) return;

            if (UnityEngine.Input.GetKeyDown(stopKey))
            {
                var currentMachine = machines[_currentMachineIndex];
                if (currentMachine != null && currentMachine.IsSpinning && currentMachine.IsStopping)
                {
                    currentMachine.HandleStopInput();
                }
            }
        }

        void StartCurrentMachine()
        {
            if (machines == null || _currentMachineIndex >= machines.Length) return;

            var currentMachine = machines[_currentMachineIndex];
            if (currentMachine != null)
            {
                Debug.Log($"Starting machine {currentMachine.gameObject.name} (Index: {_currentMachineIndex})");
                currentMachine.StartSpin();
            }
        }

        void HandleMachineStarted()
        {
            _isTransitioning = true;
            Debug.Log($"Machine {machines[_currentMachineIndex].gameObject.name} finished processing.");
            
            _currentMachineIndex = (_currentMachineIndex + 1) % machines.Length;
            
            _isTransitioning = false;
            StartCurrentMachine();
        }
    }
}
