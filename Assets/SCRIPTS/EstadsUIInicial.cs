using TMPro;
using UnityEngine;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class EstadsUIInicial : MonoBehaviour
{
    public TextMeshProUGUI userName;
    public TextMeshProUGUI puntos;
    public TextMeshProUGUI kills;
    public TextMeshProUGUI victorias;



    public void closeUIEstads()
    {
        this.gameObject.SetActive(false);
    }

    public void OpenUIEstads()
    {
        gameObject.SetActive(true);
    }
}
