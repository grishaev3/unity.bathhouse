using System;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private double _msCurrentTime = 0d;

    private BoundManager _boundManager = null;
    private StateManager _stateManager = null;
    private CameraModelManager _modelManager = null;

    void Awake()
    {
        _boundManager = new BoundManager();
        _stateManager = new StateManager();
        _modelManager = new CameraModelManager(_boundManager.ActiveBound.bound);

        CheckAndReset(float.MaxValue);
    }

    void LateUpdate()
    {

        CameraBase model = _modelManager.ActiveModel;
        float normalizedTime = GetNormalizedTime(model);
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

        CheckAndReset(normalizedTime);
    }

    private void CheckAndReset(float normalizedTime)
    {
        if (normalizedTime < 0.999f)
        {
            return;
        }

        _msCurrentTime = 0d;

        _boundManager.Reset();
        _stateManager.Reset();

        CameraBase model = _modelManager.ActiveModel;
        (string name, Bounds bound) = _boundManager.ActiveBound;
        Debug.Log($"_currentMode: {name}-{model.Name}-{_stateManager.ActiveCameraMode}");

        _modelManager.Reset(bound);
    }

    private float GetNormalizedTime(CameraBase model)
    {
        _msCurrentTime += Time.deltaTime * TimeSpan.FromSeconds(1).TotalMilliseconds;

        // учитываем что замедлили время 
        double normalizedTime = _msCurrentTime / (model.Duration.TotalMilliseconds / (1d / Time.timeScale));

        return (float)normalizedTime;
    }
}