using System.Runtime.CompilerServices;
using UnityEngine;

namespace Phobos.Helpers;

public class TimePacing(float interval)
{
    private float _triggerTime;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Blocked()
    {
        if (Time.time < _triggerTime)
            return true;

        _triggerTime = Time.time + interval;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Allowed()
    {
        if (Time.time < _triggerTime)
            return false;

        _triggerTime = Time.time + interval;
        return true;
    }
}

public class FramePacing(int interval)
{
    private float _triggerCount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Blocked()
    {
        if (Time.time < _triggerCount)
            return true;

        _triggerCount = Time.frameCount + interval;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Allowed()
    {
        if (Time.time < _triggerCount)
            return false;

        _triggerCount = Time.frameCount + interval;
        return true;
    }
}