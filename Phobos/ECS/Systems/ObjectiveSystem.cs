using System.Collections.Generic;
using Phobos.Diag;
using Phobos.ECS.Entities;
using Phobos.Objectives;

namespace Phobos.ECS.Systems;

public class ObjectiveSystem(MovementSystem movementSystem) : BaseActorSystem
{
    public void AssignObjective(List<Actor> actors, Location objective)
    {
        for (var i = 0; i < actors.Count; i++)
        {
            var actor = actors[i];
            AssignObjective(actor, objective);
        }
    }

    private void AssignObjective(Actor actor, Location objective)
    {
        actor.Objective.Assign(objective);
        movementSystem.MoveTo(actor, objective.Position);
        DebugLog.Write($"Assigned {objective} to {actor}");
    }

    public void Update()
    {
        // TODO:
        // Track the objective for each actor, update it when finished.
        // Flag it as failed if it doesn't work for whatever reason.
    }
    
    // TODO: The above stuff doesn't yet work for bots who spawned in after the squad is created.
    //       Figure out a neat way of assigning the current squad objective to these bots.
}