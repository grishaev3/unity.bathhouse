using Assets.Scripts.Types;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {

        Container.Bind<BoundManager>().AsSingle().NonLazy();
        Container.Bind<StateManager>().AsSingle().NonLazy();
        Container.Bind<TimeManager>().FromMethod((context) =>
        {
            var settings = context.Container.Resolve<Settings>();
            return new TimeManager(settings);
        }).AsTransient().NonLazy();

        Container.Bind<CameraModelManager>().FromMethod((context) =>
        {
            var boundManager = context.Container.Resolve<BoundManager>();
            var settings = context.Container.Resolve<Settings>();
            return new CameraModelManager(boundManager.ActiveBound, settings);
        }).AsSingle().NonLazy();

        Container.Bind<SettingsManager>().AsSingle().WithArguments(PresetLevel.Low).NonLazy();
        Container.Bind<Settings>().FromMethod((context) =>
        {
            var settingsManager = context.Container.Resolve<SettingsManager>();
            return settingsManager.Current;
        }).AsSingle().NonLazy();
    }
}