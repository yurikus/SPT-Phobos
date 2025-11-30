using UnityEngine;

namespace Phobos.Objectives;

public enum LocationCategory
{
    ContainerLoot,
    LooseLoot,
    Quest,
    Exfil
}

public class Location(string name, LocationCategory category, Vector3 position)
{
    public readonly string Name = name;
    public readonly LocationCategory Category = category;
    public readonly Vector3 Position = position;

    public override string ToString()
    {
        return $"Location(name: {Name}, category: {Category})";
    }
}