using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

public class CaptureZone : NetworkBehaviour
{
    public float winTime;


    [Networked] public bool IsGameFinished { get; set; }
    [Networked] public byte TeamBlueInZone { get; set; }
    [Networked] public byte TeamRedInZone { get; set; }
    [Networked] public Team WinningTeam { get; set; }
    [Networked] public float TeamBlueTime { get; set; }
    [Networked] public float TeamRedTime { get; set; }

    
    
    //[SerializeField]private UnityEvent<Team> OnWin;

    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TMP_Text victoryText;


    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer) return;
        if (IsGameFinished) return;

            CheckWin();

        timer += Runner.DeltaTime;

        if (timer >= pointTime)
        {
            timer = 0;

            foreach (TeamHandler player in playersInZone)
            {
                player.GetComponent<Estadisticas>().puntosRealizadosEnPartida++;
            }
        }

    }

    public void CheckWin()
    {
        if(TeamBlueInZone > 0 && TeamRedInZone > 0)
        {
            return;
        }

        if (TeamBlueInZone > 0)
        {
            TeamBlueTime += Runner.DeltaTime;
            if (TeamBlueTime >= winTime)
            {
                DeclareWinner(Team.Blue);
            }

        }

        if (TeamRedInZone > 0)
        {
            TeamRedTime += Runner.DeltaTime;

            if(TeamRedTime >= winTime)
            {
                DeclareWinner(Team.Red);
            }
        }
    }


    private void DeclareWinner(Team winningTeam)
    {
        if (IsGameFinished) return;

        IsGameFinished = true;
        WinningTeam = winningTeam;

        TeamHandler[] players = FindObjectsByType<TeamHandler>(FindObjectsSortMode.None);

        foreach (TeamHandler player in players)
        {
            player.OnGameFinished(winningTeam);
            player.GetComponent<Estadisticas>().subirEstadisticas();
        }

        RPC_ShowWin(winningTeam);

        StartCoroutine(RestartGame());
    }

    private IEnumerator RestartGame()
    {
        RPC_HideWin();

        TeamHandler[] players = FindObjectsByType<TeamHandler>(FindObjectsSortMode.None);

        foreach (var player in players)
        {
            Estadisticas estadisticas = player.GetComponent<Estadisticas>();

            if (estadisticas != null)
            {
                estadisticas.EnviarDatosAlPanel();
            }
        }

        EstadisticasManager estadisticasManager = FindAnyObjectByType<EstadisticasManager>();
        estadisticasManager.MostrarPanel();


        // Esperar para que todos vean el mensaje de victoria
        foreach (var player in players)
        {
            player.MoveToSpawn();
            player.GetComponent<Estadisticas>().ResetStatsPartida();
            //player.ResetRound();
        }
        yield return new WaitForSeconds(5f);

        estadisticasManager.OcultarPanel();

        if (!Runner.IsServer)
            yield break;


        IsGameFinished = false;
        WinningTeam = default;

        TeamBlueTime = 0;
        TeamRedTime = 0;

        TeamBlueInZone = 0;
        TeamRedInZone = 0;

        //TeamHandler[] players = FindObjectsByType<TeamHandler>(FindObjectsSortMode.None);

    }

    

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ShowWin(Team winningTeam)
    {
        victoryPanel.SetActive(true);
        victoryText.text = $"¡Ganó el equipo {winningTeam}!";
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_HideWin()
    {
        victoryPanel.SetActive(false);
    }

    private List<TeamHandler> playersInZone = new List<TeamHandler>();

    [SerializeField] private float pointTime = 1f;
    private float timer;
    private void OnTriggerEnter(Collider other)
    {
        if (!Runner.IsServer) return;

        TeamHandler player = other.GetComponent<TeamHandler>();

        if (player != null && player.Object != null)
        {
            if (!playersInZone.Contains(player))
            {
                playersInZone.Add(player);
            }

            if (player.team == Team.Red)
            {
                TeamRedInZone++;
            }

            if (player.team == Team.Blue)
            {
                TeamBlueInZone++;
            }
        }


    }
    

    private void OnTriggerExit(Collider other)
    {
        if (!Runner.IsServer) return;

        TeamHandler player = other.GetComponent<TeamHandler>();

        if (player != null && player.Object != null)
        {
            playersInZone.Remove(player);

            if (player.team == Team.Red)
            {
                TeamRedInZone--;
            }

            if (player.team == Team.Blue)
            {
                TeamBlueInZone--;
            }
        }
    }

    

}
