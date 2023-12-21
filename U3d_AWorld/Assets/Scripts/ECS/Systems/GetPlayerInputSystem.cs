using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

[UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
public partial class GetPlayerInputSystem : SystemBase
{
    private ExampleInputActions _exampleInputActions;
    private Entity _playerEntity;

    protected override void OnCreate()
    {
        RequireForUpdate<PlayerTag>();
        RequireForUpdate<PlayerMoveInput>();

        _exampleInputActions = new ExampleInputActions();
    }

    protected override void OnStartRunning()
    {
        _exampleInputActions.Enable();
        _exampleInputActions.ExampleMap.PlayerJump.performed += OnPlayerShoot;
            
        _playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
    }

    protected override void OnStopRunning()
    {
        _exampleInputActions.ExampleMap.PlayerJump.performed -= OnPlayerShoot;
        _exampleInputActions.Disable();
        _playerEntity = Entity.Null;
    }

    protected override void OnUpdate()
    {
        var curMoveInput = _exampleInputActions.ExampleMap.PlayerMovement.ReadValue<Vector2>();
        SystemAPI.SetSingleton(new PlayerMoveInput { value = curMoveInput });
    }

    private void OnPlayerShoot(InputAction.CallbackContext obj)
    {
        if (!SystemAPI.Exists(_playerEntity)) return;

        SystemAPI.SetComponentEnabled<FireProjectileTag>(_playerEntity, true);
    }
}