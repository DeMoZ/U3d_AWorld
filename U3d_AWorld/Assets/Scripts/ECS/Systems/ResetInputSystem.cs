using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[UpdateInGroup (typeof(SimulationSystemGroup), OrderLast = true)]
[UpdateAfter (typeof(EndSimulationEntityCommandBufferSystem) )]
public partial struct ResetInputSystem : ISystem
{
    private bool _isEnabled;
    
    public void OnCreate(ref SystemState state)
    {
        _isEnabled = false;
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!_isEnabled) return;
        
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (tag, entity) in SystemAPI.Query<FireProjectileTag>().WithEntityAccess())
            ecb.SetComponentEnabled<FireProjectileTag>(entity, false);
    }
}