using EasyButtons;
using Meteoroid;
using UniRx;
using UnityEngine;

namespace UnityTemplateProjects
{
    public class MainBehavior : MonoBehaviour
    {
        private MeteoroidPatternController mMeteoroidPatternController;
        
        private void Awake()
        {
            mMeteoroidPatternController = FindObjectOfType<MeteoroidPatternController>();
            Application.targetFrameRate = 60;
        }
        
        void Start()
        {
            /*mMeteoroidPatternController.ProgressChanged()
                .Subscribe(value => Debug.Log($"progress: {value}"))
                .AddTo(this);
            
            mMeteoroidPatternController.SpeedChanged()
                .Subscribe(value => Debug.Log($"speed: {value}"))
                .AddTo(this);*/
            
            mMeteoroidPatternController.MeteoroidHitTargetAsObservable()
                .Subscribe(_ => Debug.Log("<color=red>was hit!</color>"));

            mMeteoroidPatternController.ProgressEndAsObservable()
                .Subscribe(_ => Debug.Log("<color=yellow>complete!</color>"));
        }

        [Button]
        public void Pause()
        {
            mMeteoroidPatternController.PatternPause();
            Time.timeScale = 0;
        }

        [Button]
        public void Resume()
        {
            mMeteoroidPatternController.PatternResume();
            Time.timeScale = 1;
        }

        [Button]
        public void Stop()
        {
            mMeteoroidPatternController.PatternStop();
        }

        [Button]
        public void Play()
        {
            mMeteoroidPatternController.PatternStart(1);
        }
    }
}