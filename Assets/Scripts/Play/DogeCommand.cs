
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
    None,
    Up,
    Down,
    Left,
    Right,
    Shoot
}
