using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class JoinLobbyMenu : MonoBehaviour
{
    public NetworkManagerWizAR networkManager = null;

    public GameObject landingPagePanel = null;
    public TMP_InputField ipAddressInputField = null;
    public Button joinButton = null;

    private const string PlayerPrefsIpKey = "IpAddress";

    private void OnEnable()
    {
        NetworkManagerWizAR.OnClientConnected += HandleClientConnected;
        NetworkManagerWizAR.OnClientDisconnected += HandleClientDisconnected;

        if (!PlayerPrefs.HasKey(PlayerPrefsIpKey)) { return; }

        string defaultIp = PlayerPrefs.GetString(PlayerPrefsIpKey);
        ipAddressInputField.text = defaultIp;
    }

    private void OnDisable()
    {
        NetworkManagerWizAR.OnClientConnected -= HandleClientConnected;
        NetworkManagerWizAR.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby()
    {
        string ipAddress = ipAddressInputField.text;
        PlayerPrefs.SetString(PlayerPrefsIpKey, ipAddress);
        networkManager.networkAddress = ipAddress;
        networkManager.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
    }
}
