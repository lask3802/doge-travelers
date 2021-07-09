using System;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace
{
    public class WeaponHintController : MonoBehaviour
    {
        public GameObject BulletHint;
        public GameObject LaserHint;
        
        void Start()
        {
            GameProgressManager.Instance.GameProgressState.Subscribe(state =>
            {
                switch (state)
                {
                    case GameState.Play1:
                        BulletHint.SetActive(false);
                        LaserHint.SetActive(false);
                        break;
                    case GameState.Play2:
                        BulletHint.SetActive(true);
                        LaserHint.SetActive(false);
                        break;
                    case GameState.Play3:
                        BulletHint.SetActive(true);
                        LaserHint.SetActive(true);
                        break;
                }
            });
        }
    }
}