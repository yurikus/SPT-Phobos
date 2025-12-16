using EFT;
using Phobos.Data;
using Phobos.Diag;
using Phobos.Entities;
using Phobos.Navigation;
using Phobos.Orchestration;

namespace Phobos;

public class PhobosSystem
{
    public readonly Dataset Dataset;
    
    public readonly ActionManager ActionManager;
    public readonly SquadManager SquadManager;
    
    private readonly Telemetry _telemetry;

    public PhobosSystem(Telemetry telemetry)
    {
        var objectiveQueue = new LocationQueue();
        
        Dataset = new Dataset();
        ActionManager = new ActionManager(Dataset);
        SquadManager =  new SquadManager(objectiveQueue);
        
        _telemetry = telemetry;
    }

    // TODO: Replace SystemOrchestrator as the main object.
    // Add/remove agents
    // API for registering components and actions, etc...

    public virtual void RegisterComponents()
    {
        // Register components with the Dataset
    }

    public virtual void RegisterActions()
    {
        // Register actions
    }
    
    public Agent AddAgent(BotOwner bot)
    {
        var agent = Dataset.AddAgent(bot);
        DebugLog.Write($"Adding {agent} to Phobos");
        SquadManager.AddAgent(agent);
        _telemetry.AddAgent(agent);
        return agent;
    }

    public void RemoveAgent(Agent agent)
    {
        DebugLog.Write($"Removing {agent} from Phobos");
        Dataset.RemoveAgent(agent);
        ActionManager.RemoveAgent(agent);
        SquadManager.RemoveAgent(agent);
        _telemetry.RemoveAgent(agent);
    }

    public void Update()
    {
        ActionManager.Update();
        SquadManager.Update();
    }
}