using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DatosJugador
{
    public PlayerRef player;
    public string nombre;
    public int puntos;
    public int kills;
    public Team team;
}

public class Estadisticas : NetworkBehaviour
{
    [Networked]
    public int killsEnPartida { get; set; }

    [Networked]
    public int puntosRealizadosEnPartida { get; set; }

    [Networked]
    public int VictoriasEnPartida { get; set; }

    [Networked, Capacity(32)]
    public NetworkString<_32> nombreJugador { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public async void subirEstadisticas()
    {
        if (!Object.HasInputAuthority)
            return;

        PlayFabMnaguer playFab = FindAnyObjectByType<PlayFabMnaguer>();

        var result = await playFab.DownloadPlayerDataTask();

        int killsTotales = 0;
        int puntosTotales = 0;
        int victoriasTotales = 0;


        killsTotales += killsEnPartida;
        puntosTotales += puntosRealizadosEnPartida;
        victoriasTotales += VictoriasEnPartida;

        var datos = new Dictionary<string, string>
        {
            { "Kills", killsTotales.ToString() },
            { "Puntos Anotados", puntosTotales.ToString() },
            { "Victorias", victoriasTotales.ToString() }
        };

        await playFab.UploadPlayerData(datos);

    }

    public void ResetStatsPartida()
    {
        killsEnPartida = 0;
        puntosRealizadosEnPartida = 0;
        VictoriasEnPartida = 0;
    }

    private TMP_Text textoKills;
    private TMP_Text textoPuntos;

    public override void Spawned()
    {
        if (!Object.HasInputAuthority)
            return;

        GameObject estadisticas = GameObject.Find("Estadisticas");

        if (estadisticas == null)
        {
            Debug.LogError("No se encontró el objeto 'Estadisticas'.");
            return;
        }

        textoKills = estadisticas.transform.Find("KillsTexto")?.GetComponent<TMP_Text>();
        textoPuntos = estadisticas.transform.Find("PuntosTexto")?.GetComponent<TMP_Text>();

        if (textoKills == null)
            Debug.LogError("No se encontró 'KillsTexto'.");

        if (textoPuntos == null)
            Debug.LogError("No se encontró 'PuntosTexto'.");
    }
    private void Update()
    {
        if (!Object.HasInputAuthority)
            return;

        if (textoKills != null)
            textoKills.text = $"Kills: {killsEnPartida}";

        if (textoPuntos != null)
            textoPuntos.text = $"Puntos: {puntosRealizadosEnPartida}";
    }

    public void EnviarDatosAlPanel()
    {
        if (!Object.HasStateAuthority)
            return;

        EstadisticasManager manager = FindAnyObjectByType<EstadisticasManager>();

        if (manager == null)
            return;
        TeamHandler teamHandler = GetComponent<TeamHandler>();
        PlayFabMnaguer playFabMnaguer = FindAnyObjectByType<PlayFabMnaguer>();

        Debug.Log($"Object: {Object}");
        Debug.Log($"Manager: {manager}");
        Debug.Log($"TeamHandler: {teamHandler}");
        Debug.Log($"PlayFabManager: {playFabMnaguer}");

        DatosJugador datos = new DatosJugador()
        {
            player = Object.InputAuthority,
            nombre = playFabMnaguer.userName,
            puntos = puntosRealizadosEnPartida,
            kills = killsEnPartida,
            team = teamHandler.team
        };

        manager.RecibirDatosJugador(datos);
    }



}
