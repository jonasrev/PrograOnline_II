using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class PanelCrearPartida : MonoBehaviour
{
    public TMP_InputField nombreServer;
    public TMP_InputField numeroPlayers;

    public void CrearCustomPartida()
    {
        if (int.TryParse(numeroPlayers.text, out int numero))
        {
            Debug.Log("Número válido: " + numero);
        }
        PhotonManager.Instance.CustomGame(nombreServer.text,numero);
    }
}
