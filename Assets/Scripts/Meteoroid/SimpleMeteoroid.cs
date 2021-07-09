using System;
using EasyButtons;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityTemplateProjects.Meteoroid
{
    public class SimpleMeteoroid : MonoBehaviour
    {
        [SerializeField]
        private GameObject mAsteroidBreakEffect;
        [SerializeField]
        private Rigidbody mRigidbody;

        private MeteoroidTarget mTarget;
        private Vector3 mEndPosition;
        private float mArriveTime;

        private bool mFired;
        
        public event Action<SimpleMeteoroid> OnCollideTargetCallBack = delegate {  };
        public event Action<SimpleMeteoroid> OnCollideOthersCallBack = delegate {  };

        public void SetInitPosition(Vector3 initPosition)
        {
            transform.position = initPosition;
        }
        
        
        public void SetTarget(MeteoroidTarget target)
        {
            mTarget = target;
        }

        public void SetEndPosition(Vector3 position)
        {
            mEndPosition = position;
        }

        public void SetArriveTime(float arriveTime)
        {
            mArriveTime = arriveTime;
        }

        public void SetSize(float size)
        {
            transform.localScale = new Vector3(size, size, size);
        }

        public void Fire(System.Random random)
        {
            var rangeVector = mEndPosition - transform.position;
            var distance = rangeVector.magnitude;
            var speedUnit = distance / mArriveTime;

            var direction = rangeVector.normalized;
            mRigidbody.AddForce(direction * (speedUnit * 50)); // magic number here

            var x = 5 * ((float)random.NextDouble() * 360 - 180);
            var y = 5 * ((float)random.NextDouble() * 360 - 180);
            var z = 5 * ((float)random.NextDouble() * 360 - 180);
            
            mRigidbody.AddTorque(new Vector3(x, y, z));
            mFired = true;
        }

        public void Explode()
        {
            var explodeEffect = Instantiate(mAsteroidBreakEffect);
            explodeEffect.transform.position = transform.position;
            var particles = explodeEffect.GetComponentsInChildren<ParticleSystem>();
            foreach (var particle in particles)
            {
                particle.transform.localScale = transform.localScale;
            }
            explodeEffect.transform.rotation = Random.rotation;
            explodeEffect.SetActive(true);
            Destroy(explodeEffect, 3f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("WeaponCollider"))
            {
                OnCollideOthersCallBack.Invoke(this);
            }
            else
            {
                other.GetComponentInParent<DogeController>().ShouldPlayExplode = true;
                OnCollideTargetCallBack.Invoke(this);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag("WeaponCollider"))
            {
                OnCollideOthersCallBack.Invoke(this);
            }
            else
            {
                other.GetComponentInParent<DogeController>().ShouldPlayExplode = true;
                OnCollideTargetCallBack.Invoke(this);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Meteoroid") || collision.gameObject.CompareTag("WeaponCollider"))
            {
                OnCollideOthersCallBack.Invoke(this);
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag("Meteoroid") || collision.gameObject.CompareTag("WeaponCollider"))
            {
                OnCollideOthersCallBack.Invoke(this);
            }
        }
    }
}