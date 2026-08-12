using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;
public class PlayFabMnaguer : MonoBehaviour
{
    public string UserName { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private string currentPlayerId;
    void Start()
    {
        if(string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "1DFF0F";
        }
        
    }

    

    private IEnumerator ShowAlert(string message, float duration = 3f)
    {
        textAlert.text = message;
        textAlert.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        textAlert.gameObject.SetActive(false);
    }


    #region Rejistro

    public string email;
    public string userName;
    public string pasword;

    public string teamJugador;
    public string teamJugadorDato;
    public int KillsJugador;
    public int PuntosAnotados;

    

    public TextMeshProUGUI textAlert;

    [ContextMenu("rejistroUsuario")] //boton textual en el inspector
    public async void RegisterUserInPlayFab()
    {
        //Debug.Log("Usuario registrado");
        try
        {
            var registerTask = RegisterUserInPlayFabTask();
            FindAnyObjectByType<EstadsUIInicial>().closeUIEstads();
            FindAnyObjectByType<SessionManager>().ClosePanelIniciarSesion();

            InputManager.Instance.login = true;

            await registerTask;

            //registerTask.Result.Username  variable que guarda todo lo que hay en resultado

            //textAlert.text = " se inicio sesion correctamednte";


            //await RegisterUserInPlayFabTask();// espera que se realice la conexion a internet
        }
        catch (Exception error)
        {
            StartCoroutine(ShowAlert(GetFriendlyErrorMessage(error)));
        }

        
    }
    public async Task <RegisterPlayFabUserResult> RegisterUserInPlayFabTask()
    {
        // TASK TE ALMACENA MAS DE UNA VARIABLE, EN ESTE CASO GUARDA el resultado y el error

        var taskSource = new TaskCompletionSource<RegisterPlayFabUserResult>();

        RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest()
        {
            Email = email,
            Username = userName,
            Password = pasword,
            RequireBothUsernameAndEmail = true
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, resultCallback => taskSource.SetResult(resultCallback),errorCallback => taskSource.SetException(new Exception(errorCallback.GenerateErrorReport())));

        return await taskSource.Task;
    }

    [ContextMenu("Iniciar Sesion")]
    public async void LoginUserInPlayFab()
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            StartCoroutine(ShowAlert("Debes escribir un correo."));
            return;
        }

        if (string.IsNullOrWhiteSpace(pasword))
        {
            StartCoroutine(ShowAlert("Debes escribir una contraseña."));
            return;
        }
        try
        {
            var result = await LoginUserInPlayFabTask();

            Debug.Log("Login correcto");
            Debug.Log("Usuario: " + result.PlayFabId);

            currentPlayerId = result.PlayFabId;

            // Solo si el login fue exitoso
            FindAnyObjectByType<SessionManager>().ClosePanelIniciarSesion();

            InputManager.Instance.login = true;

            // Descargar datos
            DownloadPlayerData();
        }
        catch (Exception error)
        {
            //Debug.LogError(error.Message);

            // El panel permanece abierto
            StartCoroutine(ShowAlert(GetFriendlyErrorMessage(error)));
        }
        
    }
    private string GetFriendlyErrorMessage(Exception error)
    {
        string msg = error.Message.ToLower();

        if (msg.Contains("internet") || msg.Contains("network"))
            return "No hay conexión a Internet.";

        if (msg.Contains("timeout"))
            return "La conexión tardó demasiado. Inténtalo de nuevo.";

        if (msg.Contains("invalid"))
            return "Usuario o contraseña incorrectos.";

        if (msg.Contains("banned"))
            return "Esta cuenta ha sido suspendida.";

        return "No se pudo iniciar sesión. Inténtalo nuevamente.";
    }
    public async Task<LoginResult> LoginUserInPlayFabTask()
    {
        var taskSource = new TaskCompletionSource<LoginResult>();

        LoginWithEmailAddressRequest request = new LoginWithEmailAddressRequest()
        {
            Email = email,
            Password = pasword
        };


        PlayFabClientAPI.LoginWithEmailAddress(
            request,

            result => taskSource.SetResult(result),

            error => taskSource.SetException(
                new Exception(error.GenerateErrorReport()))
        );


        return await taskSource.Task;
    }

    [ContextMenu("Subir Player Data")]
    public async Task UploadPlayerData(Dictionary<string, string> data)
    {
        try
        {
            var request = new ExecuteCloudScriptRequest
            {
                FunctionName = "setUserData",
                FunctionParameter = new
                {
                    data = data
                }
            };

            await ExecuteCloudScriptTask(request);

            Debug.Log("Datos subidos correctamente.");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }



    // Esta función hace la conexión real con PlayFab para subir datos
    public async Task<ExecuteCloudScriptResult> ExecuteCloudScriptTask(ExecuteCloudScriptRequest request)
    {

        // Creamos un objeto que nos permite esperar una respuesta de PlayFab
        var taskSource = new TaskCompletionSource<ExecuteCloudScriptResult>();


        // Llamamos a la API de PlayFab para actualizar los datos del usuario
        PlayFabClientAPI.ExecuteCloudScript(

            // Enviamos la información que queremos guardar
            request,


            // Guarda el resultado dentro del Task
            result => taskSource.SetResult(result),


            // Esta función se ejecuta si PlayFab manda un error
            error => taskSource.SetException(
                new Exception(error.GenerateErrorReport()))
        );


        //el resultado
        return await taskSource.Task;
    }





    // Crea una opción en el Inspector llamada "Bajar Player Data"
    [ContextMenu("Bajar Player Data")]

    //EstadsUIInicial ui;
    public async void DownloadPlayerData()
    {
        try
        {
            var result = await DownloadPlayerDataTask();

            Debug.Log(result.FunctionResult);
            Debug.Log(result.FunctionResult?.GetType());

            var data = (PlayFab.Json.JsonObject)result.FunctionResult;

            Debug.Log(data == null);
            Debug.Log(data["Kills"]);

            EstadsUIInicial ui = FindAnyObjectByType<EstadsUIInicial>();

            ui.kills.text = $"Kills: {data["Kills"]}";
            ui.puntos.text = $"Puntos anotados: {data["Puntos"]}";
            ui.victorias.text = $"Victorias: {data["Victorias"]}";
            ui.userName.text = $"Usuario: {data["Username"]}";
            userName = $"Usuario: {data["Username"]}";



            Debug.Log("Datos descargados");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            EstadsUIInicial ui = FindAnyObjectByType<EstadsUIInicial>();
            ui.closeUIEstads();
        }
    }





    // Esta función hace la solicitud a PlayFab para obtener datos
    public async Task<ExecuteCloudScriptResult> DownloadPlayerDataTask()
    {

        // Creamos un objeto para esperar la respuesta de PlayFab
        var taskSource = new TaskCompletionSource<ExecuteCloudScriptResult>();



        // Llamamos a PlayFab para obtener los datos del jugador actual
        PlayFabClientAPI.ExecuteCloudScript(

            // null significa:
            // "dame todos los datos guardados"
            new ExecuteCloudScriptRequest
            {
                FunctionName = "GetPlayerData"
            },

        result => taskSource.SetResult(result),

        error => taskSource.SetException(
            new Exception(error.GenerateErrorReport()))
        );


        // Esperamos la respuesta y la regresamos
        return await taskSource.Task;
    }
    #endregion Rejistro


}
