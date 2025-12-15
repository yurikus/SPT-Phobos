using Phobos.Components;
using Phobos.Data;
using Phobos.Entities;

namespace Phobos.Actions;

public class GuardAction(Dataset dataset) : BaseAction(hysteresis: 0.05f)
{
    private readonly ComponentArray<GuardComponent> _guardComponents = dataset.GetComponentArray<GuardComponent>();

    public override void UpdateUtility()
    {
        var agents = dataset.Agents.Values;
        
        for (var i = 0; i < agents.Count; i++)
        {
            var agent = agents[i];
            agent.UtilityScores.Add(new UtilityScore(0.5f, this));
        }
    }
    
    public override void Update()
    {
        for (var i = 0; i < ActiveAgents.Count; i++)
        {
            var agent = ActiveAgents[i];
            var guard = _guardComponents[agent.Id];
        }
    }
    
    public override void Activate(Agent agent)
    {
        base.Activate(agent);
        
        // DebugLog.Write($"Assigned {objective} to {agent}");
        //
        // var objective = agent.Task.GuardComponent;
        //
        // objective.Location = location;
        //
        // agent.Task.Current = objective;
        // agent.Movement.Speed = 1f;
        //
        // Activate(agent);
    }

    public override void Deactivate(Agent agent)
    {
        base.Deactivate(agent);
    }
}