using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class MoveSystem : MonoBehaviour//SystemBase
{
    // protected override void OnUpdate()
    // {
    //     float deltaTime = Time.DeltaTime;
    //
    //     Entities.ForEach((ref Translation translation, in MoveComponent moveComponent) =>
    //     {
    //         translation.Value.x += moveComponent.Speed * deltaTime;
    //     }).Schedule();
    // }
    // protected override void OnUpdate()
    // {
    //     throw new System.NotImplementedException();
    // }
}