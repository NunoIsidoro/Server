using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    public TMP_InputField playerNameInput;
    public TMP_InputField lobbyCodeInput;
    public TMP_Text lobbyCodeText;
    public GameObject playerListPanel;
    public TMP_Text[] playerListItemPrefab;

    Lobby hostLobby, joinedLobby;

    async void Start()
    {
        await UnityServices.InitializeAsync();
    }

    async Task Authenticate()
    {
        if (AuthenticationService.Instance.IsSignedIn)
            return;

        AuthenticationService.Instance.ClearSessionToken();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log("Authenticated " + AuthenticationService.Instance.PlayerId);
    }

    async public void CreateLobby()
    {
        if (string.IsNullOrEmpty(playerNameInput.text))
        {
            Debug.LogError("Player name is required");
            return;
        }

        await Authenticate();

        var tensAquiOvalor = playerNameInput.text;


        hostLobby = await Lobbies.Instance.CreateLobbyAsync("Lobby", 4);
        Debug.Log("Lobby Created " + hostLobby.Id);
        lobbyCodeText.text = hostLobby.Id;
        UpdatePlayerList(hostLobby.Players);
    }

    async public void JoinLobby()
    {
        if (string.IsNullOrEmpty(playerNameInput.text) || string.IsNullOrEmpty(lobbyCodeInput.text))
        {
            Debug.LogError("Player name and Lobby code are required");
            return;
        }

        await Authenticate();

        // Assuming the lobby code is directly usable to join the lobby
        joinedLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCodeInput.text);
        Debug.Log("Lobby Joined " + joinedLobby.Id);
        UpdatePlayerList(joinedLobby.Players);
    }

    void UpdatePlayerList(IReadOnlyList<Player> players)
    {
        // Clear previous list
        foreach (Transform child in playerListPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Populate new list
        foreach (var player in players)
        {
            //var item = Instantiate(playerListItemPrefab, playerListPanel.transform);
            //item.GetComponentInChildren<Text>().text = player.Id;
        }
    }

    async public void LeaveLobby()
    {
        if (hostLobby != null)
        {
            await Lobbies.Instance.DeleteLobbyAsync(hostLobby.Id);
            hostLobby = null;
        }
        else if (joinedLobby != null)
        {
            await Lobbies.Instance.DeleteLobbyAsync(joinedLobby.Id);
            joinedLobby = null;
        }

        Debug.Log("Left the lobby");
    }

    void Update()
    {
    }
}
