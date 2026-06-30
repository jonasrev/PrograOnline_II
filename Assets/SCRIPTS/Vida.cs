using Fusion;
using Fusion.Addons.SimpleKCC;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Vida : NetworkBehaviour
{
    [SerializeField] private byte currentHealth;
    [Networked]private byte _currentHealth { get; set;}
    [SerializeField]private byte maxhealth;
    [SerializeField]private Image healthBar;

    [SerializeField] private UnityEvent OnDeath;

    private SimpleKCC simpleKCC;

    [Networked] private TickTimer HealDelay { get; set; }
    [Networked] private TickTimer HealTick { get; set; }

    
    [SerializeField] private Image headHealthBar;
    [SerializeField] private GameObject headHB;
    private Camera cam;

    public override void Spawned()
    {

        if (Object.HasStateAuthority)
        {
            _currentHealth = maxhealth;
        }
       
        if (Object.HasInputAuthority)
        {
            ProgressBars pB = FindAnyObjectByType<ProgressBars>();
            healthBar = pB.healthBar;
        }

        cam = Camera.main ?? FindAnyObjectByType<Camera>();

        
        if (!Object.HasInputAuthority && headHB != null)
        {
            headHB.SetActive(true);
        }
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TakeDamage(byte damage, PlayerRef shooter)
    {
        _currentHealth -= damage;

        HealDelay = TickTimer.CreateFromSeconds(Runner, 2);
        HealTick = TickTimer.None;

        //healthBar.fillAmount = (float)currentHealth / maxhealth;

        if (_currentHealth <= 0 || _currentHealth > maxhealth)
        {
            //Invoke del Metodo MoveToSpawn
            OnDeath?.Invoke();
            _currentHealth = maxhealth;
            
        }
    }
    private void Update()
    {
        if (Object.HasInputAuthority)
        {
            float value = (float)_currentHealth / maxhealth;
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, value, Time.deltaTime * 10f);
        }
    }

    private void LateUpdate()
    {
        if (headHB.activeSelf)
        {
            headHB.transform.LookAt(cam.transform);
            headHB.transform.Rotate(0, 180, 0);

            float value = (float)_currentHealth / maxhealth;
            headHealthBar.fillAmount = value;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;
        if (HealDelay.IsRunning && HealDelay.Expired(Runner))
        {
            
            HealTick = TickTimer.CreateFromSeconds(Runner, .1f);
            HealDelay = TickTimer.None;
        }

        
        if (HealTick.IsRunning && HealTick.Expired(Runner))
        {
            _currentHealth = (byte)Mathf.Min(_currentHealth + 1, maxhealth);

            // Sigue regenerando hasta llenar
            if (_currentHealth < maxhealth)
            {
                HealTick = TickTimer.CreateFromSeconds(Runner, .1f);
            }
            else
            {
                HealTick = TickTimer.None;
            }
        }
    }


}
