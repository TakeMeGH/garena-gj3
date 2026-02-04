using UnityEngine;

namespace GGJ.Code.Utils.Singleton
{
    public class ColdSingletonLoader : MonoBehaviour
    {
        [SerializeField]
        GameObject managers;

        void Awake()
        {
            if (!FindAnyObjectByType<SingletonManager>())
            {
                Instantiate(managers);
            }
        }
    }
}