using System;
using UnityEngine;
using UnityEngine.Events;

namespace DogeTraveler.UI
{
    public class OnObjectEnabled : MonoBehaviour
    {
        public UnityEvent OnEnabled;
        public UnityEvent OnDisabled;
        private void OnEnable()
        {
            OnEnabled?.Invoke();
        }

        private void OnDisable()
        {
            OnDisabled?.Invoke();
        }
    }
}