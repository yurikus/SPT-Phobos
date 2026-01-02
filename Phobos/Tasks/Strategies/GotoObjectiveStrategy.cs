using Phobos.Components;
using Phobos.Components.Squad;
using Phobos.Data;
using Phobos.Diag;
using Phobos.Entities;
using Phobos.Navigation;

namespace Phobos.Tasks.Strategies;

public class GotoObjectiveStrategy(SquadData squadData, LocationQueue locationQueue, float hysteresis) : Task<Squad>(hysteresis)
{
    public override void UpdateScore(int ordinal)
    {
        var squads = squadData.Entities.Values;
        for (var i = 0; i < squads.Count; i++)
        {
            var squad = squads[i];
            squad.TaskScores[ordinal] = 0.5f;
        }
    }

    public override void Update()
    {
        for (var i = 0; i < ActiveEntities.Count; i++)
        {
            var squad = ActiveEntities[i];
        
            if (squad.Objective.Location == null)
            {
                squad.Objective.Location = locationQueue.Next();
                DebugLog.Write($"{squad} assigned objective {squad.Objective.Location}");
            }
        
            for (var j = 0; j < squad.Size; j++)
            {
                var agent = squad.Members[j];
        
                if (squad.Objective.Location == agent.Objective.Location) continue;
        
                DebugLog.Write($"{agent} assigned objective {squad.Objective.Location}");
                
                agent.Objective.Location = squad.Objective.Location;
                agent.Objective.Status = ObjectiveStatus.Suspended;
            }
        }
    }
}