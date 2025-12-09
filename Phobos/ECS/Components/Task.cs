using Phobos.ECS.Components.Objectives;

namespace Phobos.ECS.Components;

public class Task
{
    public readonly Quest Quest = new();
    public readonly Assist Assist = new();
    public readonly Guard Guard = new();

    public readonly Objective[] Objectives;

    public Task()
    {
        Objectives =
        [
            Quest,
            Assist,
            Guard,
        ];
    }

    public Objective Current;

    public override string ToString()
    {
        return $"Task(Current: {Current} Quest: {Quest})";
    }
}