
using System;
using UnityEngine;

public struct DogeCommand
{
    public DogeCommandType Type;
    public Vector3 ShootEndPoint;
}

[Flags]
public enum DogeCommandType
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8,
    Shoot = 16,
    Laser = 32,
    LaserBegin = 64,
    LaserEnd = 128,
    MoveBegin = 256,
    MoveEnd = 512
}
