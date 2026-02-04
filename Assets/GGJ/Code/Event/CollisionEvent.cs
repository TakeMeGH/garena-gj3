using System;
using UnityEngine;

namespace GGJ.Code.Event
{
    public class CollisionEvent : MonoBehaviour
    {
        public Action<Collider> OnTriggerEnterEvent;
        public Action<Collider> OnTriggerStayEvent;
        public Action<Collider> OnTriggerExitEvent;
        public Action<Collision> OnCollisionEnterEvent;
        public Action<Collision> OnCollisionStayEvent;
        public Action<Collision> OnCollisionExitEvent;
        void OnTriggerEnter(Collider other)
        {  
            OnTriggerEnterEvent?.Invoke(other);
        }

        void OnTriggerStay(Collider other)
        {
            OnTriggerStayEvent?.Invoke(other);
        }

        void OnTriggerExit(Collider other)
        {
            OnTriggerExitEvent?.Invoke(other);
        }

        void OnCollisionEnter(Collision other)
        {
            OnCollisionEnterEvent?.Invoke(other);
        }

        void OnCollisionStay(Collision other)
        {
            OnCollisionStayEvent?.Invoke(other);
        }

        void OnCollisionExit(Collision other)
        {
            OnCollisionExitEvent?.Invoke(other);
        }
    }
}