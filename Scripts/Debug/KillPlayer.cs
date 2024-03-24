using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class KillPlayer : MonoBehaviour
{
    [SerializeField] private Button killPLayerButton;
    // Start is called before the first frame update
    public static event Action<ulong> OnKillPlayer; 
    
    void Start()
    {
        killPLayerButton.onClick.AddListener(() =>
        {
            OnKillPlayer?.Invoke(NetworkManager.Singleton.LocalClientId);
        });
        
    }
    
}
