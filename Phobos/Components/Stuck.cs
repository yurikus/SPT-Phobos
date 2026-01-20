using System.Runtime.CompilerServices;
using Phobos.Helpers;
using UnityEngine;

namespace Phobos.Components;

public enum SoftStuckStatus
{
    None,
    Vaulting,
    Jumping,
    Failed
}

public enum HardStuckStatus
{
    None,
    Retrying,
    Teleport,
    Failed
}

public class HardStuck
{
    public readonly PositionHistory PositionHistory = new(50);
    public readonly RollingAverage AverageSpeed = new(50);
    
    public HardStuckStatus Status = HardStuckStatus.None;
    public float LastUpdate; 
    public float Timer;

    public override string ToString()
    {
        var moveDist = Mathf.Sqrt(PositionHistory.GetDistanceSqr());
        return $"HardStuck(status: {Status}, timer: {Timer}, avgSpeed: {AverageSpeed.Value} moveDist: {moveDist})";
    }
}

public class SoftStuck
{
    public Vector3 LastPosition;
    public float LastSpeed;
    
    public SoftStuckStatus Status = SoftStuckStatus.None;
    public float LastUpdate; 
    public float Timer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset()
    {
        Status = SoftStuckStatus.None;
        Timer = 0f;
    }
    
    public override string ToString()
    {
        return $"SoftStuck(status: {Status}, timer: {Timer}, lastSpeed: {LastSpeed}";
    }
}

public class Stuck
{
    public readonly TimePacing Pacing = new(0.1f);
    
    public HardStuck Hard = new();
    public SoftStuck Soft = new();
    
    public override string ToString()
    {
        return $"Stuck(soft: {Soft} hard: {Hard})";
    }
}