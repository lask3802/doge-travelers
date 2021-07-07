using DogeTraveler.UI.Mockup;
using VContainer;
using VContainer.Unity;

namespace DogeTraveler.UI
{
    public class UILifetimeScope: LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<DogeSpeedPresenter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<DogeProgressPresenter>(Lifetime.Scoped);

            builder.Register<MockProgress>(Lifetime.Scoped)
                .AsImplementedInterfaces();
            builder.Register<MockSpeed>(Lifetime.Scoped)
                .AsImplementedInterfaces();

            builder.RegisterComponentInHierarchy<SpeedView>();
            builder.RegisterComponentInHierarchy<DogeProgressView>();
        }
    }
}