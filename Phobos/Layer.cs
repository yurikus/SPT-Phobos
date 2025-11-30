using System;
using System.Text;
using Comfort.Common;
using DrakiaXYZ.BigBrain.Brains;
using EFT;
using Phobos.ECS;
using Phobos.ECS.Systems;
using UnityEngine;

namespace Phobos;

internal class DummyAction(BotOwner botOwner) : CustomLogic(botOwner)
{
    public override void Start()
    {
    }

    public override void Stop()
    {
    }

    public override void Update(CustomLayer.ActionData data)
    {
    }
}

public class PhobosLayer : CustomLayer
{
    public const string LayerName = "PhobosLayer";

    private readonly SystemOrchestrator _systemOrchestrator;
    
    public PhobosLayer(BotOwner botOwner, int priority) : base(botOwner, priority)
    {
        Id = botOwner.Id;
        SquadId = botOwner.BotsGroup.Id;

        _systemOrchestrator = Singleton<SystemOrchestrator>.Instance;
        
        

        // Have to turn this off otherwise bots will be deactivated far away.
        botOwner.StandBy.CanDoStandBy = false;
        
        botOwner.Brain.BaseBrain.OnLayerChangedTo += layer => Plugin.Log.LogInfo("Layer changed to " + layer.Name());
    }
    
    public override string GetName()
    {
        return LayerName;
    }

    public override void Start()
    {
        
    }
    
    public override void Stop()
    {
        // We died/exfilled/etc.
        _systemOrchestrator.RemoveActor(this);
    }


    public override Action GetNextAction()
    {
        return new Action(typeof(DummyAction), "Dummy Action");
    }

    public override bool IsActive()
    {
        // ReSharper disable once InvertIf
        if (_paused)
        {
            if (_pauseTimer - Time.time > 0)
                return false;

            _paused = false;
        }
        
        return true;

        // var isHealing = false;
        //
        // if (BotOwner.Medecine != null)
        // {
        //     isHealing = BotOwner.Medecine.Using;
        //
        //     if (BotOwner.Medecine.FirstAid != null)
        //         isHealing |= BotOwner.Medecine.FirstAid.Have2Do;
        //     if (BotOwner.Medecine.SurgicalKit.HaveWork)
        //         isHealing |= BotOwner.Medecine.SurgicalKit.HaveWork;
        // }
        //
        // var isInCombat = BotOwner.Memory.IsUnderFire || BotOwner.Memory.HaveEnemy || Time.time - BotOwner.Memory.LastEnemyTimeSeen < 30f;
        //
        // if (isHealing || isInCombat)
        //     return false;

        // return Update(objective);
    }
    
    public override bool IsCurrentActionEnding()
    {
        return false;
    }

    // public override void BuildDebugText(StringBuilder sb)
    // {
    //     sb.AppendLine($"Squad {SquadId}, Size: {_squad.Count}");
    //     sb.AppendLine($"Squad Objective: {_squad.Objective?.Category} | {_squad.Objective?.Name}");
    //     sb.AppendLine($"Mover ismoving: {BotOwner.Mover.IsMoving} iscometo: {BotOwner.Mover.IsComeTo(1f, false)}");
    //     sb.AppendLine($"Standby: {BotOwner.StandBy.StandByType} candostandby: {BotOwner.StandBy.CanDoStandBy}");
    //     sb.AppendLine(
    //         $"CurPath: {BotOwner.Mover.ActualPathController.CurPath} progress {BotOwner.Mover.ActualPathController.CurPath?.CurIndex}/{BotOwner.Mover.ActualPathController.CurPath?.Length}"
    //     );
    // }

    public void PauseDuration(float duration)
    {
        _paused = true;
        _pauseTimer = Time.time + duration;
    }

    public void PauseUntil(float timer)
    {
        _paused = true;
        _pauseTimer = timer;
    }

    public void Resume()
    {
        _paused = false;
    }
}