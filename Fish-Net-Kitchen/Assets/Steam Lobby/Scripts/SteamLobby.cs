using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

public class SteamLobby : MonoBehaviour
{
    // Const
    public const string DEFAULT_LOBBY_TAG = "Fishnet";
    public const string LOBBY_TAG = "LobbyTag";
    public const string LOBBY_ID = "LobbyID";
    public const string LOBBY_NAME = "LobbyName";

    // Settings
    public bool debugMessages = false;
    public bool automaticallyRefreshLobbies = true;
    public int maxPlayers = 4;
    public ELobbyType lobbyType = ELobbyType.k_ELobbyTypeFriendsOnly;

    // Callbacks
    private Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    private Callback<LobbyCreated_t> lobbyCreated;
    private Callback<LobbyEnter_t> lobbyEntered;
    private Callback<LobbyChatUpdate_t> lobbyChatUpdate;
    private Callback<LobbyMatchList_t> lobbyMatchList;

    // Actions
    public Action<CSteamID, string> OnLobbyCreated;
    public Action<CSteamID, string> OnLobbyEntered;
    public Action<CSteamID, string> OnLobbyLeave;
    public Action<List<LobbyData>> OnLobbiesFound;

    // Private
    public CSteamID ServerID { get; private set; }
    public string LobbyID { get; private set; }

    public struct LobbyData { public CSteamID serverID; public string lobbyID; public string lobbyName; }
    private List<LobbyData> lobbies = new List<LobbyData>();

    private void Awake()
    {
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(GameLobbyJoinRequested);
        lobbyCreated = Callback<LobbyCreated_t>.Create(LobbyCreated);
        lobbyEntered = Callback<LobbyEnter_t>.Create(LobbyEntered);
        lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(LobbyChatUpdate);
        lobbyMatchList = Callback<LobbyMatchList_t>.Create(LobbyMatchList);
    
        if(automaticallyRefreshLobbies) InvokeRepeating(nameof(SearchLobbies), 0, 2);
    }
    
    private void OnDestroy()
    {
        gameLobbyJoinRequested.Dispose();
        lobbyCreated.Dispose();
        lobbyEntered.Dispose();
        lobbyChatUpdate.Dispose();
        lobbyMatchList.Dispose();
    }

    public void CreateLobby()
    {
        if(ServerID != CSteamID.Nil) return;

        SteamMatchmaking.CreateLobby(lobbyType, maxPlayers);
    }

    public void JoinLobby() => JoinLobby(ServerID);
    public void JoinLobby(CSteamID lobbyID)
    {
        if(ServerID != CSteamID.Nil) return;

        SteamMatchmaking.JoinLobby(lobbyID);
    }

    public void LeaveLobby() => LeaveLobby(ServerID);
    public void LeaveLobby(CSteamID lobbyID)
    {
        if(ServerID == CSteamID.Nil) return;

        SteamMatchmaking.LeaveLobby(lobbyID);
        ServerID = CSteamID.Nil;
        LobbyID = string.Empty;

        OnLobbyLeave?.Invoke(ServerID, LobbyID);
    }

    public void SearchLobbies()
    {
        if(!SteamManager.Initialized) return;

        SteamMatchmaking.AddRequestLobbyListStringFilter(LOBBY_TAG, DEFAULT_LOBBY_TAG, ELobbyComparison.k_ELobbyComparisonEqual);
        SteamMatchmaking.RequestLobbyList();
    }

    private void LobbyMatchList(LobbyMatchList_t param)
    {
        PrintDebug($"Found {param.m_nLobbiesMatching} lobbies.");

        lobbies.Clear();

        for (int i = 0; i < param.m_nLobbiesMatching; i++)
        {
            CSteamID serverID = SteamMatchmaking.GetLobbyByIndex(i);
            string lobbyTag = SteamMatchmaking.GetLobbyData(serverID, LOBBY_TAG);
            string lobbyID = SteamMatchmaking.GetLobbyData(serverID, LOBBY_ID);
            string lobbyName = SteamMatchmaking.GetLobbyData(serverID, LOBBY_NAME);

            // ignore duplicate lobbies
            if(lobbies.Exists(x => x.lobbyID == lobbyID)) continue;

            PrintDebug($"Lobby: {lobbyID} - {lobbyTag} - {lobbyID} - {lobbyName}");

            lobbies.Add(new LobbyData { serverID = serverID, lobbyID = lobbyID, lobbyName = lobbyName});
        }

        OnLobbiesFound?.Invoke(lobbies);
    }

    private void LobbyChatUpdate(LobbyChatUpdate_t param)
    {

    }

    private void LobbyEntered(LobbyEnter_t param)
    {
        string lobbyTag = SteamMatchmaking.GetLobbyData((CSteamID)param.m_ulSteamIDLobby, LOBBY_TAG);
        LobbyID = SteamMatchmaking.GetLobbyData((CSteamID)param.m_ulSteamIDLobby, LOBBY_ID);
        ServerID = (CSteamID)param.m_ulSteamIDLobby;

        OnLobbyEntered?.Invoke(ServerID, LobbyID);

        PrintDebug($"Lobby entered: {ServerID} - {LobbyID}");
    }

    private void LobbyCreated(LobbyCreated_t param)
    {
        string lobbyID = SteamHelpers.GetSteamID().ToString();

        SteamMatchmaking.SetLobbyData((CSteamID)param.m_ulSteamIDLobby, LOBBY_TAG, DEFAULT_LOBBY_TAG);
        SteamMatchmaking.SetLobbyData((CSteamID)param.m_ulSteamIDLobby, LOBBY_ID, lobbyID);
        SteamMatchmaking.SetLobbyData((CSteamID)param.m_ulSteamIDLobby, LOBBY_NAME, $"{SteamHelpers.GetPersonaName()}'s Lobby");
        ServerID = (CSteamID)param.m_ulSteamIDLobby;

        OnLobbyCreated?.Invoke(ServerID, lobbyID);

        PrintDebug($"Lobby created: {ServerID} - {lobbyID}");
    }

    private void GameLobbyJoinRequested(GameLobbyJoinRequested_t param)
    {

    }

    private void PrintDebug(string message) { if(debugMessages) Debug.Log(message); }
}
