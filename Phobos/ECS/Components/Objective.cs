using Phobos.Objectives;

namespace Phobos.ECS.Components;

public enum ObjectiveStatus
{
    Missing,
    Active,
    Completed,
    Failed,
}

public enum ObjectiveFailure
{
    None,
    InvalidPath,
    Timeout,
}

public class Objective
{
    public Location Target;
    public ObjectiveStatus Status = ObjectiveStatus.Missing;
    public ObjectiveFailure Failure = ObjectiveFailure.None;
    public bool IsValid => Target != null;

    public void Assign(Location location)
    {
        Target = location;
        Status = ObjectiveStatus.Active;
    }

    public void Reset()
    {
        Target = null;
        Status = ObjectiveStatus.Missing;
    }

    public override string ToString()
    {
        return $"Objective(location: {Target}, status: {Status}, failure: {Failure})";
    }
}