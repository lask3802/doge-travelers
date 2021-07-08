using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityTemplateProjects.Meteoroid;

namespace UnityTemplateProjects.Weapon
{
    public class WeaponManager : MonoBehaviour
    {
        public Camera DogeCamera;
        public GunHolder MainGunHolder;
        public LaserHolder MainLaserHolder;
        
        private List<SimpleBullet> mFiredBullet = new List<SimpleBullet>();
        private MeteoroidManager mMeteoroidManager;
        private GunHolder mGunHolder;
        private LaserHolder mLaserHolder;
        private bool mRunning;

        private readonly HashSet<LaserHolder> mRegisteredLaserHolders = new HashSet<LaserHolder>();

        void Awake()
        {
            mMeteoroidManager = FindObjectOfType<MeteoroidManager>();
            mGunHolder = FindObjectOfType<GunHolder>();
        }
        
        public void RegisterGunHolder(GunHolder gunHolder)
        {
            mGunHolder = gunHolder;
        }

        public void RegisterLaserHolder(LaserHolder laserHolder)
        {
            mLaserHolder = laserHolder;
            if (!mRegisteredLaserHolders.Contains(laserHolder))
            {
                mRegisteredLaserHolders.Add(laserHolder);
            }
        }

        public void RunWeapon()
        {
            mRunning = true;
        }

        public void StopWeapon()
        {
            mRunning = false;
            foreach (var bullet in mFiredBullet)
            {
                Destroy(bullet.gameObject);
            }
            mFiredBullet.Clear();
            MainLaserHolder.LaserEnd();
            foreach (var laserHolder in mRegisteredLaserHolders)
            {
                laserHolder.LaserEnd();
            }
            mRegisteredLaserHolders.Clear();
        }

        public void GunFire(Vector3 position)
        {
            var bullet = mGunHolder.FireToTarget(position);
            bullet.OnHitMeteoroidCallback += BulletHitMeteoroid;
            mFiredBullet.Add(bullet);
        }

        public void LaserFire(Vector3 position)
        {
            mLaserHolder.FireToTarget(position);
        }

        public void LaserBegin()
        {
            mLaserHolder.LaserBegin();
        }

        public void LaserEnd()
        {
            mLaserHolder.LaserEnd();
        }

        private void BulletHitMeteoroid(SimpleMeteoroid meteoroid, SimpleBullet bullet)
        {
            mMeteoroidManager.ExplodeMeteoroid(meteoroid);
            mFiredBullet.Remove(bullet);
            Destroy(bullet.gameObject);
        }
        private void Update()
        {
            if (!mRunning) return;
            
            var removedCandidate = mFiredBullet.Where(b => b.transform.position.z > 300).ToList();
            foreach (var bullet in removedCandidate)
            {
                mFiredBullet.Remove(bullet);
                Destroy(bullet.gameObject);
            }

            if (Input.GetMouseButtonDown(0))
            {
                var target = GetTargetFromMouseClick();
                mGunHolder = MainGunHolder;
                GunFire(target);
            }

            LaserInputProcess();
        }

        public Vector3 GetLaserEndPointFromMouseClick()
        {
            var camera = DogeCamera;
            var mouse = Input.mousePosition;
            mouse.z = 300;
            return camera.ScreenToWorldPoint(mouse);
        }

        public Vector3 GetTargetFromMouseClick()
        {
            var camera = DogeCamera;
            
            var ray = camera.ScreenPointToRay(Input.mousePosition);
        
            var allHits = Physics.RaycastAll(ray);
            foreach (var hit in allHits)
            {
                var meteoroid = hit.transform.GetComponent<SimpleMeteoroid>();
                if (meteoroid == null) continue;
                if (meteoroid.transform.position.z <= transform.position.z + 0.5f) continue;
                return meteoroid.transform.position;
            }
            var mouse = Input.mousePosition;
            mouse.z = 200;
            return camera.ScreenToWorldPoint(mouse);
        }

        private void LaserInputProcess()
        {
            if (Input.GetMouseButtonDown(1))
            {
                mLaserHolder = MainLaserHolder;
                LaserBegin();
            }

            if (Input.GetMouseButton(1))
            {
                var target = GetLaserEndPointFromMouseClick();
                mLaserHolder = MainLaserHolder;
                LaserFire(target);
            }

            if (Input.GetMouseButtonUp(1))
            {
                mLaserHolder = MainLaserHolder;
                LaserEnd();
            }
        }
    }
}