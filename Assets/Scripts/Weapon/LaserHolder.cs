using UnityEngine;

namespace UnityTemplateProjects.Weapon
{
    public class LaserHolder : MonoBehaviour
    {
        public AudioSource AudioSource;
        
        public GameObject LaserVisual;
        public GameObject Laser;
        
        private WeaponManager mWeaponManager;
        private DogeController mDogeController;
        private MeteoroidManager mMeteoroidManager;
        
        void Awake()
        {
            mWeaponManager = FindObjectOfType<WeaponManager>();
            mMeteoroidManager = FindObjectOfType<MeteoroidManager>();
            mDogeController = GetComponentInParent<DogeController>();
        }

        public void LaserBegin()
        {
            LaserVisual.SetActive(true);
            Laser.SetActive(true);
            AudioSource.Play();
            mDogeController.RecordLaserBegin();
        }

        public void LaserEnd()
        {
            LaserVisual.SetActive(false);
            Laser.SetActive(false);
            AudioSource.Stop();
            mDogeController.RecordLaserEnd();
        }

        public void FireToTarget(Vector3 position)
        {
            /*
            var positionThis = transform.position;
            var ray = new Ray(positionThis, position - positionThis);
            var allHits = Physics.RaycastAll(ray);
            foreach (var hit in allHits)
            {
                var meteoroid = hit.transform.GetComponent<SimpleMeteoroid>();
                if (meteoroid == null) continue;
                mMeteoroidManager.ExplodeMeteoroid(meteoroid);
            }
            LineRenderer.SetPosition(1, position - positionThis);
            */
            // let physics collider do everything
            
            mDogeController.RecordLaserFire(position);
        }
    }
}