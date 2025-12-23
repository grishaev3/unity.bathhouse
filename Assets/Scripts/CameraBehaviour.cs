using Assets.Scripts.Types;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private TimeManager _timeManager;
    private BoundManager _boundManager;
    private StateManager _stateManager;
    private CameraModelManager _modelManager;
    private Settings _settings;

    void Awake()
    {
        _settings = SettingsManager.Current;

        _timeManager = new TimeManager();
        _boundManager = new BoundManager();
        _stateManager = new StateManager();
        _modelManager = new CameraModelManager(_boundManager.ActiveBound.bound);

        IsPeriodEnded(float.MaxValue);
    }

    void LateUpdate()
    {
        CameraBase model = _modelManager.ActiveModel;

        _timeManager.UpdateNormalizedTime(model, out float normalizedTime);

        Vector3 position = model.Func(normalizedTime, model);

        transform.position = position;
        if (_stateManager.ActiveCameraMode)
        {
            transform.LookAt(new Vector3(position.x, position.y, -2f));
        }
        else
        {
            transform.LookAt(new Vector3(0f, 0f, -2f));
        }

        IsPeriodEnded(normalizedTime);
    }

    private void IsPeriodEnded(float normalizedTime)
    {
        if (normalizedTime < 0.999f)
        {
            return;
        }

        _timeManager.Reset();
        _boundManager.Reset(_settings.VolumeName);
        _stateManager.Reset();

        CameraBase model = _modelManager.ActiveModel;
        (string name, Bounds bound) = _boundManager.ActiveBound;
        Debug.Log($"_currentMode: {name}-{model.Name}-{_stateManager.ActiveCameraMode}");

        _modelManager.Reset(bound);
    }
}