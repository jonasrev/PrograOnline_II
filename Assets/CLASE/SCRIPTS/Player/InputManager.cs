using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    private static InputManager _instance = null;
    
    public static InputManager Instance { get => _instance; private set => _instance = value;}
    
    
    private PlayerControls playerControls;

    public bool login;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        playerControls = new PlayerControls();

        Debug.Log($"InputManager creado: {GetInstanceID()}");
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }
    
    private void OnDisable()
    {
        playerControls.Disable();
    }
    
    public Vector2 GetMoveInput()
    {
        return playerControls.Player.Move.ReadValue<Vector2>();
    }
    
    public bool IsMoveInputPressed()
    {
        return playerControls.Player.Move.IsPressed();
    }
    
    public bool WasRunInputPressed()
    {
        return playerControls.Player.Run.IsPressed();
    }
    
    public bool IsMovingBackwards()
    {
        return playerControls.Player.Move.ReadValue<Vector2>().y < 0;
    }
    
    public bool IsMovingOnXAxis()
    {
        return playerControls.Player.Move.ReadValue<Vector2>().x != 0;
    }
    
    public Vector2 GetMouseDelta()
    {
        return playerControls.Player.Look.ReadValue<Vector2>();
    }
 
    public bool IsMainFirePressed()
    {
        return playerControls.Player.Fire.IsPressed();
    }

    public bool IsReloadPressed()
    {
        return playerControls.Player.Reload.IsPressed();
    }
    
}
