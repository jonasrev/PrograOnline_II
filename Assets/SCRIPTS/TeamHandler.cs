using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Linq;
using UnityEngine;

public enum Team { Red, Blue }

public class TeamHandler : NetworkBehaviour
{
    [Networked] public Team team { get; set; }
    [Networked] public float timeInZone {  get; set; }

    private SpawnPointManager spawnPointManager;
    private SimpleKCC simpleKCC;

    public override void Spawned()
    {
        spawnPointManager = FindAnyObjectByType<SpawnPointManager>();
        simpleKCC = GetComponent<SimpleKCC>();

        if (Runner.IsServer)
        {
            team = AssignTeam();
        }

        MoveToSpawn();
    }


    public void MoveToSpawn()
    {
        simpleKCC.SetPosition(spawnPointManager.SetSpawn(team));
    }

    private Team AssignTeam()
    {
        //return Runner.ActivePlayers.Count() % 2 == 0 ? Team.Red : Team.Blue;

        var players = FindObjectsOfType<TeamHandler>();

        int red = players.Count(p => p.team == Team.Red);
        int blue = players.Count(p => p.team == Team.Blue);

        return red <= blue ? Team.Red : Team.Blue;
    }
}
