using System.Collections.Generic;
using Phobos.ECS.Entities;
using Phobos.ECS.Systems;
using Phobos.Navigation;
using Phobos.Objectives;

namespace Phobos.ECS;

public class SystemOrchestrator : IActorSystem
{
    private readonly MovementSystem _movementSystem;
    private readonly ObjectiveSystem _objectiveSystem;
    private readonly SquadOrchestrator _squadOrchestrator;
    private readonly List<IActorSystem> _systems;

    public SystemOrchestrator(NavJobExecutor navJobExecutor, ObjectiveQueue objectiveQueue)
    {
        _movementSystem = new MovementSystem(navJobExecutor);
        _objectiveSystem = new ObjectiveSystem(_movementSystem);
        _squadOrchestrator = new SquadOrchestrator(_objectiveSystem, objectiveQueue);

        _systems = [_movementSystem, _objectiveSystem, _squadOrchestrator];
    }

    public void AddActor(Actor actor)
    {
        for (var i = 0; i < _systems.Count; i++)
        {
            _systems[i].AddActor(actor);
        }
    }

    public void RemoveActor(Actor actor)
    {
        for (var i = 0; i < _systems.Count; i++)
        {
            _systems[i].RemoveActor(actor);
        }
    }

    public void Update()
    {
        _squadOrchestrator.Update();
        _objectiveSystem.Update();
        _movementSystem.Update();
    }
}