using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;

//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class PhotonManager : MonoBehaviour, INetworkRunnerCallbacks
{


    [SerializeField] private NetworkRunner runner;
    [SerializeField] NetworkSceneManagerDefault sceneManaguer;


    
    [SerializeField] private Transform[] spawnPoint;


    [SerializeField] private UnityEvent Joined;
    

    [SerializeField] Dictionary <PlayerRef,NetworkObject> players = new Dictionary<PlayerRef, NetworkObject>();

    [SerializeField] private NetworkObject playerPrefab;

    [SerializeField] private TextMeshProUGUI mensajeTMP;
    private float tiempoVisible = 3f;


    public event Action <List<SessionInfo>> onSessionListUpdated;

    public static PhotonManager Instance;

    private void Awake()
    {
        Instance = this;

        runner = FindAnyObjectByType<NetworkRunner>();

        Debug.Log($"Runner encontrado: {runner != null}");

        if (runner != null)
        {
            Debug.Log($"Runner IsRunning: {runner.IsRunning}");
        }

        runner.AddCallbacks(this);
    }

    #region metodos photon
    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        InputInfo info = new InputInfo()
        {
            playerPosition = InputManager.Instance.GetMoveInput(),
            lookDirection = InputManager.Instance.GetMouseDelta(),
            isMoving = InputManager.Instance.IsMoveInputPressed(),
            isRunInputPressed = InputManager.Instance.WasRunInputPressed(),
            isMovingBackwards = InputManager.Instance.IsMovingBackwards(),
            isMovingOnXAxis = InputManager.Instance.IsMovingOnXAxis(),
            isFirePressed = InputManager.Instance.IsMainFirePressed(),
            isReloadPressed = InputManager.Instance.IsReloadPressed()
        };
        input.Set(info);
        
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {


        if (runner.LocalPlayer != null)
        {
            Joined?.Invoke();
        }
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    #endregion metodos photon

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {

        //Debug.Log($"playerPrefab es null: {playerPrefab == null}");

        if (runner.IsServer)
        {
            var playerObject = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
            //Cursor.lockState = CursorLockMode.Locked;
            runner.SetPlayerObject(player, playerObject);

            players.Add(player, playerObject);
        }

    }



    

    /// <Summary>
    /// Nos esta proporcionando informacion de lo incios de secion en automatico en todo momento.
    /// 
    /// 1.- click buscar partida o ver lista de servidores
    /// 
    /// 2.- si das en las en buscar servcidores aparece lista de servidores
    /// 
    ///  3.- boton refresh
    /// </Summary>

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        onSessionListUpdated?.Invoke(sessionList);

    }

    public async Task ConnectToPhotonLobby()
    {
        var result = await runner.JoinSessionLobby(SessionLobby.ClientServer);

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    /// <Summary>
    /// puede recibir parametros a la hora de buscar y asi
    /// </Summary>

    public async void JointLobby(string sesionName)
    {
        var sessionManager = FindFirstObjectByType<SessionManager>();

        //SessionInfo sesion = sessionManager.sessionList.Find(s => s.Name == sesionName);

        //// Verificar si existe
        //if (sesion == null)
        //{
        //    StartCoroutine(MostrarMensajeTemporal("No se encontró la sesión."));
        //    return;
        //}

        //// Verificar si está llena
        //if (sesion.PlayerCount >= sesion.MaxPlayers)
        //{
        //    StartCoroutine(MostrarMensajeTemporal("La sesión está llena."));
        //    return;
        //}
        runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(1);

        var sceneInfo = new NetworkSceneInfo();


        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = sesionName,
            Scene = scene,
            SceneManager = sceneManaguer
        });

        //if (!result.Ok)
        //{
        //    _ = MostrarMensajeTemporal("No se encontró la sesión");
        //}


    }

    private IEnumerator MostrarMensajeTemporal(string mensaje)
    {
        mensajeTMP.text = mensaje;
        mensajeTMP.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        mensajeTMP.gameObject.SetActive(false);
    }

    /// <Summary>
    /// nos crea o busca una partida random existente
    /// </Summary>

    public async void StartRandomGame(GameMode mode)
    {
        runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(1);

        var sceneInfo = new NetworkSceneInfo();


        if(scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        string nombreRandomserver = RandomServerName();

        var sessionProperties = new Dictionary<string, SessionProperty>();

        sessionProperties.Add("GameMode", "streed");
        sessionProperties.Add("Map","newYork");

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = nombreRandomserver,
            Scene = scene,
            //CustomLobbyName = "GLOBAL",
            SceneManager = sceneManaguer,
            SessionProperties = sessionProperties,

        });

    }

    public async void StartCustomGame(string server, int maxPlayer)
    {

        // Validación de parámetros
        if (string.IsNullOrWhiteSpace(server))
        {
            StartCoroutine(MostrarMensajeTemporal("Falta el nombre del servidor"));
            return;
        }

        if (maxPlayer < 2 || maxPlayer > 10)
        {
            StartCoroutine(
                MostrarMensajeTemporal("El número de jugadores debe ser entre 2 y 10")
            );
            return;
        }

        runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(1);

        var sceneInfo = new NetworkSceneInfo();


        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        var sessionProperties = new Dictionary<string, SessionProperty>();

        sessionProperties.Add("GameMode", "streed");
        sessionProperties.Add("Map", "newYork");

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = server,
            Scene = scene,
            //CustomLobbyName = "GLOBAL",
            SceneManager = sceneManaguer,
            SessionProperties = sessionProperties,
            PlayerCount = maxPlayer,
        });

    }

    public void CustomGame(string server, int maxPlayer)
    {

        StartCustomGame(server,  maxPlayer);
    }
    public void CreateGame()
    {
        StartRandomGame(GameMode.Host);
    }



    private int randomSereverNameMaxLength = 5;
    public string RandomServerName()
    {
        string characters = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZabcdefghijklmnñopqrstuvwxyz0123456789";
        string randomName = "";

        for (int i = 0; i < randomSereverNameMaxLength; i++)
        {
            int index = UnityEngine.Random.Range(0, characters.Length);
            randomName += characters[index];
        }

        return randomName;
    }


}
