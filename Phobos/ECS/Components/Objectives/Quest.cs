using Phobos.Navigation;
using UnityEngine;

namespace Phobos.ECS.Components.Objectives;

public class Quest() : Objective(ObjectiveType.Quest)
{
    public Location Location;
    public float DistanceSqr;
    
    public override string ToString()
    {
        return $"{nameof(Quest)}({Location} Dist: {Mathf.Sqrt(DistanceSqr)})";
    }
}