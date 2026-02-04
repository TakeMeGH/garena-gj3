using Sirenix.OdinInspector;
using UnityEngine;

namespace GGJ.Code.Utils.Singleton
{
    public class Singleton<T> : SerializedMonoBehaviour where T : SerializedMonoBehaviour
    {
        static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance) return _instance;
                _instance = FindAnyObjectByType<T>();

                if (_instance) return _instance;
                GameObject singleton = new(typeof(T).Name);
                _instance = singleton.AddComponent<T>();
                return _instance;
            }
        }

        void Awake()
        {
            if (!_instance)
            {
                _instance = this as T;
                OwnAwake();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            OwnDestroy();
        }

        protected virtual void OwnAwake()
        {
        }

        protected virtual void OwnDestroy()
        {
        }
    }
}