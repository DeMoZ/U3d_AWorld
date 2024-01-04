using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct SpawnPlayerSystem : ISystem
{
    private bool _isEnabled;
    
    private Random _random;
    private bool _isSpawned;

    public void OnCreate(ref SystemState state)
    {
        _isEnabled = true;
        
        _isSpawned = false;
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        _random = new Random((uint)System.DateTime.Now.Millisecond);
        //_random = new Random(12345);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!_isEnabled) return;
        
        if (_isSpawned) return;
        _isSpawned = true;

        var ecb = GetEntityCommandBuffer(ref state);

        new ProcessPlayerSpawnerJob
        {
            ecb = ecb,
            seed = _random.NextUInt(),
        }.ScheduleParallel();
    }

    private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        return ecb.AsParallelWriter();
    }
}

[BurstCompile]
public partial struct ProcessPlayerSpawnerJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;
    public uint seed;
    
    private void Execute([ChunkIndexInQuery] int chunkIndex, ref PlayerSpawner spawner)
    {
        var random = new Random(seed);
        var position = random.NextFloat3(new float3(-10, 0, -10), new float3(10, 0, 10));
        var newEntity = ecb.Instantiate(chunkIndex, spawner.playerPrefab);
        ecb.SetComponent(chunkIndex, newEntity, LocalTransform.FromPosition(position));
        ecb.AddComponent<PlayerTag>(chunkIndex, newEntity);
    }
}