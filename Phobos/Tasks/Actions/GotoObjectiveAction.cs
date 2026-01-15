using Phobos.Components;
using Phobos.Data;
using Phobos.Diag;
using Phobos.Entities;
using Phobos.Systems;
using UnityEngine;

namespace Phobos.Tasks.Actions;

public class GotoObjectiveAction(AgentData dataset, MovementSystem movementSystem, float hysteresis) : Task<Agent>(hysteresis)
{
    public const float ObjectiveEpsDistSqr = 10f * 10f;
    
    private const float UtilityBase = 0.5f;
    private const float UtilityBoost = 0.15f;
    private const float UtilityBoostMaxDistSqr = 50f * 50f;

    public override void UpdateScore(int ordinal)
    {
        var agents = dataset.Entities.Values;
        for (var i = 0; i < agents.Count; i++)
        {
            var agent = agents[i];
            var objective = agent.Objective;

            if (objective.Status == ObjectiveStatus.Failed || objective.Location == null)
            {
                agent.TaskScores[ordinal] = 0;
                continue;
            }

            // Baseline utility is 0.5f, boosted up to 0.65f as the bot gets nearer the objective. Once within the objective radius, the
            // utility falls off sharply.
            var distSqr = (objective.Location.Position - agent.Position).sqrMagnitude;

            var utilityBoostFactor = Mathf.InverseLerp(UtilityBoostMaxDistSqr, ObjectiveEpsDistSqr, distSqr);
            var utilityDecay = Mathf.InverseLerp(0f, ObjectiveEpsDistSqr, distSqr);

            agent.TaskScores[ordinal] = utilityDecay * (UtilityBase + utilityBoostFactor * UtilityBoost);
        }
    }

    public override void Update()
    {
        for (var i = 0; i < ActiveEntities.Count; i++)
        {
            var agent = ActiveEntities[i];
            var objective = agent.Objective;

            if (agent.Objective.Status == ObjectiveStatus.Failed || objective.Location == null)
            {
                continue;
            }

            if ((agent.Movement.Target - objective.Location.Position).sqrMagnitude <= ObjectiveEpsDistSqr)
            {
                if (agent.Movement.Status == MovementStatus.Failed)
                {
                    objective.Status = ObjectiveStatus.Failed;
                }
                continue;
            }
            
            DebugLog.Write($"{agent} received new objective {agent.Objective.Location}, submitting move order");
            movementSystem.MoveToByPath(agent, objective.Location.Position);
        }
    }

    public override void Activate(Agent entity)
    {
        base.Activate(entity);

        if (entity.Objective.Location == null)
        {
            return;
        }
        
        // Check if we are already moving to our target
        if (entity.Movement.HasPath)
        {
            if ((entity.Movement.Target - entity.Objective.Location.Position).sqrMagnitude <= ObjectiveEpsDistSqr)
            {
                return;
            }
        }

        movementSystem.MoveToByPath(entity, entity.Objective.Location.Position);
    }
}