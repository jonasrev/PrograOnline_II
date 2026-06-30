using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

public class SessionManager : MonoBehaviour
{
    #region Variables

 /*   public */List<SessionInfo> sessionList =
        new List<SessionInfo>();

    #endregion


    #region UI

    [Header("Panel Encontarar Paretida")]
    public GameObject sessionsPanel;

    [Header("Panel Buscar Partida")]
    public GameObject searchPanel;

    [Header("Panel Personalizar Partida")]
    public GameObject PersonalizarParrtidaPanel;

    [Header("Scroll View")]
    public Transform contentParent;
    public GameObject sessionItemPrefab;


    [Header("Input Nombre Sala")]
    public TMP_InputField roomInputField;

    #endregion

    private void Awake()
    {
        PhotonManager.Instance.onSessionListUpdated += OnSessionListUpdated;
    }

    #region Photon Callbacks

    /// <summary>
    /// Actualiza la lista de sesiones
    /// </summary>
    public void OnSessionListUpdated(List<SessionInfo> sessionList)
    {

        Debug.Log($"SessionManager recibió {sessionList.Count} sesiones");

        this.sessionList = sessionList;

        UpdateSessionsListCanvas();

    }

    #endregion


    #region Sessions UI

    /// <summary>
    /// Actualiza las salas visibles
    /// </summary>
    /// 
    public async void OpenSessionsMenu()
    {
        await PhotonManager.Instance.ConnectToPhotonLobby();


        OpenPanelEncontrarPartida();

    }
    public void UpdateSessionsListCanvas()
    {

        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        if (sessionList == null || sessionList.Count == 0)
        {
            Debug.Log("No hay partidas disponibles.");
            return;
        }

        foreach (SessionInfo session in sessionList)
        {
            GameObject sessionGO =
                Instantiate(sessionItemPrefab, contentParent);

            SessionEntry sessionEntry =
                sessionGO.GetComponent<SessionEntry>();

            if (sessionEntry != null)
            {
                sessionEntry.SetssionInfo(session);
            }
        }
    }

    #endregion


    #region Panel Control

    public void OpenPanelEncontrarPartida()
    {
        sessionsPanel.SetActive(true);
    }

    public void ClosePanelEncontrarPartida()
    {
        sessionsPanel.SetActive(false);
    }

    /// <summary>
    /// Abrir panel
    /// </summary>
    public void OpenSearchPanel()
    {
        searchPanel.SetActive(true);
    }

    /// <summary>
    /// Cerrar panel
    /// </summary>
    public void CloseSearchPanel()
    {
        searchPanel.SetActive(false);
    }


    public void OpenPanelPartidaPersonal()
    {
        PersonalizarParrtidaPanel.SetActive(true);
    }

    public void ClosePanelPartidaPersonal()
    {
        PersonalizarParrtidaPanel.SetActive(false);
    }
    #endregion







    // hacer funcionar los botones panel buscarPartida, animooo tu puedes bro


}
