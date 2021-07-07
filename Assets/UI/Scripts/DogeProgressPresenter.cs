using VContainer.Unity;

namespace DogeTraveler.UI
{
    public class DogeProgressPresenter: ITickable
    {
        private readonly IDogeProgress m_progress;
        private readonly DogeProgressView m_view;

        public DogeProgressPresenter(IDogeProgress progress, DogeProgressView view)
        {
            m_progress = progress;
            m_view = view;
        }

        public void Tick()
        {
            if (!m_view) return;
            m_view.Progress = m_progress.GetProgress();
        }
    }
}