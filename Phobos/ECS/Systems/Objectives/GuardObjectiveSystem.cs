using Phobos.ECS.Components.Objectives;
using Phobos.ECS.Entities;

namespace Phobos.ECS.Systems.Objectives;

public class GuardObjectiveSystem : BaseObjectiveSystem
{
    public override void SuspendObjective(Agent agent)
    {
        RemoveActor(agent);

        var objective = agent.Task.Guard;
        
        if (objective.Status == ObjectiveStatus.Active)
        {
            objective.Status = ObjectiveStatus.Suspended;
        }
    }
}