using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AttackSpell : NetworkBehaviour
{
    public GameObject explosionEffect;

    private uint playerIdentity;

    public uint PlayerIdentity { get => playerIdentity; set => playerIdentity = value; }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), 4.0f);
    }

    // set velocity for server and client. this way we don't have to sync the
    // position, because both the server and the client simulate it.
    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * 2.0f;
        GetComponent<ParticleSystem>().Play();
    }

    // destroy for everyone on the server
    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    // ServerCallback because we don't want a warning if OnCollisionEnter is
    // called on the client
    [ServerCallback]
    void OnTriggerEnter(Collider collider)
    {
        var hit = collider.gameObject;

        if (hit.CompareTag("Shield"))
        {
            NetworkServer.Spawn(Instantiate(explosionEffect));
            DestroySelf();
        }
        if (hit.CompareTag("Player") && !(hit.GetComponent<GamePlayer>().netId == PlayerIdentity))
        {
            var playerHealth = hit.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10);
            }
            NetworkServer.Spawn(Instantiate(explosionEffect));
            DestroySelf();
        }

    }
}
