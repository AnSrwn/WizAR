using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class GamePlayer : NetworkBehaviour {
    [Header("UI")]
    [SyncVar]
    private string displayName = "Loading...";

    public GameObject endRoundPanel = null;
    public TMP_Text defeatVictoryText = null;

    public NetworkManagerWizAR room;
    public NetworkManagerWizAR Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerWizAR;
        }
    }

    [Header("Spells")]
    public GameObject attackPrefab;
    public GameObject breakAttackPrefab;
    public Transform attackSpawn;
    public GameObject defendPrefab;
    public Transform defendSpawn;

    private Transform avatarPosition;

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);
        Room.GamePlayers.Add(this);
    }

    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

    [Client]
    private void Start() {
        if (isClientOnly)
        {
            FindObjectOfType<ARSessionOrigin>().transform.Translate(-GameObject.Find("SpawnEnemy").transform.position);
        }
        avatarPosition = Camera.main.transform;
        if (isLocalPlayer)
        {
            SpeechRecognizer.attack.AddListener(CmdAttack);
            SpeechRecognizer.shatterAttack.AddListener(CmdShatterAttack);
            SpeechRecognizer.defend.AddListener(CmdDefend);
        }
    }

    [Client]
    private void Update()
    {
        if (!hasAuthority)
        {
            return;
        }
        if (!isLocalPlayer)
        {
            return;
        }


        var avatarRotation = Quaternion.Euler(0, avatarPosition.transform.eulerAngles.y, 0);
        var groundedAvatarPosition = new Vector3(
           avatarPosition.position.x,
           avatarPosition.position.y - 1.0f,
           avatarPosition.position.z
           );


        groundedAvatarPosition -= avatarPosition.forward * 0.5f;
        transform.SetPositionAndRotation(groundedAvatarPosition, avatarRotation);
    }

    [Command]
    private void CmdAttack()
    {
        GameObject attack = Instantiate(attackPrefab, attackSpawn.position, transform.rotation);
        attack.GetComponent<AttackSpell>().PlayerIdentity = netId;
        NetworkServer.Spawn(attack);
        RpcAttack();
    }

    [ClientRpc]
    private void RpcAttack()
    {
        GetComponent<Animation>().CrossFade("attack_short_001", 0.0f);
        GetComponent<Animation>().CrossFadeQueued("idle_combat");
    }

    [Command]
    private void CmdShatterAttack()
    {
        GameObject breakAttack = Instantiate(breakAttackPrefab, attackSpawn.position, transform.rotation);
        breakAttack.GetComponent<ShatterShieldSpell>().PlayerIdentity = netId;
        NetworkServer.Spawn(breakAttack);
        RpcShatterAttack();
    }

    [ClientRpc]
    private void RpcShatterAttack()
    {
        GetComponent<Animation>().CrossFade("attack_short_001", 0.0f);
        GetComponent<Animation>().CrossFadeQueued("idle_combat");
    }

    [Command]
    private void CmdDefend()
    {
        GameObject defend = Instantiate(defendPrefab, defendSpawn.position, defendSpawn.rotation);
        defend.GetComponent<ShieldSpell>().PlayerIdentity = netId;
        NetworkServer.Spawn(defend);        
        RpcDefend();
    }

    [ClientRpc]
    private void RpcDefend()
    {
        GetComponent<Animation>().CrossFadeQueued("idle_combat");
    }

    [Server]
    public void EndRound()
    {
        uint winnerPlayerID = 0;
        if (GetComponent<Health>().currentHealth > 0)
        {
            winnerPlayerID = netId;
        }
        RpcEndRound(winnerPlayerID);
    }

    [ClientRpc]
    private void RpcEndRound(uint winnerPlayerID)
    {
        if (hasAuthority && isLocalPlayer)
        {
            endRoundPanel.SetActive(true);
            GetComponent<Health>().healthBar.HideHealthBar(true);

            if (winnerPlayerID == netId) {
                defeatVictoryText.text = "Victory";
            } else {
                defeatVictoryText.text = "Defeat";
            }

            room.NewRound();
        }
    }

    [Server]
    public void NewRound()
    {
        Invoke(nameof(resetHealth), 5.0f);
        Invoke(nameof(RpcNewRound), 5.0f);
    }

    [ClientRpc]
    private void RpcNewRound()
    {
        endRoundPanel.SetActive(false);
        GetComponent<Health>().healthBar.HideHealthBar(false);
    }

    private void resetHealth()
    {
        GetComponent<Health>().currentHealth = 100;
        GetComponent<Health>().healthBar.SetHealth(100);
    }
}
