using System;
using UnityEngine;

namespace UnityTemplateProjects.Weapon
{
    public class SimpleBullet : MonoBehaviour
    {
        public event Action<SimpleBullet> OnHitMeteoroidCallback = delegate {  };

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
            if (collision.gameObject.CompareTag("Meteoroid"))
            {
                OnHitMeteoroidCallback.Invoke(this);
            }
        }
    }
}