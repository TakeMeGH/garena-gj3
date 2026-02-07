using UnityEngine;

namespace GGJ.Code.SlotMachine
{
    public enum SymbolType
    {
        Whip,
        MagicWand,
        Garlic
    }

    public class SymbolController : MonoBehaviour
    {
        [SerializeField]
        public GameObject Outline;

        public SymbolType SymbolType;

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