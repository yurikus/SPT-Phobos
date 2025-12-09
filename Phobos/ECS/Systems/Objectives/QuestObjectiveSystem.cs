using System.Runtime.CompilerServices;
using Phobos.Diag;
using Phobos.ECS.Components;
using Phobos.ECS.Components.Objectives;
using Phobos.ECS.Entities;
using Phobos.Navigation;
using UnityEngine;

namespace Phobos.ECS.Systems.Objectives;

public class QuestObjectiveSystem(MovementSystem movementSystem) : BaseObjectiveSystem
{
    private const float ObjectiveReachedDistSqr = 10f * 10f;
    private const float ObjectiveVicinityDistSqr = 35f * 35f;
    
    public void BeginObjective(Agent agent, Location location)
    {
        var objective = agent.Task.Quest;
        objective.DistanceSqr = (objective.Location.Position - agent.Bot.Position).sqrMagnitude;
        objective.Location = location;
        
        // Short circuit if we are already within the AO
        if (objective.DistanceSqr <= ObjectiveReachedDistSqr)
        {
            objective.Status = ObjectiveStatus.Success;
            return;
        }
        
        objective.Status = ObjectiveStatus.Active;
        agent.Movement.Speed = 1f;
        
        AddActor(agent);
        
        movementSystem.MoveToDestination(agent, location.Position);
        DebugLog.Write($"Assigned {location} to {agent}");
    }

    public override void SuspendObjective(Agent agent)
    {
        RemoveActor(agent);

        var objective = agent.Task.Quest;
        
        if (objective.Status == ObjectiveStatus.Active)
        {
            objective.Status = ObjectiveStatus.Suspended;
        }
    }

    public void Update()
    {
        for (var i = 0; i < Actors.Count; i++)
        {
            var actor = Actors[i];
            UpdateObjective(actor);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void UpdateObjective(Agent agent)
    {
        var bot = agent.Bot;
        var objective = agent.Task.Quest;

        if (agent.Movement.Status == MovementStatus.Failed)
        {
            objective.Status = ObjectiveStatus.Failed;
        }
        
        // Failsafe
        if (objective.Location == null)
        {
            Plugin.Log.LogError($"Null objective for {agent} even though the status is {objective.Status}");
            objective.Status = ObjectiveStatus.Suspended;
            return;
        }
        
        objective.DistanceSqr = (objective.Location.Position - bot.Position).sqrMagnitude;

        if (objective.DistanceSqr <= ObjectiveReachedDistSqr)
        {
            objective.Status = ObjectiveStatus.Success;
        }
        
        var targetSpeed = Mathf.Lerp(0.5f, 1f, Mathf.Pow(objective.DistanceSqr / ObjectiveVicinityDistSqr, 2));
        agent.Movement.Speed = targetSpeed;
    }
}