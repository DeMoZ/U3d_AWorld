using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct FireProjectileSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (projectilePrefab, transform) in SystemAPI.Query<ProjectilePrefab, LocalTransform>()
                     .WithAll<FireProjectileTag>())
        {
            var newProjectile = ecb.Instantiate(projectilePrefab.value);
            var projectileTransform = LocalTransform.FromPositionRotationScale
                (transform.Position, transform.Rotation, 0.5f);

            ecb.SetComponent(newProjectile, projectileTransform);
        }
        
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}