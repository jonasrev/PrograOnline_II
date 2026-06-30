using Fusion;
using Fusion.Addons.SimpleKCC;
using System;
using UnityEngine;
using UnityEngine.Serialization;


public class CameraController : NetworkBehaviour
{
    [Header("Camera Settings")]

    [SerializeField] private float mouseSensitivity = 1;

    [FormerlySerializedAs("smooth")]
    [SerializeField]
    private float smoothnes;

    [SerializeField] private float maxAngleY = 80;
    [SerializeField] private float minAngleY = -80;

    [Networked] private Vector2 camVelociy { get; set; }
    private Vector2 smoothVelocity;

    [Header("Blob Movement")]
    [SerializeField] private float walkingSpeed = 1f;

    [SerializeField, Range(0, 0.1f)] private float walkingAmplitude = 0.015f; // Que tanto se mueve hacia los lados al caminar
    [SerializeField, Range(0, 0.1f)] private float runningAmplitude = 0.015f; // Que tanto se mueve hacia los lados al correr
    [SerializeField, Range(0, 15)] private float walkingFrequency = 10.0f; // La frecuencia con la que se mueve al caminar
    [SerializeField, Range(10, 20)] private float runningFrequency = 18f; // La frecuencia con la que se mueve al correr
    [SerializeField] private float resetPosSpeed = 3.0f; // Cuando dejas de moverte que regrese al centro
    [SerializeField] private float toggleSpeed = 3.0f; // 

    private Vector3 startPos; // Posicion inicial de la cabeza , el centro

    [SerializeField] private bool moveHead;

    private Vector2 head;

    private InputManager inputManager;

    private InputInfo input;

    private SimpleKCC simpleKCC;

    private void Awake()
    {
        startPos = transform.localPosition;
    }

    private void Start()
    {
        inputManager = InputManager.Instance;
        if (simpleKCC == null)
        {
            simpleKCC = GetComponentInParent<SimpleKCC>();
        }



        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;



    }

    public override void Spawned()
    {
        if (!HasInputAuthority)
        {
            GetComponent<Camera>().enabled = false;
            GetComponent<AudioListener>().enabled = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out input))
        {
            RotateCamera();
            //if (!moveHead) return;
            //BlobMove();
            //ResetPosition();
        }
    }

    public override void Render()
    {
        if (!HasInputAuthority && !HasStateAuthority)
        {
            transform.localRotation = Quaternion.AngleAxis(-camVelociy.y, Vector3.right);
        }

    }
    private void RotateCamera()
    {
        Vector2 rawFrameVelocity = Vector2.Scale(input.lookDirection, Vector2.one * mouseSensitivity);
        smoothVelocity = Vector2.Lerp(smoothVelocity, rawFrameVelocity, 1 / smoothnes); // Te mueve desde donde tengas el mouse, a la nueva posicion del mouse
        Vector2 currentVelocity = camVelociy;
        currentVelocity += smoothVelocity;
        currentVelocity.y = Mathf.Clamp(currentVelocity.y, minAngleY, maxAngleY); // Limita la rotacion de la camara en Y. En base el movimiento del mouse.
        camVelociy = currentVelocity;

        transform.localRotation = Quaternion.AngleAxis(-camVelociy.y, Vector3.right);

        if (simpleKCC != null)
        {
            simpleKCC.AddLookRotation(-smoothVelocity.y, smoothVelocity.x);
        }

        //player.localRotation = Quaternion.AngleAxis(camVelociy.x, Vector3.up);
    }

    private void BlobMove()
    {
        if (!input.isMoving) // Si no presiono ningun input
        {
            return; // termina el metodo
        }

        if (input.isMoving) // Pregunto si me estoy moviendo
        {
            if (input.isMovingBackwards || input.isMovingOnXAxis) // Me estoy moviendo hacia atras o hacia los lados?
            {
                transform.localPosition += FootStepMotion();
            }
            else //  Entonces me muevo hacia adelante
            {
                if (input.isRunInputPressed) // Estoy corriendo?
                {
                    transform.localPosition += RunningFootStepMotion();
                }
                else // Estoy caminando
                {
                    transform.localPosition += FootStepMotion();
                }
            }
        }

        if (input.isMoving)
        {
            transform.localPosition += input.isMovingBackwards || input.isMovingOnXAxis ? FootStepMotion() : input.isRunInputPressed ? RunningFootStepMotion() : FootStepMotion();
        }



    }

    private void ResetPosition()
    {
        if (transform.localPosition == startPos) return; // Si la camara ya esta en la pos inicial, no hace nada
        transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, resetPosSpeed * Time.deltaTime);
    }

    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y = Mathf.Sin(Time.time * walkingFrequency) * walkingAmplitude * walkingSpeed;
        pos.x = Mathf.Cos(Time.time * walkingFrequency / 2) * walkingAmplitude * 2 * walkingSpeed;
        return pos;
    }


    private Vector3 RunningFootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y = Mathf.Sin(Time.time * runningFrequency) * runningAmplitude * walkingSpeed;
        pos.x = Mathf.Cos(Time.time * runningFrequency / 2) * runningAmplitude * 2 * walkingSpeed;
        return pos;
    }

}