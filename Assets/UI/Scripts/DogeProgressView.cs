using UnityEngine;

namespace DogeTraveler.UI
{
    [ExecuteAlways]
    public class DogeProgressView : MonoBehaviour
    {
        [SerializeField] private RectTransform ProgressBar;

        [SerializeField] private RectTransform Indicator;
        // Start is called before the first frame update

        public float Progress = 0;
        // Update is called once per frame
        void Update()
        {
            Indicator.anchoredPosition = Progress * ProgressBar.rect.height * Vector2.up + Indicator.anchoredPosition*Vector2.right;
        }
    }
}
