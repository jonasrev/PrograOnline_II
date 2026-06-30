using Fusion;
using TMPro;
using UnityEngine;

public class SessionEntry : MonoBehaviour
{
    [SerializeField] TMP_Text serverNameLB1;
    [SerializeField] TMP_Text gameModeLB1;
    [SerializeField] TMP_Text mapNameLB1;
    [SerializeField] TMP_Text playerCountLB1;



    private string serverName;
    private string gameMode;
    private string mapName;
    private int playerInGame;
    private int maxPlayer;

    public void SetssionInfo(SessionInfo sessionInfo)
    {
        this.serverName = sessionInfo.Name;
        this.gameMode = sessionInfo.Properties["GameMode"];
        this.mapName = sessionInfo.Properties["Map"];
        this.playerInGame = sessionInfo.PlayerCount;
        this.maxPlayer = sessionInfo.MaxPlayers;

        serverNameLB1.text = serverName;
        gameModeLB1.text = gameMode;
        mapNameLB1.text = mapName;
        playerCountLB1.text = $"{playerInGame}/{maxPlayer}";
    }

    public void joinGame()
    {
        PhotonManager.Instance.JointLobby(serverName);
    }
}
