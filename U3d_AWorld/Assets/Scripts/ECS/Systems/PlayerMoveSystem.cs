using Unity.Burst;
using Unity.Entities;

public partial struct PlayerMoveSystem : ISystem
{
    private bool _isEnabled;

    public void OnCreate(ref SystemState state)
    {
        _isEnabled = true;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!_isEnabled) return;
        
        var deltaTime = SystemAPI.Time.DeltaTime;
        new PlayerMoveJob
        {
            deltaTime = deltaTime
        }.Schedule();
    }
}