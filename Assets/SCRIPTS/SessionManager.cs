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

    [Header("Panel iniciar sesion")]
    public GameObject iniciarSesionPanel;

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

    private void Start()
    {
        OpenPanelIniciarSesion();
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

        GameObject uiStats = FindAnyObjectByType<EstadsUIInicial>().gameObject;

        uiStats.gameObject.SetActive(false);
    }

    public void ClosePanelEncontrarPartida()
    {
        sessionsPanel.SetActive(false);

        EstadsUIInicial uiStats = FindAnyObjectByType<EstadsUIInicial>(FindObjectsInactive.Include);

        uiStats.gameObject.SetActive(true);
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

    public void OpenPanelIniciarSesion()
    {
        if (InputManager.Instance.login == false)
        {
            iniciarSesionPanel.SetActive(true);
        }

        
    }

    public void ClosePanelIniciarSesion()
    {
        iniciarSesionPanel.SetActive(false);
    }
    #endregion







    // hacer funcionar los botones panel buscarPartida, animooo tu puedes bro

    //Puntos Anotados: por el tiempo que este en las zona de captura ira sumando puntos
    //Kills: Numero de asesinatos que a realizados
    //Victorias: Total de victorias obtenidas


}
