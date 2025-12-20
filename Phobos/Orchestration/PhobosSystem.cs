using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EFT;
using Phobos.Components;
using Phobos.Components.Squad;
using Phobos.Data;
using Phobos.Diag;
using Phobos.Entities;
using Phobos.Navigation;
using Phobos.Tasks;
using Phobos.Tasks.Strategies;

namespace Phobos.Orchestration;

public class PhobosSystem
{
    public delegate void RegisterComponentsDelegate(AgentData agentData, SquadData squadData);
    public delegate void RegisterStrategiesDelegate(List<Task<Squad>> strategies);
    
    public static RegisterComponentsDelegate OnRegisterComponents;
    public static RegisterStrategiesDelegate OnRegisterStrategies;

    public SquadRegistry SquadRegistry
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _squadRegistry;
    }
    
    private readonly AgentData _agentData;
    private readonly SquadData _squadData;
    
    private readonly ActionSystem _actionSystem;
    private readonly StrategySystem _strategySystem;
    private readonly SquadRegistry _squadRegistry;
    
    private readonly Telemetry _telemetry;

    public PhobosSystem(Telemetry telemetry)
    {
        _agentData = new AgentData();
        _squadData = new SquadData();
        
        RegisterComponents();
        var strategies = RegisterStrategies();
        
        _actionSystem = new ActionSystem(_agentData);
        _strategySystem = new StrategySystem(_squadData, strategies);
        
        _squadRegistry =  new SquadRegistry(_squadData, _strategySystem, telemetry);
        
        _telemetry = telemetry;
    }

    public void RegisterComponents()
    {
        // Register components with the datasets
        _agentData.RegisterComponent(id => new Objective(id));
        _squadData.RegisterComponent(id => new SquadObjective(id));

        OnRegisterComponents?.Invoke(_agentData, _squadData);
    }

    public Task<Squad>[] RegisterStrategies()
    {
        List<Task<Squad>> strategies = [
            new GotoObjectiveStrategy(_squadData, _agentData, new LocationQueue(), 0.25f)
        ];
        
        OnRegisterStrategies?.Invoke(strategies);
        
        return strategies.ToArray();
    }
    
    public Agent AddAgent(BotOwner bot)
    {
        var agent = _agentData.AddEntity(bot, _actionSystem.TaskCount);
        _squadRegistry.AddAgent(agent);
        DebugLog.Write($"Adding {agent} to Phobos");
        _telemetry.AddEntity(agent);
        return agent;
    }

    public void RemoveAgent(Agent agent)
    {
        DebugLog.Write($"Removing {agent} from Phobos");
        
        _agentData.RemoveEntity(agent);
        _squadRegistry.RemoveAgent(agent);
        
        _actionSystem.RemoveEntity(agent);
        _telemetry.RemoveEntity(agent);
    }

    public void Update()
    {
        _actionSystem.Update();
        _strategySystem.Update();
    }
}