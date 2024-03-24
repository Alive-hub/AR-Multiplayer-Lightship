using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputControls : NetworkBehaviour
{
    private PlayerControlsInputAction _playerControlsInputAction;
    private Vector3 movementVector;

    public event Action<Vector3> OnMoveInput;
    public event Action OnMoveActionCancelled;
    public event Action OnShootInput;
    public event Action OnShootInputCancelled;

    public event Action<Vector2> OnShootAnglePerformed;

    public override void OnNetworkSpawn()
    {
        if (GetComponent<NetworkObject>().IsOwner)
        {
            _playerControlsInputAction = new PlayerControlsInputAction();
            _playerControlsInputAction.Enable();
            
            
            
            _playerControlsInputAction.PlayerControlsMap.Move.performed += MoveActionPerformed;
            _playerControlsInputAction.PlayerControlsMap.Move.canceled += MoveActionCancelled;
            
            _playerControlsInputAction.PlayerControlsMap.Shoot.performed += ShootOnperformed;
            _playerControlsInputAction.PlayerControlsMap.Shoot.canceled += ShootOncanceled;
            
            _playerControlsInputAction.PlayerControlsMap.ShootAngle.performed += ShootAngleOnperformed;
            
            
        }
    }

    private void ShootAngleOnperformed(InputAction.CallbackContext context)
    {
        OnShootAnglePerformed?.Invoke(context.ReadValue<Vector2>());
    }

    private void ShootOncanceled(InputAction.CallbackContext obj)
    {
        OnShootInputCancelled?.Invoke();
    }

    private void ShootOnperformed(InputAction.CallbackContext obj)
    {
        OnShootInput?.Invoke();
    }

    private void MoveActionCancelled(InputAction.CallbackContext context)
    {
        movementVector = Vector3.zero;
        OnMoveActionCancelled?.Invoke();
    }

    private void MoveActionPerformed(InputAction.CallbackContext context)
    {
        Vector2 v2Movement = context.ReadValue<Vector2>();
        movementVector = new Vector3(v2Movement.x, 0, v2Movement.y);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (movementVector != Vector3.zero)
        {
            OnMoveInput?.Invoke(movementVector);
        }
    }


    public override void OnNetworkDespawn()
    {
        if (GetComponent<NetworkObject>().IsOwner)
        {
            _playerControlsInputAction.PlayerControlsMap.Move.performed -= MoveActionPerformed;
            _playerControlsInputAction.PlayerControlsMap.Move.canceled -= MoveActionCancelled;
            _playerControlsInputAction.PlayerControlsMap.Shoot.performed -= ShootOnperformed;
            _playerControlsInputAction.PlayerControlsMap.Shoot.canceled -= ShootOncanceled;
        }
    }
}
