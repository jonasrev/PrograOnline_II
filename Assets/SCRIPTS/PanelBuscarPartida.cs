using TMPro;
using UnityEngine;

public class PanelBuscarPartida : MonoBehaviour
{
    public TMP_InputField nombreServer;

    public void joinGame()
    {
        PhotonManager.Instance.JointLobby(nombreServer.ToString());
    }
}
