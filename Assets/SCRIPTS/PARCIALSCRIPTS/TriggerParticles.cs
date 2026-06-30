using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Threading.Tasks;
using UnityEngine;

public class TriggerParticles : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef particlePrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (!Runner.IsServer)
            return;

        if (other.CompareTag("Player"))
        {
            SimpleKCC playerNetObj = other.GetComponentInParent<SimpleKCC>();


            if (playerNetObj != null)
            {
                NetworkObject particle = Runner.Spawn(particlePrefab);
                NetworkTransform partTr = particle.GetComponent<NetworkTransform>();
                partTr.SyncParent = true;
                particle.transform.SetParent(playerNetObj.Transform);

                particle.transform.localPosition = Vector3.zero;


                Object.gameObject.SetActive(false);
                DespawnParticle(particle);
            }
        }
        
    }

    private async void DespawnParticle(NetworkObject part)
    {
        await Task.Delay((int)(3 * 1000));
        Runner.Despawn(part);
        Runner.Despawn(Object);

    }

}
