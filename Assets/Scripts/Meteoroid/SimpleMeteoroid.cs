using System;
using UnityEngine;

namespace UnityTemplateProjects.Meteoroid
{
    public class SimpleMeteoroid : MonoBehaviour
    {
        private Rigidbody mRigidbody;

        private MeteoroidTarget mTarget;
        private Vector3 mMovement;
        private float mSpeed;
        
        public event Action<SimpleMeteoroid> OnCollideTargetCallBack = delegate {  };
        
        void Awake()
        {
            mRigidbody = GetComponent<Rigidbody>();
        }

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

        /// <summary>
        /// Begin this meteoroid's journey, which is add force(goto target vector * speed) to rigid body
        /// </summary>
        public void Fire()
        {
            mRigidbody.AddForce(mMovement * mSpeed);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject != mTarget.gameObject) return;
            
            OnCollideTargetCallBack.Invoke(this);
        }
    }
}