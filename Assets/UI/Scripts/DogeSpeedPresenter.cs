using System;
using UniRx;
using VContainer.Unity;

namespace DogeTraveler.UI
{
    public class DogeSpeedPresenter: ITickable
    {
        private readonly IDogeSpeed m_speed;
        private readonly SpeedView m_view;

        public DogeSpeedPresenter(IDogeSpeed speed, SpeedView view)
        {
            m_speed = speed;
            m_view = view;
        }
        public void Tick()
        {
            if (!m_view) return;
            m_view.SetSpeed(GameProgressManager.Instance.GamePlaySpeed.Value);
        }
    }
}