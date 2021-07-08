using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Meteoroid;
using UniRx;
using UnityEngine;
using UnityTemplateProjects.Meteoroid;
using Random = System.Random;

namespace UnityTemplateProjects
{
    public class MeteoroidManager : MonoBehaviour
    {
        public SimpleMeteoroid MeteoroidBasic;

        private readonly List<SimpleMeteoroid> mExistMeteoroids = new List<SimpleMeteoroid>();
        private MeteoroidTarget mTarget;
        private Random mRandom;

        private bool mRunning;
        private int mWaitFrames;
        
        public float FireDuration = 5;
        public float FireArriveTime = 10;
        public int FireCount = 1;

        private int FireDelay = 6;
        
        private float FirePlaneLength = 300f;
        private float FirePlaneHeight = 150f;
        private float FirePlaneZ = 300f;

        private float PlayerPlaneLength = 10f;
        private float PlayerPlaneHeight = 10f;
        private float PlayerPlaneZ = 7f;

        private readonly Subject<Unit> mMeteoroidHitTargetSubject = new Subject<Unit>();

        public IObservable<Unit> MeteoroidHitTargetAsObservable()
        {
            return mMeteoroidHitTargetSubject;
        }

        public IEnumerable<SimpleMeteoroid> ExistMeteoroids => mExistMeteoroids;
        
        public void RegisterTarget()
        {
            mTarget = FindObjectOfType<MeteoroidTarget>();
        }

        public void StartShooting(int randSeed)
        {
            mRunning = true;
            mWaitFrames = 0;
            mRandom = new Random(randSeed);
        }

        public void SetMeteoroidPattern(MeteoroidPattern pattern)
        {
            FireCount = pattern.FireCount;
            FireDuration = pattern.FireDuration;
            FireArriveTime = pattern.FireArriveTime;
        }

        public void Pause()
        {
            mRunning = false;
        }

        public void Resume()
        {
            mRunning = true;
        }

        public void Stop()
        {
            mRunning = false;
            RemoveAllMeteoroids();
        }

        public void ExplodeMeteoroid(SimpleMeteoroid meteoroid)
        {
            mExistMeteoroids.Remove(meteoroid);
            meteoroid.Explode();
            Destroy(meteoroid.gameObject);
        }

        public void SilentRemoveMeteoroid(SimpleMeteoroid meteoroid)
        {
            mExistMeteoroids.Remove(meteoroid);
            Destroy(meteoroid.gameObject);
        }

        void Update()
        {
            if (!mRunning) return;
            mWaitFrames--;

            if (mWaitFrames < 0)
            {
                for (var i = 0; i < FireCount; i++)
                {
                    CreateMeteoroid(i);
                }

                mWaitFrames = (int)(FireDuration * 60);
            }

            PassPlayerPlaneMeteoroidCheck();
        }

        private void PassPlayerPlaneMeteoroidCheck()
        {
            var removeCandidate = mExistMeteoroids
                .Where(meteoroid => meteoroid.transform.position.z <= PlayerPlaneZ - 10)
                .ToList();

            foreach (var meteoroid in removeCandidate)
            {
                mExistMeteoroids.Remove(meteoroid);
                Destroy(meteoroid.gameObject);
            }
        }

        private void RemoveAllMeteoroids()
        {
            foreach (var meteoroid in mExistMeteoroids)
            {
                Destroy(meteoroid.gameObject);
            }
            mExistMeteoroids.Clear();
        }

        private SimpleMeteoroid CreateMeteoroid(int roundIndex)
        {
            var meteoroid = Instantiate(MeteoroidBasic, transform);
            meteoroid.transform.localPosition = Vector3.zero;
            
            var x = mRandom.Next(-4, 5);
            var y = mRandom.Next(-4, 5);

            var lengthBlock = FirePlaneLength * 2 / 8;
            var heightBlock = FirePlaneHeight * 2 / 8;

            meteoroid.SetInitPosition(new Vector3(x * lengthBlock, y * heightBlock, FirePlaneZ));
            meteoroid.SetTarget(mTarget);
            
            lengthBlock = PlayerPlaneLength * 2 / 8;
            heightBlock = PlayerPlaneHeight * 2 / 8;
            
            meteoroid.SetEndPosition(new Vector3(x * lengthBlock, y * heightBlock, PlayerPlaneZ));
            
            meteoroid.SetArriveTime(FireArriveTime);
            meteoroid.SetSize(1);
            meteoroid.OnCollideTargetCallBack += OnMeteoroidHitTarget;
            meteoroid.OnCollideAnotherMeteoroidCallBack += OnMeteoroidHitAnotherMeteoroid;
                
            mExistMeteoroids.Add(meteoroid);
            if (roundIndex != 0)
            {
                StartCoroutine(DelayFire(FireDelay * roundIndex, meteoroid));
            }
            else
            {
                meteoroid.Fire(mRandom);
            }
            return meteoroid;
        }

        private void OnMeteoroidHitTarget(SimpleMeteoroid meteoroid)
        {
            meteoroid.OnCollideTargetCallBack -= OnMeteoroidHitTarget;
            mMeteoroidHitTargetSubject.OnNext(new Unit());
            ExplodeMeteoroid(meteoroid);
        }

        private void OnMeteoroidHitAnotherMeteoroid(SimpleMeteoroid meteoroid)
        {
            meteoroid.OnCollideAnotherMeteoroidCallBack -= OnMeteoroidHitAnotherMeteoroid;
            if (meteoroid.transform.position.z < PlayerPlaneZ + 20)
            {
                SilentRemoveMeteoroid(meteoroid);
            }
            else
            {
                ExplodeMeteoroid(meteoroid);
            }
        }

        private IEnumerator DelayFire(int delay, SimpleMeteoroid meteoroid)
        {
            while (delay != 0)
            {
                delay--;
                yield return null;
            }

            if (meteoroid != null)
            {
                meteoroid.Fire(mRandom);
            }
        }
    }
}