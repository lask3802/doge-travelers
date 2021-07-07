using System;
using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using UnityEngine;
using UnityTemplateProjects.Meteoroid;
using Random = System.Random;

namespace UnityTemplateProjects
{
    public class MeteoroidManager : MonoBehaviour
    {
        public SimpleMeteoroid MeteoroidBasic;

        private List<SimpleMeteoroid> mExistMeteoroids = new List<SimpleMeteoroid>();
        private MeteoroidTarget mTarget;
        private Random mRandom;

        private bool mRunning;
        private int mFramesPassed;
        
        public int FireDuration = 300;
        public float FireSpeed = 5;
        public float FireSpeedDelta = 3;
        public int FireCount = 1;
        public float FireMeteoroidSize = 3;
        
        public float FirePlaneLength = 200f;
        public float FirePlaneHeight = 100f;
        public float FirePlaneZ = 200f;

        public float PlayerPlaneLength = 10f;
        public float PlayerPlaneHeight = 5f;
        public float PlayerPlaneZ = -5f;
        
        public event Action OnMeteoroidHitTargetCallback = delegate {  };

        public IEnumerable<SimpleMeteoroid> ExistMeteoroids => mExistMeteoroids;
        
        public void RegisterTarget()
        {
            mTarget = FindObjectOfType<MeteoroidTarget>();
        }

        public void StartShooting(int randSeed)
        {
            mRunning = true;
            mFramesPassed = 0;
            mRandom = new Random(randSeed);
        }

        [Button]
        public void Stop()
        {
            mRunning = false;
            RemoveAllMeteoroids();
        }

        [Button]
        public void Pause()
        {
            Time.timeScale = 0f;
            mRunning = false;
        }

        [Button]
        public void Resume()
        {
            Time.timeScale = 1;
            mRunning = true;
        }

        public void RemoveMeteoroid(SimpleMeteoroid meteoroid)
        {
            mExistMeteoroids.Remove(meteoroid);
            Destroy(meteoroid.gameObject);
        }

        void Update()
        {
            if (!mRunning) return;
            
            mFramesPassed++;

            if (mFramesPassed % FireDuration == 0)
            {
                for (var i = 0; i < FireCount; i++)
                {
                    CreateMeteoroid(i);
                }
            }

            PassPlayerPlaneMeteoroidCheck();
        }

        private void OnMeteoroidHitTarget(SimpleMeteoroid meteoroid)
        {
            meteoroid.OnCollideTargetCallBack -= OnMeteoroidHitTarget;
            Debug.LogError($"{meteoroid.name} hit target!");
            mExistMeteoroids.Remove(meteoroid);
            Destroy(meteoroid.gameObject);
            OnMeteoroidHitTargetCallback.Invoke();
        }

        private void PassPlayerPlaneMeteoroidCheck()
        {
            var removeCandidate = mExistMeteoroids
                .Where(meteoroid => meteoroid.transform.position.z <= PlayerPlaneZ - 1)
                .ToList();

            foreach (var meteoroid in removeCandidate)
            {
                Debug.Log($"{meteoroid.name} not hit target");
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
            
            var x = (float) mRandom.NextDouble() * 2 * FirePlaneLength - FirePlaneLength;
            var y = (float) mRandom.NextDouble() * 2 * FirePlaneHeight - FirePlaneHeight;
            meteoroid.SetInitPosition(new Vector3(x, y, FirePlaneZ));
            meteoroid.SetTarget(mTarget);
                    
                    
            x = (float) mRandom.NextDouble() * 2 * PlayerPlaneHeight - PlayerPlaneHeight;
            y = (float) mRandom.NextDouble() * 2 * PlayerPlaneLength - PlayerPlaneLength;
            if (roundIndex % 2 == 0)
            {
                meteoroid.SetEndPosition(mTarget.transform.position);
            }
            else
            {
                meteoroid.SetEndPosition(new Vector3(x, y, PlayerPlaneZ));
            }
            meteoroid.SetSpeed(FireSpeed + (float)(mRandom.NextDouble() * FireSpeedDelta));
            meteoroid.SetSize(FireMeteoroidSize);
            meteoroid.OnCollideTargetCallBack += OnMeteoroidHitTarget;
            meteoroid.name = $"Meteoroid (time: {mFramesPassed})";
                
            mExistMeteoroids.Add(meteoroid);
            meteoroid.Fire();
            return meteoroid;
        }
    }
}