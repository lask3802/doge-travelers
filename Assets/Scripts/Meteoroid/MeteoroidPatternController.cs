using System;
using System.Linq;
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
        
        private bool mRunning;
        private int mFramePassed;

        private readonly Subject<Unit> mEndGameSubject = new Subject<Unit>();
        private readonly Subject<float> mProgressSubject = new Subject<float>();
        private readonly Subject<float> mSpeedSubject = new Subject<float>();
        
        private const int FramesOneSec = 60;

        private const int EndGameFrameCount = (3 * 60 + 5) * FramesOneSec;
        private const int StopFireFrameCount = 3 * 60 * FramesOneSec;

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

            if (mFramePassed == EndGameFrameCount)
            {
                mEndGameSubject.OnNext(new Unit());
                PatternStop();
                return;
            }

            MeteoroidPattern pattern;
            if (mFramePassed >= StopFireFrameCount)
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
            mSpeedSubject.OnNext(300000f / pattern.FireArriveTime);
            mProgressSubject.OnNext((float)mFramePassed/EndGameFrameCount);
        }

        private bool IsInTimeRangeSecond(int start, int end)
        {
            return FrameToSecFloor(mFramePassed) >= start && FrameToSecFloor(mFramePassed) < end;
        }

        private static int FrameToSecFloor(int frameCount)
        {
            return frameCount / FramesOneSec;
        }
    }
}