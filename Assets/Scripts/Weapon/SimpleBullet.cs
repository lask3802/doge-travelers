using System;
using UnityEngine;
using UnityTemplateProjects.Meteoroid;

namespace UnityTemplateProjects.Weapon
{
    public class SimpleBullet : MonoBehaviour
    {
        public event Action<SimpleMeteoroid, SimpleBullet> OnHitMeteoroidCallback = delegate {  };

        private Rigidbody mRigidbody;
        private Vector3 mMovement;
        private float mSpeed;

        private void Awake()
        {
            mRigidbody = GetComponent<Rigidbody>();
        }

        public void SetEndPosition(Vector3 position)
        {
            mMovement = position - transform.position;
            mMovement /= mMovement.magnitude;
        }

        public void SetSpeed(float speed)
        {
            mSpeed = speed;
        }

        public void Fire()
        {
            mRigidbody.AddForce(mMovement * mSpeed);
        }

        private void OnCollisionEnter(Collision collision)
        {
            var meteoroidCandidate = collision.gameObject.GetComponent<SimpleMeteoroid>();
            if (meteoroidCandidate != null)
            {
                Debug.Log("bullet hit meteoroid");
                OnHitMeteoroidCallback.Invoke(meteoroidCandidate, this);
            }
        }
    }
}