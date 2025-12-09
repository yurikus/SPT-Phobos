using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Phobos.ECS.Entities;

namespace Phobos.ECS.Systems.Objectives;

public abstract class BaseObjectiveSystem
{
    protected readonly ActorList Actors = new(16);
    private readonly HashSet<Agent> _actorSet = [];
    
    public abstract void SuspendObjective(Agent agent);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void AddActor(Agent agent)
    {
        if (!_actorSet.Add(agent))
            return;

        Actors.Add(agent);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void RemoveActor(Agent agent)
    {
        if (!_actorSet.Remove(agent))
            return;
        
        Actors.SwapRemove(agent);
    }
}