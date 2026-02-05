using Assets.Scripts.Types;
using UnityEngine;
using Zenject;

public class CameraBehaviour : MonoBehaviour
{
    [Inject] private readonly TimeManager _timeManager;
    [Inject] private readonly BoundManager _boundManager;
    [Inject] private readonly StateManager _stateManager;
    [Inject] private readonly CameraModelManager _modelManager;
    [Inject] private readonly Settings _settings;

    void Start()
    {
        IsPeriodEnded(float.MaxValue);
    }

    void LateUpdate()
    {
        CameraBase cameraModel = _modelManager.ActiveModel;

        _timeManager.UpdateNormalizedTime(cameraModel, out float normalizedTime);

        (Vector3 lookFrom, Vector3 lookAt) = cameraModel.Invoke(normalizedTime, cameraModel);

        transform.position = lookFrom;
        transform.LookAt(lookAt);
        // TODO: if (_stateManager.ActiveCameraMode)

        IsPeriodEnded(normalizedTime);
    }

    private void IsPeriodEnded(float normalizedTime)
    {
        if (normalizedTime < 0.999f)
        {
            return;
        }

        _timeManager.Reset();
        _stateManager.Reset();

        _boundManager.Reset(_settings.VolumeName);
        BoundParameters activeBound = _boundManager.ActiveBound;
        _modelManager.Reset(activeBound);

        CameraBase model = _modelManager.ActiveModel;
        Debug.Log($"_currentMode: {activeBound.Description}-{model.Name}");
    }
}