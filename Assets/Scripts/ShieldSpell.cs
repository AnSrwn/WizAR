using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ShieldSpell : NetworkBehaviour
{
    private uint playerIdentity;
    public uint PlayerIdentity { get => playerIdentity; set => playerIdentity = value; }
    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), 6.0f);
    }

    // destroy for everyone on the server
    [Server]
    public void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
