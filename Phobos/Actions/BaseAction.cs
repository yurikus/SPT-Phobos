using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Phobos.Data;
using Phobos.Entities;
using Phobos.Helpers;

namespace Phobos.Actions;

public abstract class BaseAction(Dataset dataset, float hysteresis)
{
    public readonly float Hysteresis = hysteresis;
    
    protected readonly List<Agent> ActiveAgents = new(16);
    private readonly HashSet<Agent> _agentSet = [];

    public abstract void UpdateUtility();
    public abstract void Update();

    public virtual void Activate(Agent agent)
    {
        if (!_agentSet.Add(agent))
            return;

        ActiveAgents.Add(agent);
    }

    public virtual void Deactivate(Agent agent)
    {
        if (!_agentSet.Remove(agent))
            return;

        ActiveAgents.SwapRemove(agent);
    }
}