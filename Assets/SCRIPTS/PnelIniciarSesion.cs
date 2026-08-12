using TMPro;
using UnityEngine;

public class PnelIniciarSesion : MonoBehaviour
{
    public TMP_InputField gmail;
    public TMP_InputField userName;
    public TMP_InputField pasword;
    public void Registro()
    {
        
        PlayFabMnaguer playFabMnaguer = FindAnyObjectByType<PlayFabMnaguer>();

        playFabMnaguer.email = gmail.text;
        playFabMnaguer.userName = userName.text;
        playFabMnaguer.pasword = pasword.text;

        playFabMnaguer.RegisterUserInPlayFab();
    }

    public void inicioSesion()
    {
        PlayFabMnaguer playFabMnaguer = FindAnyObjectByType<PlayFabMnaguer>();

        playFabMnaguer.email = gmail.text;
        playFabMnaguer.userName = userName.text;
        playFabMnaguer.pasword = pasword.text;

        playFabMnaguer.LoginUserInPlayFab();
    }
}
