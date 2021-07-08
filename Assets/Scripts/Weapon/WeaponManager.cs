using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityTemplateProjects.Meteoroid;

namespace UnityTemplateProjects.Weapon
{
    public class WeaponManager : MonoBehaviour
    {
        private List<SimpleBullet> mFiredBullet = new List<SimpleBullet>();
        private MeteoroidManager mMeteoroidManager;
        private GunHolder mGunHolder;
        private bool mRunning;

        void Awake()
        {
            mMeteoroidManager = FindObjectOfType<MeteoroidManager>();
            mGunHolder = FindObjectOfType<GunHolder>();
        }
        
        public void RegisterGunHolder(GunHolder gunHolder)
        {
            mGunHolder = gunHolder;
        }

        public void RunWeapon()
        {
            mRunning = true;
        }

        public void StopWeapon()
        {
            mRunning = false;
        }

        public void GunFire(Vector3 position)
        {
            var bullet = mGunHolder.FireToTarget(position);
            bullet.OnHitMeteoroidCallback += BulletHitMeteoroid;
            mFiredBullet.Add(bullet);
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
                GunFire(target);
            }
        }

        public Vector3 GetTargetFromMouseClick()
        {
            var camera = Camera.main;
            
            var ray = camera.ScreenPointToRay(Input.mousePosition);
        
            var allHits = Physics.RaycastAll(ray);
            foreach (var hit in allHits)
            {
                var meteoroid = hit.transform.GetComponent<SimpleMeteoroid>();
                if (meteoroid == null) continue;
                if (meteoroid.transform.position.z < 5) continue;
                return meteoroid.transform.position;
            }
            var mouse = Input.mousePosition;
            mouse.z = 200;
            return camera.ScreenToWorldPoint(mouse);
        }
    }
}