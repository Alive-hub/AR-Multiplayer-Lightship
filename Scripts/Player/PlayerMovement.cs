using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;



[RequireComponent(typeof(PlayerInputControls))]
public class PlayerMovement : NetworkBehaviour
{
    private PlayerInputControls _playerInputControls;
    private const float MOVE_SPEED = .04f;
    private const float MOVE_THRESHOLD = 0.01f;

    private const float LOOKATPOINT_DELTA = 2f;
    private GameObject lookAtPoint;
    
    public override void OnNetworkSpawn()
    {
        if (GetComponent<NetworkObject>().IsOwner)
        {
            lookAtPoint = new GameObject();

            lookAtPoint.transform.position = transform.position;
            lookAtPoint.transform.rotation = transform.rotation;
            
            _playerInputControls = GetComponent<PlayerInputControls>();
            _playerInputControls.OnMoveInput += PlayerInputControlsOnOnMoveInput;
        }
    }

    private void PlayerInputControlsOnOnMoveInput(Vector3 inputMovement)
    {
        if(inputMovement.magnitude < MOVE_THRESHOLD) return;
        
        
        transform.position += inputMovement * MOVE_SPEED;

        PlayerLookInMovementDirection(inputMovement);
    }


    void PlayerLookInMovementDirection(Vector3 inputVector)
    {
        Vector3 pointToLookAt = transform.position + (inputVector.normalized * LOOKATPOINT_DELTA);

        lookAtPoint.transform.position = pointToLookAt;
        
        transform.LookAt(lookAtPoint.transform);
    }

    public override void OnNetworkDespawn()
    {
        if (GetComponent<NetworkObject>().IsOwner)
        {
            _playerInputControls.OnMoveInput -= PlayerInputControlsOnOnMoveInput;
        }
    }
}
