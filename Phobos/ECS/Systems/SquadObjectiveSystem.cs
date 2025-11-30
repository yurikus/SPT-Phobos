using System.Collections.Generic;
using Phobos.Diag;
using Phobos.ECS.Entities;
using Phobos.Objectives;

namespace Phobos.ECS.Systems;

public class SquadObjectiveSystem(ObjectiveSystem objectiveSystem, ObjectiveQueue objectiveQueue)
{
    public void Update(List<Squad> squads)
    {
        for (var i = 0; i < squads.Count; i++)
        {
            var squad = squads[i];
            if (squad.Objective.IsValid)
                continue;

            var objective = objectiveQueue.Next();
            squad.Objective.Assign(objective);
            objectiveSystem.AssignObjective(squad.Members, squad.Objective.Target);
            DebugLog.Write($"Assigned {objective} to {squad}");
        }
    }
}