using System;
using System.Collections.Generic;
using Phobos.Helpers;
using Phobos.Navigation;

namespace Phobos.Entities;


public class Squad(int id) : IEquatable<Squad>
{
    public Location Objective;
    public readonly List<Agent> Members = new(6);
    
    private readonly int _id = id;
    
    public int Count => Members.Count;

    public void AddAgent(Agent member)
    {
        Members.Add(member);
    }

    public void RemoveAgent(Agent member)
    {
        Members.SwapRemove(member);
    }
    
    public bool Equals(Squad other)
    {
        if (ReferenceEquals(other, null))
            return false;

        return _id == other._id;
    }
    
    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Squad)obj);
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