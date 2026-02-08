using GGJ.Code.Ability;
using UnityEngine;

namespace GGJ.Code.SlotMachine
{
    public class SymbolController : MonoBehaviour
    {
        public GameObject quad;
        public GameObject Outline;

        public SharedAbilityData AbilityData;

        void Start()
        {
            EnableOutline(false);
            Renderer renderer = quad.GetComponent<Renderer>();
            renderer.material.SetTexture("_BaseMap", AbilityData.AbilityTexture);
        }

        public void EnableOutline(bool setEnable)
        {
            if (Outline == null) return;
            Outline.SetActive(setEnable);
        }
    }
}