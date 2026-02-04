using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GGJ.Code.Utils.Singleton
{
    public sealed class SingletonManager : SerializedMonoBehaviour
    {
        static SingletonManager _instance;
        
        public static SingletonManager Instance
        {
            get
            {
                if (_instance) return _instance;
                _instance = FindAnyObjectByType<SingletonManager>();
                if (_instance) return _instance;
                GameObject singleton = new(nameof(SingletonManager));
                _instance = singleton.AddComponent<SingletonManager>();
                DontDestroyOnLoad(singleton);
                return _instance;
            }
        }

        void Awake()
        {
            if (!_instance)
            {
                _instance = this;
                DOTween.Init().SetCapacity(2048, 1024);
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}