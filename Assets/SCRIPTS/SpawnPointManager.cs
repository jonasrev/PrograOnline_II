using Fusion;
using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    [SerializeField] public Transform redSpawn;
    [SerializeField] public Transform blueSpawn;



    public Vector3 SetSpawn(Team colorTeam)
    {
        Vector3 spawner;

        switch (colorTeam)
        {
            case Team.Blue:
                spawner = blueSpawn.position;
                break;
            case Team.Red: 
                spawner = redSpawn.position; 
                break;
            default:
                spawner = Vector3.zero;
                break;

        }
        return spawner;


    }

}
