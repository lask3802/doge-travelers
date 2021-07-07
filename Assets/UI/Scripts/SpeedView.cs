using TMPro;
using UnityEngine;

namespace DogeTraveler.UI
{
    public class SpeedView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI SpeedText;

        void SetSpeed(double speed)
        {
            SpeedText.text = $"Speed: {speed:F2} km/s";
        }
    }
}