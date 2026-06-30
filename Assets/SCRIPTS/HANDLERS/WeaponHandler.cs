using Fusion;
using System;
using UnityEngine;
using UnityEngine.Windows;
using System.Threading.Tasks;

public class WeaponHandler : NetworkBehaviour
{
    [Networked] private TickTimer shootCooldown { get; set; }
    public Weapons weaponInHand;

    private InputInfo input;

    Action Shoot;

    public void ShootWeapon()
    {
        if (!input.isFirePressed) return;

        
        if (!shootCooldown.ExpiredOrNotRunning(Runner)) return;

        Shoot();

        
        shootCooldown = TickTimer.CreateFromSeconds(Runner, weaponInHand.fireRate);
    }
    private void Start()
    {
        switch (weaponInHand._mode._type)
        {
            case ShootMode.ShootType.Raycast:
                Shoot = weaponInHand.RaycastShoot;
                break;
            case ShootMode.ShootType.Fisico:
                Shoot = weaponInHand.PhysyicalShoot;
                break;
            default:
                Debug.Log("Sin tipo asignado");
                break;
        }
    }


    public void ReloadWeapon()
    {
        if (input.isReloadPressed)
        {
            weaponInHand.Reload();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority && GetInput(out input))
        {
            ShootWeapon();
            ReloadWeapon();
        }
    }


}
