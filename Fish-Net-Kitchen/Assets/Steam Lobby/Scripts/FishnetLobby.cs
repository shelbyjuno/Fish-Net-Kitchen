using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Transporting;
using FishNet.Transporting;
using FishNet.Transporting.Multipass;
using Steamworks;
using UnityEngine;

public class FishnetLobby : MonoBehaviour
{
    public TransportManager transportManager;
    private Multipass multipass;
    public SteamLobby steamLobby;

    private void Awake()
    {
        if(steamLobby == null) FindFirstObjectByType<SteamLobby>();

        multipass = transportManager.GetTransport<Multipass>();

        steamLobby.OnLobbyCreated += OnLobbyCreated;
        steamLobby.OnLobbyEntered += OnLobbyEntered;
        steamLobby.OnLobbyLeave += OnLobbyLeave;
    }

    private void OnLobbyCreated(CSteamID serverID, string lobbyID)
    {
        // fishySteamworks.SetClientAddress(lobbyID);
        // fishySteamworks.StartConnection(true);

        multipass.SetClientTransport<FishySteamworks.FishySteamworks>();
        multipass.SetClientAddress(lobbyID);
        multipass.StartConnection(true);
    }

    private void OnLobbyEntered(CSteamID serverID, string lobbyID)
    {
        // fishySteamworks.SetClientAddress(lobbyID);
        // fishySteamworks.StartConnection(false);

        multipass.SetClientTransport<FishySteamworks.FishySteamworks>();
        multipass.SetClientAddress(lobbyID);
        multipass.StartConnection(false);
    }

    private void OnLobbyLeave(CSteamID serverID, string lobbyID)
    {
        if(InstanceFinder.IsServerStarted) multipass.StopConnection(true);
        multipass.StopConnection(false);
    }
}
