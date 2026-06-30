using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CaptureZone : NetworkBehaviour
{
    public float winTime;


    [Networked] public bool IsGameFinished { get; set; }
    [Networked] public byte TeamBlueInZone { get; set; }
    [Networked] public byte TeamRedInZone { get; set; }
    [Networked] public Team WinningTeam { get; set; }
    [Networked] public float TeamBlueTime { get; set; }
    [Networked] public float TeamRedTime { get; set; }


    
    [SerializeField]private UnityEvent<Team> OnWin;


    public override void FixedUpdateNetwork()
    {
        if (!Runner.IsServer) return;
        if (IsGameFinished) return;

            CheckWin();

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

        RPC_ShowWin(winningTeam);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ShowWin(Team winningTeam)
    {
        Debug.Log($"¡El equipo {winningTeam} ha ganado!");
        OnWin?.Invoke(winningTeam);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!Runner.IsServer) return;

        TeamHandler player = other.GetComponent<TeamHandler>();
        if (player != null && player.Object != null)
        {
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
