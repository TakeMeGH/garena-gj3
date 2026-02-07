using UnityEngine;

namespace GGJ.Code.SlotMachine
{
    public class SymbolController : MonoBehaviour
    {
        [SerializeField]
        public GameObject Outline;

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