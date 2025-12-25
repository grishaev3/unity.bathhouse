using Assets.Scripts.Types;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private TimeManager _timeManager;
    private BoundManager _boundManager;
    private StateManager _stateManager;
    private CameraModelManager _modelManager;
    private Settings _settings;

    void Start()
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
        _boundManager.Reset(_settings.VolumeName);
        _stateManager.Reset();

        CameraBase model = _modelManager.ActiveModel;
        (string name, Bounds bound) = _boundManager.ActiveBound;
        //Debug.Log($"_currentMode: {name}-{model.Name}-{_stateManager.ActiveCameraMode}");

        _modelManager.Reset(bound);
    }
}