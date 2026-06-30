using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Threading.Tasks;
using System;
public class PlayFabMnaguer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "1DFF0F";
        }
        if(string.IsNullOrEmpty(PlayFabSettings.DeveloperSecretKey))
        {
            PlayFabSettings.DeveloperSecretKey = "B86ZU4F89IYSA511EOOM9CIG3FZI97849PPGK17TD5W3YCARQD\r\n\r\nHide\r\n";
        }
    }

    // metodo pdonde esta la logica para crear un usuario se llama desde un boton

    public void registerUser()
    {
        RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest() // creamos la solicitud
        {
            Email ="",
            Username ="",
            Password ="",
            RequireBothUsernameAndEmail =true
        };

        PlayFabClientAPI.RegisterPlayFabUser(request,onRegisterUserSuccess,OnPlayFabError);
        // cada solicituda se debe aprobar o mostrar el error
    }

    public void onRegisterUserSuccess(RegisterPlayFabUserResult result) // si se asprueba manda el resultado
    {
        
    }
    public void OnPlayFabError(PlayFabError error) // si no se aprueba se manda el error
    {
        Debug.Log(error);
    }

    //-------------------------------------------------------------------------------

    //
    public async void RegisterUserInPlayFab()
    {
        try
        {
            var registerTask = RegisterUserInPlayFabTask();
            await registerTask;

            //registerTask.Result.Username  variable que guarda todo lo que hay en resultado

            Debug.Log(" se inicio sesion correctamednte");

            //await RegisterUserInPlayFabTask();// espera que se realica la conexion a internet
        }
        catch(Exception error)
        {
            Debug.Log(error.Message);
        }
    }

    public async Task <RegisterPlayFabUserResult> RegisterUserInPlayFabTask()
    {
        // TASK TE ALMACENA MAS DE UNA VARIABLE, EN ESTE CASO GUARDA el resultado y el error

        var taskSource = new TaskCompletionSource<RegisterPlayFabUserResult>();

        RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest()
        {
            Email = "",
            Username = "",
            Password = "",
            RequireBothUsernameAndEmail = true
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, resultCallback => taskSource.SetResult(resultCallback),errorCallback => taskSource.SetException(new Exception(errorCallback.GenerateErrorReport())));

        return await taskSource.Task;
    }


}
