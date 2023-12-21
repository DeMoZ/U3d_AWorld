using Unity.Burst;
using Unity.Entities;

public partial struct PlayerMoveSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        new PlayerMoveJob
        {
            deltaTime = deltaTime
        }.Schedule();
    }
}