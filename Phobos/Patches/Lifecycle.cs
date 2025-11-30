using System.Reflection;
using Comfort.Common;
using EFT;
using Phobos.ECS;
using Phobos.Navigation;
using Phobos.Objectives;
using SPT.Reflection.Patching;

namespace Phobos.Patches;

public class PhobosInitPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));
    }

    [PatchPrefix]
    // ReSharper disable once InconsistentNaming
    public static void Prefix(GameWorld __instance)
    {
        if (__instance is HideoutGameWorld)
        {
            Plugin.Log.LogInfo("Skipping Phobos in hideout");
            return;
        }

        // Services
        var navJobExecutor = new NavJobExecutor();
        var objectiveQueue = new ObjectiveQueue();
        
        // Systems
        var systemOrchestrator = new SystemOrchestrator(navJobExecutor, objectiveQueue);
        Singleton<SystemOrchestrator>.Create(systemOrchestrator);

        // Updater
        var updater = __instance.gameObject.AddComponent<Updater>();
        updater.SystemOrchestrator = systemOrchestrator;
        updater.NavJobExecutor = navJobExecutor;
    }
}

public class PhobosDisposePatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(GameWorld).GetMethod(nameof(GameWorld.Dispose));
    }

    [PatchPostfix]
    // ReSharper disable once InconsistentNaming
    public static void Prefix()
    {
        Plugin.Log.LogInfo("Disposing of static & long lived objects.");
        Singleton<SystemOrchestrator>.Release(Singleton<SystemOrchestrator>.Instance);
        Plugin.Log.LogInfo("Disposing complete.");
    }
}

