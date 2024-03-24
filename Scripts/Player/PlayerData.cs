using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientID;
    public int score;
    public float lifePoints;
    public bool playerPlaced;


    public PlayerData(ulong clientID, int score, float lifePoints, bool playerPlaced)
    {
        this.clientID = clientID;
        this.score = score;
        this.lifePoints = lifePoints;
        this.playerPlaced = playerPlaced;
    }
    
    

    public bool Equals(PlayerData other)
    {
        return (
            other.playerPlaced == playerPlaced &&
            other.lifePoints == lifePoints &&
            other.score == score &&
            other.clientID == clientID
        );
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientID);
        serializer.SerializeValue(ref score);
        serializer.SerializeValue(ref lifePoints);
        serializer.SerializeValue(ref playerPlaced);
    }
}
