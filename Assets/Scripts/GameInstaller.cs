using Assets.Scripts.Types;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {

        Container.Bind<BoundManager>().AsSingle();
        Container.Bind<StateManager>().AsSingle();
        Container.Bind<TimeManager>().AsTransient();

        Container.Bind<CameraModelManager>().FromMethod((context) =>
        {
            var boundManager = context.Container.Resolve<BoundManager>();
            return new CameraModelManager(boundManager.ActiveBound.bound);
        }).AsSingle();

        Container.Bind<SettingsManager>().AsSingle().WithArguments(PresetLevel.Low);
        Container.Bind<Settings>().FromMethod((context) =>
        {
            var settingsManager = context.Container.Resolve<SettingsManager>();
            return settingsManager.Current;
        }).AsSingle();


        var ss = Container.Resolve<Settings>();
    }
}