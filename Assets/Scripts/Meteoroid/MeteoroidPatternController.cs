using System;
using System.Linq;
using DefaultNamespace;
using UniRx;
using UnityEngine;
using UnityTemplateProjects;

namespace Meteoroid
{
    public class MeteoroidPatternController : MonoBehaviour
    {
        [SerializeField]
        private MeteoroidManager mMeteoroidManager;

        [SerializeField] 
        private MeteoroidPatternSerialized mPatternSerialized;

        [SerializeField]
        private SpeedLinearTable mSpeedLinearTable;
        
        private bool mRunning;
        private int mFramePassed;

        private readonly Subject<Unit> mEndGameSubject = new Subject<Unit>();
        private readonly Subject<float> mProgressSubject = new Subject<float>();
        private readonly Subject<float> mSpeedSubject = new Subject<float>();
        
        private const int FramesOneSec = 60;

        private int mStopFireFrameCount;
        private int mEndGameFrameCount;

        public IObservable<Unit> MeteoroidHitTargetAsObservable()
        {
            return mMeteoroidManager.MeteoroidHitTargetAsObservable();
        }

        public IObservable<float> ProgressChanged()
        {
            return mProgressSubject;
        }

        public IObservable<float> SpeedChanged()
        {
            return mSpeedSubject;
        }

        public IObservable<Unit> ProgressEndAsObservable()
        {
            return mEndGameSubject;
        }

        public void PatternStart(int seed)
        {
            mRunning = true;
            mFramePassed = 0;
            mMeteoroidManager.RegisterTarget();
            mMeteoroidManager.StartShooting(seed);
            mMeteoroidManager.SetMeteoroidPattern(mPatternSerialized.Patterns[0]);
            mStopFireFrameCount = (int)mPatternSerialized.Patterns.Last().Time * FramesOneSec;
            mEndGameFrameCount = mStopFireFrameCount + 5 * FramesOneSec;
        }

        public void PatternPause()
        {
            mRunning = false;
            mMeteoroidManager.Pause();
        }

        public void PatternResume()
        {
            mRunning = true;
            mMeteoroidManager.Resume();
        }

        public void PatternStop()
        {
            mRunning = false;
            mMeteoroidManager.Stop();
        }

        void Update()
        {
            if (!mRunning) return;
            
            mFramePassed++;

            if (mFramePassed == mEndGameFrameCount)
            {
                mEndGameSubject.OnNext(new Unit());
                PatternStop();
                return;
            }

            MeteoroidPattern pattern;
            if (mFramePassed >= mStopFireFrameCount)
            {
                pattern = new MeteoroidPattern {FireDuration = int.MaxValue};
            }
            else
            {
                pattern = mPatternSerialized.Patterns
                    .OrderByDescending(p => p.Time)
                    .First(p => mFramePassed >= p.Time * FramesOneSec);
            }
            
            mMeteoroidManager.SetMeteoroidPattern(pattern);
            SetSpeedHappyGauge();
            mProgressSubject.OnNext((float)mFramePassed/mEndGameFrameCount);
        }

        private void SetSpeedHappyGauge()
        {
            var currentTime = mFramePassed / (float)FramesOneSec;
            var elements = mSpeedLinearTable.Elements;
            if (currentTime >= elements.Last().Time)
            {
                mSpeedSubject.OnNext(elements.Last().SpeedAtTime);
                return;
            }
            
            for (var i = 0; i < elements.Count; i++)
            {
                if (currentTime >= elements[i].Time && currentTime <= elements[i + 1].Time)
                {
                    var speed = elements[i].SpeedAtTime
                                + (elements[i + 1].SpeedAtTime - elements[i].SpeedAtTime)
                                * ((currentTime - elements[i].Time) / (elements[i + 1].Time - elements[i].Time));
                    mSpeedSubject.OnNext(speed);
                }
            }
        }
    }
}