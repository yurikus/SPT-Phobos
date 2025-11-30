using System;
using System.Collections.Generic;
using Phobos.ECS.Components;
using Phobos.Extensions;

namespace Phobos.ECS.Entities;

public class Squad(int id) : IEquatable<Squad>
{
    public readonly Objective Objective = new();
    public readonly List<Actor> Members = [];
    
    private readonly int _id = id;
    
    public int Count => Members.Count;

    public void AddMember(Actor member)
    {
        Members.Add(member);
    }

    public void RemoveMember(Actor member)
    {
        Members.RemoveSwap(member);
    }
    
    public bool Equals(Squad other)
    {
        if (ReferenceEquals(other, null))
            return false;

        return _id == other._id;
    }

    public override int GetHashCode()
    {
        return _id;
    }

    public override string ToString()
    {
        return $"Squad(id: {_id})";
    }
}