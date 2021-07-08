using System;
using TMPro;
using UniRx;
using UnityEngine;

namespace DogeTraveler.UI
{
    public class SpeedView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI SpeedText;

        private void Start()
        {
            GameProgressManager.Instance.GamePlaySpeed.Subscribe(s => SetSpeed(s)).AddTo(this);
        }

        public void SetSpeed(double speed)
        {
            SpeedText.text = $"Speed: {speed:F2} km/s";
        }
    }
}