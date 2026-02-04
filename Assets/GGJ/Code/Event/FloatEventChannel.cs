using System;
using UnityEngine;

namespace GGJ.Code.Event
{
    [CreateAssetMenu(menuName = "GGJ/Events/Float Event Channel")]
    public class FloatEventChannel : ScriptableObject
    {
        public Action<float> OnEventRaised;

        public void RaiseEvent(float value)
        {
            OnEventRaised?.Invoke(value);
        }
        
        public void Subscribe(Action<float> listener)
        {
            OnEventRaised += listener;
        }

        public void Unsubscribe(Action<float> listener)
        {
            OnEventRaised -= listener;
        }
    }

}