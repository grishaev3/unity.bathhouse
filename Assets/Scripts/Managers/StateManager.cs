using Assets.Scripts;

class StateManager : IResetable<object>
{
    private bool _currentCameraMode;

    public bool ActiveCameraMode => _currentCameraMode;

    public void Reset(object o = null)
    {
        _currentCameraMode = UniqueRandom.NextBool();
    }
}
