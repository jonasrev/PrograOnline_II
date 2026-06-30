using Fusion;
using UnityEngine;

public struct InputInfo : INetworkInput
{
    public Vector2 playerPosition;
    public Vector2 lookDirection;

    public bool isMoving;
    public bool isRunInputPressed;
    public bool isMovingBackwards;
    public bool isMovingOnXAxis;

    public bool isFirePressed;
    public bool isReloadPressed;

    public Vector3 bulletPosition;

}
