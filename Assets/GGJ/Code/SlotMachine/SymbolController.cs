using GGJ.Code.Ability;
using UnityEngine;

namespace GGJ.Code.SlotMachine
{
    public class SymbolController : MonoBehaviour
    {
        public GameObject Outline;

        public SharedAbilityData AbilityData;

        void Start()
        {
            EnableOutline(false);
        }

        public void EnableOutline(bool setEnable)
        {
            Outline.SetActive(setEnable);
        }
    }
}