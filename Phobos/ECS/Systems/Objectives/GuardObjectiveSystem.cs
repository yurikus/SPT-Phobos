using Phobos.Diag;
using Phobos.ECS.Entities;
using Phobos.Navigation;

namespace Phobos.ECS.Systems.Objectives;

public class GuardObjectiveSystem : BaseObjectiveSystem
{
    public void BeginObjective(Agent agent, Location location)
    {
        var objective = agent.Task.Guard;
        
        objective.Location = location;
        
        agent.Task.Current = objective;
        agent.Movement.Speed = 1f;
        
        AddAgent(agent);
        DebugLog.Write($"Assigned {objective} to {agent}");
    }

    public override void ResetObjective(Agent agent)
    {
        agent.Task.Guard.Location = null;
    }
}