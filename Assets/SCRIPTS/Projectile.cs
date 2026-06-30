using Fusion;
using System.Threading.Tasks;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private byte damageP;
    [SerializeField] private float lifeTime;

    private PlayerRef shooter;

    private Rigidbody rb;

    public void SetData(PlayerRef playerRef, byte damage)
    {
        shooter = playerRef;
        damageP = damage;
    }


    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();
        AddForce();
    }



    private void AddForce()
    {
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;

            DespawnBullet();
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Damageable"))
        {
            Runner.Despawn(Object);

        }

    }



    private async void DespawnBullet()
    {
        await Task.Delay((int)(lifeTime * 1000));
        Runner.Despawn(Object);
        
    }
    
}
