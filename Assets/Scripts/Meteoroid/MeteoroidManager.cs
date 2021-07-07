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
        
        public int FireDuration = 120;
        public float FireSpeed = 10;
        public int FireCount = 1;
        public float FireMeteoroidSize = 5;

        private int FireDelay = 30;
        
        private float FirePlaneLength = 300f;
        private float FirePlaneHeight = 150f;
        private float FirePlaneZ = 300f;

        private float PlayerPlaneLength = 20f;
        private float PlayerPlaneHeight = 20f;
        private float PlayerPlaneZ = 0f;

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
            FireSpeed = pattern.FireSpeed;
            FireMeteoroidSize = pattern.FireSize;
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

        public void RemoveMeteoroid(SimpleMeteoroid meteoroid)
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

                mWaitFrames = FireDuration;
            }

            PassPlayerPlaneMeteoroidCheck();
        }

        private void OnMeteoroidHitTarget(SimpleMeteoroid meteoroid)
        {
            meteoroid.OnCollideTargetCallBack -= OnMeteoroidHitTarget;
            mMeteoroidHitTargetSubject.OnNext(new Unit());
            mExistMeteoroids.Remove(meteoroid);
            Destroy(meteoroid.gameObject);
        }

        private void PassPlayerPlaneMeteoroidCheck()
        {
            var removeCandidate = mExistMeteoroids
                .Where(meteoroid => meteoroid.transform.position.z <= PlayerPlaneZ - 8)
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
            
            //var x = (float) mRandom.NextDouble() * 2 * FirePlaneLength - FirePlaneLength;
            //var y = (float) mRandom.NextDouble() * 2 * FirePlaneHeight - FirePlaneHeight;

            var x = mRandom.Next(-4, 5);
            var y = mRandom.Next(-4, 5);

            var lengthBlock = FirePlaneLength * 2 / 8;
            var heightBlock = FirePlaneHeight * 2 / 8;

            meteoroid.SetInitPosition(new Vector3(x * lengthBlock, y * heightBlock, FirePlaneZ));
            meteoroid.SetTarget(mTarget);

            var xInt = mRandom.Next(-4, 5);
            var yInt = mRandom.Next(-4, 5);
            
            meteoroid.SetEndPosition(new Vector3(xInt * 5, yInt * 5, PlayerPlaneZ));
            
            meteoroid.SetSpeed(FireSpeed);
            meteoroid.SetSize(FireMeteoroidSize);
            meteoroid.OnCollideTargetCallBack += OnMeteoroidHitTarget;
                
            mExistMeteoroids.Add(meteoroid);
            if (roundIndex != 0)
            {
                StartCoroutine(DelayFire(FireDelay * roundIndex, meteoroid));
            }
            else
            {
                meteoroid.Fire();
            }
            meteoroid.Fire();
            return meteoroid;
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
                meteoroid.Fire();
            }
        }
    }
}