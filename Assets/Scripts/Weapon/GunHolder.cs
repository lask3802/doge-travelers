using UnityEngine;

namespace UnityTemplateProjects.Weapon
{
    public class GunHolder : MonoBehaviour
    {
        public SimpleBullet Bullet;
        public float FireSpeed = 300;

        private WeaponManager mWeaponManager;
        
        void Awake()
        {
            mWeaponManager = FindObjectOfType<WeaponManager>();
        }

        public SimpleBullet FireToTarget(Vector3 position)
        {
            var bullet = Instantiate(Bullet, mWeaponManager.transform);
            bullet.transform.position = transform.position;
            bullet.SetEndPosition(position);
            bullet.SetSpeed(FireSpeed);
            bullet.Fire();
            return bullet;
        }
    }
}