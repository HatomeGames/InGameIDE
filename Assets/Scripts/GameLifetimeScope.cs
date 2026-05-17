using UnityEngine;
using VContainer;
using VContainer.Unity;
using CodeView;
using IGP;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<GameInput>(Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<ProgramViewManager>().AsSelf();
        builder.RegisterComponentInHierarchy<CodeViewManager>().AsSelf();
        builder.RegisterComponentInHierarchy<Logger>().AsSelf();
        builder.RegisterComponentInHierarchy<Runtime>().AsSelf();
    }
}
