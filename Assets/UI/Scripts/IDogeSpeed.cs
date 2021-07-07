using System;

namespace DogeTraveler.UI
{
    public interface IDogeSpeed
    {
        IObservable<double> DogeSpeedObservable();
    }
}