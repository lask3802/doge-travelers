
using System;

public struct DogeCommand
{
    public DogeCommandType Type;
    public float ShootingX;
    public float ShootingY;
}

[Flags]
public enum DogeCommandType
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8,
    Shoot = 16
}
