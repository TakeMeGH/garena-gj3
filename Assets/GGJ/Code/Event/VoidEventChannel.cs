using System;
using UnityEngine;

namespace GGJ.Code.Event
{
    [CreateAssetMenu(menuName = "GGJ/Events/Void Event Channel")]
    public class VoidEventChannel : ScriptableObject
    {
        public Action OnEventRaised;

        public void RaiseEvent()
        {
            OnEventRaised?.Invoke();
        }
        
        public void Subscribe(Action listener)
        {
            OnEventRaised += listener;
        }

        public void Unsubscribe(Action listener)
        {
            OnEventRaised -= listener;
        }
    }

}