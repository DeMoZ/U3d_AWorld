using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct PlayerMoveJob : IJobEntity
{
    public float deltaTime;

    [BurstCompile]
    private void Execute(ref LocalTransform transform, in PlayerMoveInput moveInput, PlayerMoveSpeed moveSpeed)
    {
        transform.Position.xz += moveInput.value * moveSpeed.value * deltaTime;
        if (math.lengthsq(moveInput.value) > float.Epsilon)
        {
            var forward = new float3(moveInput.value.x, 0f, moveInput.value.y);
            transform.Rotation = quaternion.LookRotation(forward, math.up());
        }
    }
}