using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class AllPlayerDataManager : NetworkBehaviour
{
    public static AllPlayerDataManager Instance;

    private NetworkList<PlayerData> allPlayerData;
    private const int LIFEPOINTS = 4;
    private const int LIFEPOINTS_TO_REDUCE = 1;

    public event Action<ulong> OnPlayerDead;
    public event Action<ulong> OnPlayerHealthChanged;
    
    private void Awake()
    {
        allPlayerData = new NetworkList<PlayerData>();
        
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }

        Instance = this;
        
    }
    
    public void AddPlacedPlayer(ulong id)
    {
        for (int i = 0; i < allPlayerData.Count; i++)
        {
            if (allPlayerData[i].clientID == id)
            {
                PlayerData newData = new PlayerData(
                    allPlayerData[i].clientID,
                    allPlayerData[i].score,
                    allPlayerData[i].lifePoints,
                    true
                );
                
                allPlayerData[i] = newData;
            }
        }
    }
    public bool GetHasPlacerPlaced(ulong id)
    {
        for (int i = 0; i < allPlayerData.Count; i++)
        {
            if (allPlayerData[i].clientID == id)
            {
                return allPlayerData[i].playerPlaced;
            }
        }

        return false;
    }


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            AddNewClientToList(NetworkManager.LocalClientId);
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += AddNewClientToList;
        BulletData.OnHitPlayer += BulletDataOnOnHitPlayer;
        KillPlayer.OnKillPlayer += KillPlayerOnOnKillPlayer;
        RestartGame.OnRestartGame += RestartGameOnOnRestartGame;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= AddNewClientToList;
        BulletData.OnHitPlayer -= BulletDataOnOnHitPlayer;
        KillPlayer.OnKillPlayer -= KillPlayerOnOnKillPlayer;
        RestartGame.OnRestartGame -= RestartGameOnOnRestartGame;
    }


    private void RestartGameOnOnRestartGame()
    {
        if(!IsServer) return;

        List<NetworkObject> playerObjects = FindObjectsOfType<PlayerMovement>()
            .Select(x => x.transform.GetComponent<NetworkObject>()).ToList();

        List<NetworkObject> bulletObjects = FindObjectsOfType<BulletData>()
            .Select(x => x.transform.GetComponent<NetworkObject>()).ToList();



        foreach (var playerobj in playerObjects)
        {
           playerobj.Despawn(); 
        }

        foreach (var bulletObject in bulletObjects)
        {
            bulletObject.Despawn();
        }

        ResetNetworkList();
    }


    void ResetNetworkList()
    {
        for (int i = 0; i < allPlayerData.Count; i++)
        {
            PlayerData resetPLayer = new PlayerData(
                allPlayerData[i].clientID,
                playerPlaced: false,
                lifePoints: LIFEPOINTS,
                score: 0
                );

            allPlayerData[i] = resetPLayer;
        }
    }

    private void KillPlayerOnOnKillPlayer(ulong id)
    {
        (ulong, ulong) fromTO = new(555, id);
        BulletDataOnOnHitPlayer(fromTO);
    }

    public float GetPlayerHealth(ulong id)
    {
        for (int i = 0; i < allPlayerData.Count; i++)
        {
            if (allPlayerData[i].clientID == id)
            {
                return allPlayerData[i].lifePoints;
            }
        }

        return default;
    }

    private void BulletDataOnOnHitPlayer((ulong from, ulong to) ids)
    {
        if (IsServer)
        {
            if (ids.from != ids.to)
            {
                for (int i = 0; i < allPlayerData.Count; i++)
                {
                    if (allPlayerData[i].clientID == ids.to)
                    {
                        int lifePointsToReduce = allPlayerData[i].lifePoints == 0 ? 0 : LIFEPOINTS_TO_REDUCE;

                        PlayerData newData = new PlayerData(
                            allPlayerData[i].clientID,
                            allPlayerData[i].score,
                            allPlayerData[i].lifePoints - lifePointsToReduce,
                            allPlayerData[i].playerPlaced
                        );
                        
                        

                        if (newData.lifePoints <= 0)
                        {
                            OnPlayerDead?.Invoke(ids.to);
                        }
                        
                        
                        
                        Debug.Log("Player got hit " + ids.to + " lifepoints left => " + newData.lifePoints +  " shot by " + ids.from);

                        allPlayerData[i] = newData;
                        break;
                    }
                }
            }
        }

        SyncReducePlayerHealthClientRpc(ids.to);
    }

    [ClientRpc]
    void SyncReducePlayerHealthClientRpc(ulong hitID)
    {
        OnPlayerHealthChanged?.Invoke(hitID);
    }

    void AddNewClientToList(ulong clientID)
    {
        
        if(!IsServer) return;


        foreach (var playerData in allPlayerData)
        {
            if(playerData.clientID == clientID) return;
        }

        PlayerData newPlayerData = new PlayerData();
        newPlayerData.clientID = clientID;
        newPlayerData.score = 0;
        newPlayerData.lifePoints = LIFEPOINTS;
        newPlayerData.playerPlaced = false;
        
        if(allPlayerData.Contains(newPlayerData)) return;
        
        allPlayerData.Add(newPlayerData);
        PrintAllPlayerPlayerList();
    }


    void PrintAllPlayerPlayerList()
    {
        foreach (var playerData in allPlayerData)
        {
            Debug.Log("Player ID => " + playerData.clientID + " hasPlaced " + playerData.playerPlaced + " Called by " + NetworkManager.Singleton.LocalClientId);
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
