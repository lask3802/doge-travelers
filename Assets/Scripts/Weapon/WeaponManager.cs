using System.Collections.Generic;
using System.Linq;
using EasyButtons;
using UnityEngine;
using UnityTemplateProjects.Meteoroid;

namespace UnityTemplateProjects.Weapon
{
    public class WeaponManager : MonoBehaviour
    {
        private List<SimpleBullet> mFiredBullet = new List<SimpleBullet>();
        private MeteoroidManager mMeteoroidManager;
        private GunHolder mGunHolder;

        public Camera DogeCamera;

        void Awake()
        {
            mMeteoroidManager = FindObjectOfType<MeteoroidManager>();
            mGunHolder = FindObjectOfType<GunHolder>();
        }
        
        public void RegisterGunHolder(GunHolder gunHolder)
        {
            mGunHolder = gunHolder;
        }

        public void GunFire(Vector3 position)
        {
            Debug.Log("gun shot!");
            var bullet = mGunHolder.FireToTarget(position);
            bullet.OnHitMeteoroidCallback += BulletHitMeteoroid;
            mFiredBullet.Add(bullet);
        }

        private void BulletHitMeteoroid(SimpleMeteoroid meteoroid, SimpleBullet bullet)
        {
            mMeteoroidManager.RemoveMeteoroid(meteoroid);
            mFiredBullet.Remove(bullet);
            Destroy(bullet.gameObject);
        }
        private void Update()
        {
            var removedCandidate = mFiredBullet.Where(b => b.transform.position.z > 500).ToList();
            foreach (var bullet in removedCandidate)
            {
                mFiredBullet.Remove(bullet);
                Destroy(bullet.gameObject);
            }

            if (Input.GetMouseButtonDown(0))
            {
                GetTargetFromMouseClick();
                //DebugFire();
            }
        }

        private void GetTargetFromMouseClick()
        {
            var ray = DogeCamera.ScreenPointToRay(Input.mousePosition);
        
            Debug.Log("Try get meteoroid from click");
            var allHits = Physics.RaycastAll(ray);
            foreach (var hit in allHits)
            {
                var meteoroid = hit.transform.GetComponent<SimpleMeteoroid>();
                if (meteoroid == null) continue;
                
                GunFire(meteoroid.transform.position);
                break;
            }
        }

        [Button]
        public void DebugFire()
        {
            var meteoroids = mMeteoroidManager.ExistMeteoroids
                .Where(m => m.transform.position.z > mGunHolder.transform.position.z).ToList();
            if (meteoroids.Count == 0)
            {
                Debug.Log("no meteoroids");
            }
            else
            {
                GunFire(meteoroids[0].transform.position);
            }
        }
    }
}