using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(GroundCheck))]
public class MovementController : NetworkBehaviour
{
    private InputManager inputManager;
    private Rigidbody rbPlayer;

    [SerializeField] private Animator _animator;

    private InputInfo input;

    private SimpleKCC simpleKCC;

    public Vector2 vector;
    public Vector3 kinematicVel;

    public override void Spawned()
    {
        inputManager = InputManager.Instance;
        simpleKCC = GetComponent<SimpleKCC>();

        //simpleKCC.SetGravity(-9.8f);
    }


    public override void FixedUpdateNetwork()
    {

        if (GetInput(out input ) && HasStateAuthority)
        {
            Movement();
            vector = input.playerPosition;

        }



    }

    public override void Render()
    {
        Animaciones();
    }
    private void Animaciones()
    {
        _animator.SetBool("IsWalking", input.isMoving);
        _animator.SetBool("IsRunning", input.isRunInputPressed);
        _animator.SetFloat("WalkingZ", input.playerPosition.y);
        _animator.SetFloat("WalkingX", input.playerPosition.x);
    }

    #region Movimiento

    [SerializeField] private float walkSpeed = 5.5f;
    [SerializeField] private float runSpeed = 7.7f;
    [SerializeField] private float crouchSpeed = 3.9f;

    private void Movement()
    {
        
        kinematicVel = transform.localRotation * new Vector3(input.playerPosition.x, 0, input.playerPosition.y) * (Runner.DeltaTime * Speed());
        simpleKCC.Move(kinematicVel);
    }

    private float Speed()
    {
        return (input.isMovingBackwards || input.isMovingOnXAxis ? walkSpeed :
            input.isRunInputPressed ? runSpeed : walkSpeed) * 100f;
    }


    #endregion


}