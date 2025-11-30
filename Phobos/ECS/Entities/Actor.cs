using System;
using EFT;
using Phobos.ECS.Components;

namespace Phobos.ECS.Entities;

public class Actor(BotOwner bot) : IEquatable<Actor>
{
    public bool Suspended;
    public bool Paused;
    
    public readonly int SquadId = bot.BotsGroup.Id;
    public readonly BotOwner Bot = bot;
    
    public readonly Objective Objective = new();
    public readonly Routing Routing = new();
    
    public bool IsActive => !Suspended && !Paused;
    
    private readonly int _id = bot.Id;

    public bool Equals(Actor other)
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
        return $"Actor(id: {_id}, name: {Bot.Profile.Nickname})";
    }
}