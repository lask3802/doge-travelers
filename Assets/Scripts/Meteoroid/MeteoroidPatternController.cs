using System;
using UniRx;
using UnityEngine;
using UnityTemplateProjects;

namespace Meteoroid
{
    public class MeteoroidPatternController : MonoBehaviour
    {
        [SerializeField]
        private MeteoroidManager mMeteoroidManager;
        private bool mRunning;
        private int mFramePassed;

        private readonly Subject<Unit> mEndGameSubject = new Subject<Unit>();
        private readonly Subject<float> mProgressSubject = new Subject<float>();
        private readonly Subject<float> mSpeedSubject = new Subject<float>();

        private const int EndGameFrameCount = 3 * 60 * 60;

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
            
            mMeteoroidManager.SetMeteoroidPattern(new MeteoroidPattern
            {
                FireCount = FrameToSec(mFramePassed) / 30 + 1,
                FireSpeed = FrameToSec(mFramePassed) / 20 + 11,
                FireDuration = 2 * 60 - (FrameToSec(mFramePassed) / 50) * 30,
                FireSize = 5
            });
            
            mProgressSubject.OnNext((float)mFramePassed/EndGameFrameCount);
            mSpeedSubject.OnNext(FrameToSec(mFramePassed) / 10 + 10);
        }

        private int FrameToSec(int frameCount)
        {
            // assume 1 sec = 60 frames

            return frameCount / 60;
        }
    }
}