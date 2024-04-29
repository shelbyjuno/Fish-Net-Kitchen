using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerBrowser : MonoBehaviour
{
    public SteamLobby steamLobby;
    public GameObject templateLobby;
    
    void Awake()
    {
        if(steamLobby == null) FindFirstObjectByType<SteamLobby>();
        steamLobby.OnLobbiesFound += OnLobbiesFound;
    }

    private void OnLobbiesFound(List<SteamLobby.LobbyData> list)
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        foreach (var lobby in list)
        {
            var lobbyObject = Instantiate(templateLobby, transform);

            // Set the avatar
            lobbyObject.transform.GetChild(0).GetComponent<Image>().sprite 
                = SteamHelpers.GetAvatarSprite(SteamHelpers.ConvertToCSteamID(lobby.lobbyID));

            // Set the name
            lobbyObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text 
                = lobby.lobbyName;

            lobbyObject.GetComponent<Button>().onClick.AddListener(() => steamLobby.JoinLobby(lobby.serverID));
        }
    }
}
