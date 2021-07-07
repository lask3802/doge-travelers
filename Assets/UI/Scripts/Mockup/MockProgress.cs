using UnityEngine;

namespace DogeTraveler.UI.Mockup
{
    public class MockProgress: IDogeProgress
    {
        public float GetProgress()
        {
            return Random.value;
        }
    }
}