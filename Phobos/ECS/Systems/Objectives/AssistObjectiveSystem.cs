using Phobos.ECS.Components.Objectives;
using Phobos.ECS.Entities;

namespace Phobos.ECS.Systems.Objectives;

public class AssistObjectiveSystem : BaseObjectiveSystem
{
    public override void SuspendObjective(Agent agent)
    {
        RemoveActor(agent);

        var objective = agent.Task.Assist;
        
        if (objective.Status == ObjectiveStatus.Active)
        {
            objective.Status = ObjectiveStatus.Suspended;
        }
    }
}