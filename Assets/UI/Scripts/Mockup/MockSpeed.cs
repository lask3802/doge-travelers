using UnityEngine;

namespace DogeTraveler.UI.Mockup
{
    public class MockSpeed: IDogeSpeed
    {
        public double GetSpeed()
        {
            return Random.value*11234;
        }
    }
}