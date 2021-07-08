using System;
using Doozy.Engine.Progress;
using UniRx;
using UnityEngine;

namespace DogeTraveler.UI
{
    [ExecuteAlways]
    public class DogeProgressView : MonoBehaviour
    {
        [SerializeField] private RectTransform ProgressBar;

        [SerializeField] private RectTransform Indicator;
        // Start is called before the first frame update
        [SerializeField] private Progressor Progressor;
        [Range(0,1f)]
        public float Progress = 0;
        // Update is called once per frame
        private void Start()
        {
            GameProgressManager.Instance.GamePlayProgress.Subscribe(s =>
            {
                Progressor.SetProgress(s);
                Indicator.anchoredPosition = s * ProgressBar.rect.height * Vector2.up +
                                             Indicator.anchoredPosition * Vector2.right;

            }).AddTo(this);
        }
    }
}
