using System;
using UnityEngine;

namespace Phobos.Objectives;

public enum LocationCategory
{
    ContainerLoot,
    LooseLoot,
    Quest,
    Exfil
}

public class Objective(LocationCategory category, string name, Vector3 position) : IEquatable<Objective>
{
    public readonly LocationCategory Category = category;
    public readonly string Name = name;
    public readonly Vector3 Position = position;

    public override string ToString()
    {
        return $"Objective({Category}, {Name}, {Position})";
    }

    public bool Equals(Objective other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Category == other.Category && Name == other.Name && Position.Equals(other.Position);
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Objective)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)Category, Name, Position);
    }
}