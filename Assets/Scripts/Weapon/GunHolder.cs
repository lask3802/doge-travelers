using UnityEngine;

namespace UnityTemplateProjects.Weapon
{
    public class GunHolder : MonoBehaviour
    {
        public SimpleBullet Bullet;
        private float FireSpeed = 15000;

        public AudioSource AudioSource;
        
        private WeaponManager mWeaponManager;
        private DogeController mDogeController;
        
        void Awake()
        {
            mWeaponManager = FindObjectOfType<WeaponManager>();
            mDogeController = GetComponentInParent<DogeController>();
            Bullet = mWeaponManager.GetComponentInChildren<SimpleBullet>();
        }

        public SimpleBullet FireToTarget(Vector3 position)
        {
            var bullet = Instantiate(Bullet, mWeaponManager.transform);
            bullet.transform.position = transform.position;
            bullet.SetEndPosition(position);
            bullet.SetSpeed(FireSpeed);
            bullet.Fire();
            AudioSource.Play();
            mDogeController.RecordGunFire(position);
            return bullet;
        }
    }
}