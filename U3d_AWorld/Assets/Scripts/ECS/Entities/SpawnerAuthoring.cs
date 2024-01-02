using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3 playerPosition;

    [SerializeField] private GameObject botPrefab;
    [SerializeField] private int botInstanceCount;
    [SerializeField] private float botSpawnRate;

    public GameObject PlayerPrefab => playerPrefab;
    public Vector3 PlayerPosition => playerPosition;

    public GameObject BotPrefab => botPrefab;
    public int BotInstanceCount => botInstanceCount;
    public float BotSpawnRate => botSpawnRate;
}

public class SpawnerBaker : Baker<SpawnerAuthoring>
{
    public override void Bake(SpawnerAuthoring authoring)
    {
        // Spawner to Component
        var spawnerEntity = GetEntity(TransformUsageFlags.None);

        var playerPrefab = GetEntity(authoring.PlayerPrefab, TransformUsageFlags.Dynamic);
        var botPrefab = GetEntity(authoring.BotPrefab, TransformUsageFlags.Dynamic);
        var botsAmount = authoring.BotInstanceCount;
        var spawnRate = authoring.BotSpawnRate;

        AddComponent(spawnerEntity, new PlayerSpawner
        {
            playerPrefab = playerPrefab,
        });
        
        AddComponent(spawnerEntity, new BotSpawner
        {
            botPrefab = botPrefab,
            botsAmount = botsAmount,
            spawnRate = spawnRate,
        });
    }
}


public struct PlayerSpawner : IComponentData
{
    public Entity playerPrefab;
    public float3 spawnPoint;
    public bool isSpawned;
}

public struct BotSpawner : IComponentData
{
    public Entity botPrefab;
    public int botsAmount;
    public float spawnRate;
    public float nextBotSpawnTime;
}