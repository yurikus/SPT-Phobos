using Phobos.Navigation;

namespace Phobos.ECS.Components.Objectives;

public class Guard() : Objective(ObjectiveType.Guard)
{
    public Location Location;
    
    public override string ToString()
    {
        return $"{nameof(Guard)}({Location})";
    }
}