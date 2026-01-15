using Phobos.Navigation;

namespace Phobos.Components;


public enum ObjectiveStatus
{
    Active,
    Failed
}


public class Objective
{
    public Location Location;
    public ObjectiveStatus Status;

    public override string ToString()
    {
        return $"Objective({Location}, {Status})";
    }
}