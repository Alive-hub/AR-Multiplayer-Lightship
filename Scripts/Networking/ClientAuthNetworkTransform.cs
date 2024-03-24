using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;


[DisallowMultipleComponent]
public class ClientAuthNetworkTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
