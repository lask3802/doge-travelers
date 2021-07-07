using UnityEngine;

namespace UnityTemplateProjects
{
    public class MainBehavior : MonoBehaviour
    {
        private MeteoroidManager mMeteoroidManager;
        
        private void Awake()
        {
            mMeteoroidManager = FindObjectOfType<MeteoroidManager>();
            Application.targetFrameRate = 60;
        }

        private float mCountDown;
        
        void Start()
        {
            mCountDown = 3;
            mMeteoroidManager.RegisterTarget();
            Debug.Log("ready");
        }

        private bool mStart;
        
        void Update()
        {
            mCountDown -= Time.deltaTime;
            if (mCountDown < 0 && !mStart)
            {
                Debug.Log("start!");
                mMeteoroidManager.StartShooting(1);
                mStart = true;
            }
        }
    }
}