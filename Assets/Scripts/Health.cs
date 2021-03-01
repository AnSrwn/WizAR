using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class Health : NetworkBehaviour
{
    public HealthBar healthBar;
    private const int maxHealth = 100;

    [SyncVar(hook = nameof(OnHealthChange))]
    public int currentHealth = maxHealth;

    private void Start() {

        // Canvas is disabled before, because otherwise the canvas of all players would be overlaying each other
        if (isLocalPlayer)
        {
            GetComponentInChildren<Canvas>().enabled = true;
        }
    }

    [Server]
    public void TakeDamage(int amount)
    {
        if (!isServer)
        {
            return;
        }

        currentHealth = Mathf.Max(currentHealth - amount, 0);
    }

    public void OnHealthChange(int oldHealth, int newHealth) {
        healthBar.SetHealth(newHealth);

        if (newHealth <= 0)
        {
            GetComponent<Animation>().CrossFade("dead", 0.0f);
            if (isServer)
            {
                gameObject.GetComponent<GamePlayer>().Room.EndRound();
            }
        } else if (newHealth > oldHealth) {
            GetComponent<Animation>().CrossFadeQueued("idle_normal");
        } else
        {
            GetComponent<Animation>().CrossFade("damage_001", 0.0f);
        }
    }
}
