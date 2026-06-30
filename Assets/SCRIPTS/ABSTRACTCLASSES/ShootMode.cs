using System;
using UnityEngine;
[Serializable]
public class ShootMode
{
    public ShootType _type;
    public enum ShootType
    {
        Raycast, Fisico
    }

}
