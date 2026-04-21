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
        OnPeriodEnd(float.MaxValue);
    }

    void LateUpdate()
    {
        CameraBase cameraModel = _modelManager.ActiveModel;

        _timeManager.UpdateNormalizedTime(cameraModel, out float normalizedTime);

        (Vector3 lookFrom, Vector3 lookAt) = cameraModel.Invoke(normalizedTime, cameraModel);

        transform.position = lookFrom;
        transform.LookAt(lookAt);

        OnPeriodEnd(normalizedTime);
    }

    private void OnPeriodEnd(float normalizedTime)
    {
        if (normalizedTime < 0.999f)
        {
            return;
        }

        _timeManager.Reset();
        _stateManager.Reset();

        _modelManager.ResetModel();
        CameraBase model = _modelManager.ActiveModel;

        if (_modelManager.ActiveModelUsesBounds)
        {
            _boundManager.Reset(_settings.VolumeName);
            BoundParameters activeBound = _boundManager.ActiveBound;
            _modelManager.Reset(activeBound);

            Debug.Log($"_currentMode: {activeBound.Description}-{model.Name}");
        }
        else
        {
            Debug.Log($"_currentMode: {model.Name}");
        }
    }
}