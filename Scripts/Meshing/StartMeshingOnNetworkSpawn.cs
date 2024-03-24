using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class StartMeshingOnNetworkSpawn : NetworkBehaviour
{
    [SerializeField] private ARMeshManager _meshManager;
    // Start is called before the first frame update
    void Start()
    {
        _meshManager.enabled = false;
    }


    public override void OnNetworkSpawn()
    {
        _meshManager.enabled = true;
    }
}
