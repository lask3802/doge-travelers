using UnityEngine;
using UnityTemplateProjects.Meteoroid;

namespace UnityTemplateProjects.Weapon
{
    public class LaserHolder : MonoBehaviour
    {
        public AudioSource AudioSource;
        public LineRenderer LineRenderer;
        
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
            LineRenderer.gameObject.SetActive(true);
            AudioSource.Play();
            mDogeController.RecordLaserBegin();
        }

        public void LaserEnd()
        {
            LineRenderer.gameObject.SetActive(false);
            AudioSource.Stop();
            mDogeController.RecordLaserEnd();
        }

        public void FireToTarget(Vector3 position)
        {
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
            mDogeController.RecordLaserFire(position);
        }
    }
}