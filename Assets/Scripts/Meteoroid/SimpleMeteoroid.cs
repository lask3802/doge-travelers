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
        [SerializeField]
        private AudioSource mExplodeSound;

        private MeteoroidTarget mTarget;
        private Vector3 mMovement;
        private float mSpeed;
        
        public event Action<SimpleMeteoroid> OnCollideTargetCallBack = delegate {  };
        public event Action<SimpleMeteoroid> OnCollideAnotherMeteoroidCallBack = delegate {  };

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
            mMovement = position - transform.position;
        }

        /// <summary>
        /// vector from start to target, multiply speed
        /// </summary>
        /// <param name="speed"></param>
        public void SetSpeed(float speed)
        {
            mSpeed = speed;
        }

        public void SetSize(float size)
        {
            transform.localScale = new Vector3(size, size, size);
        }

        public void Fire(System.Random random)
        {
            mRigidbody.AddForce(mMovement * mSpeed);
            var x = 5 * ((float)random.NextDouble() * 360 - 180);
            var y = 5 * ((float)random.NextDouble() * 360 - 180);
            var z = 5 * ((float)random.NextDouble() * 360 - 180);
            mRigidbody.AddTorque(new Vector3(x, y, z));
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

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<SimpleMeteoroid>() != null)
            {
                OnCollideAnotherMeteoroidCallBack.Invoke(this);
            }

            if (collision.gameObject == mTarget.gameObject)
            {
                OnCollideTargetCallBack.Invoke(this);
            }
        }
    }
}