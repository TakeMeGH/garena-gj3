using System;
using UnityEngine;

namespace GGJ.Code.Event
{
    public class ParticleSystemEvent : MonoBehaviour
    {
        public Action<GameObject> OnParticleSystemStoppedEvent;

        void OnParticleSystemStopped()
        {
            OnParticleSystemStoppedEvent?.Invoke(gameObject);
        }
    }
}