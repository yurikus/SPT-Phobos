using Phobos.Diag;
using Phobos.ECS.Entities;
using Phobos.ECS.Systems;
using Phobos.ECS.Systems.Objectives;
using Phobos.Navigation;

namespace Phobos.ECS;

// ReSharper disable MemberCanBePrivate.Global
// Every AI project needs a shitty, llm generated component name like "orchestrator".
public class SystemOrchestrator
{
    public readonly SquadOrchestrator SquadOrchestrator;
    
    public readonly QuestObjectiveSystem QuestObjectiveSystem;
    public readonly GuardObjectiveSystem GuardObjectiveSystem;
    public readonly AssistObjectiveSystem AssistObjectiveSystem;
    
    public readonly MovementSystem MovementSystem;
    public readonly ActorList LiveActors;

    public SystemOrchestrator(NavJobExecutor navJobExecutor, LocationQueue locationQueue)
    {
        LiveActors = new ActorList(32);
        
        DebugLog.Write("Creating MovementSystem");
        MovementSystem = new MovementSystem(navJobExecutor, LiveActors);
        
        DebugLog.Write("Creating StrategicObjectiveSystem");
        QuestObjectiveSystem = new QuestObjectiveSystem(MovementSystem);
        DebugLog.Write("Creating GuardObjectiveSystem");
        GuardObjectiveSystem = new GuardObjectiveSystem();
        DebugLog.Write("Creating AssistObjectiveSystem");
        AssistObjectiveSystem = new AssistObjectiveSystem();
        
        DebugLog.Write("Creating SquadOrchestrator");
        SquadOrchestrator = new SquadOrchestrator(QuestObjectiveSystem, GuardObjectiveSystem, AssistObjectiveSystem, locationQueue);
    }

    public void AddActor(Agent agent)
    {
        DebugLog.Write($"Adding {agent} to Phobos systems");
        LiveActors.Add(agent);
        SquadOrchestrator.AddActor(agent);
    }

    public void RemoveActor(Agent agent)
    {
        LiveActors.SwapRemove(agent);
        SquadOrchestrator.RemoveActor(agent);
    }

    public void Update()
    {
        SquadOrchestrator.Update();
        QuestObjectiveSystem.Update();
        MovementSystem.Update();
    }
}