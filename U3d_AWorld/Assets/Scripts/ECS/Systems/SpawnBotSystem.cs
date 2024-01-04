using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct SpawnBotSystem : ISystem
{
    private bool _isEnabled;

    private Random _random;
    public void OnCreate(ref SystemState state)
    {
        _isEnabled = true;
        
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        _random = new Random((uint)System.DateTime.Now.Millisecond);
        //_random = new Random(12345);
    }

    public void OnDestroy(ref SystemState state) { }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!_isEnabled) return;
        
        var ecb = GetEntityCommandBuffer(ref state);
        // Creates a new instance of the job, assigns the necessary data, and schedules the job in parallel.
        new ProcessBotSpawnerJob
        {
            ecb = ecb,
            elapsedTime = SystemAPI.Time.ElapsedTime,
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
public partial struct ProcessBotSpawnerJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;
    public double elapsedTime;
    public uint seed;
    
    // IJobEntity generates a component data query based on the parameters of its `Execute` method.
    // This example queries for all Spawner components and uses `ref` to specify that the operation
    // requires read and write access. Unity processes `Execute` for each entity that matches the
    // component data query.
    private void Execute([ChunkIndexInQuery] int chunkIndex, ref BotSpawner spawner)
    {
        if (spawner.nextBotSpawnTime < elapsedTime)
        {
            var random = new Random(seed);
            var position = random.NextFloat3(new float3(-10, 0, -10), new float3(10, 0, 10));
            var newEntity = ecb.Instantiate(chunkIndex, spawner.botPrefab);
            ecb.SetComponent(chunkIndex, newEntity, LocalTransform.FromPosition(position));

            // Resets the next spawn time.
            spawner.nextBotSpawnTime = (float)elapsedTime + spawner.spawnRate;
        }
    }
}