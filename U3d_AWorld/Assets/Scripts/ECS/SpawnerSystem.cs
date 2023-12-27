using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public partial struct SpawnerSystem : ISystem
{
    private Random _random;

    public void OnCreate(ref SystemState state)
    {
        //_random = new Random(123456);
        _random = new Random((uint)System.DateTime.Now.Millisecond);
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var spawner in SystemAPI.Query<RefRW<PlayerSpawner>>())
        {
            if (spawner.ValueRO.isSpawned) continue;

            spawner.ValueRW.isSpawned = true;
            var newEntity = state.EntityManager.Instantiate(spawner.ValueRO.playerPrefab);
            var position = RandomPosition(new float3(-10, 0, -10), new float3(10, 0, 10));
            state.EntityManager.SetComponentData(newEntity, LocalTransform.FromPosition(position));
        }
        
        // Queries for all Spawner components. Uses RefRW because this system wants
        // to read from and write to the component. If the system only needed read-only
        // access, it would use RefRO instead.
        foreach (var spawner in SystemAPI.Query<RefRW<BotSpawner>>())
        {
            ProcessSpawner(ref state, spawner);
        }
    }

    private void ProcessSpawner(ref SystemState state, RefRW<BotSpawner> spawner)
    {
        if (spawner.ValueRO.nextBotSpawnTime < SystemAPI.Time.ElapsedTime)
        {
            var newEntity = state.EntityManager.Instantiate(spawner.ValueRO.botPrefab);
            var position = RandomPosition(new float3(-10, 0, -10), new float3(10, 0, 10));
            state.EntityManager.SetComponentData(newEntity, LocalTransform.FromPosition(position));

            // Resets the next spawn time.
            spawner.ValueRW.nextBotSpawnTime = (float)SystemAPI.Time.ElapsedTime + spawner.ValueRO.spawnRate;
        }
    }

    private float3 RandomPosition(float3 minPoint, float3 maxPoint)
    {
        return _random.NextFloat3(minPoint, maxPoint);
    }
    
    private quaternion RandomRotation()
    {
        float3 euler = new() {
            y = _random.NextFloat(0.0f, 360.0f)
        };
        return quaternion.Euler(euler);
    }

    /*
     private float3 RandomPosition(float2 area)
    {
        // Random position at x and z
        float3 position = new() {
            x = random.NextFloat(this.minPos, this.maxPos),
            z = random.NextFloat(this.minPos, this.maxPos)
        };

        // Set LocalTransform
        this.commandBuffer.SetComponent(instance,
            LocalTransform.FromPositionRotation(position, rotation));
    }
    */
}