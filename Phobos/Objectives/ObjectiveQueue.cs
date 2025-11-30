using System.Collections.Generic;
using EFT.Interactive;
using UnityEngine;
using UnityEngine.AI;

namespace Phobos.Objectives;

public class ObjectiveQueue
{
    private readonly Queue<Location> _queue;

    public ObjectiveQueue()
    {
        var objectives = Collect();
        Shuffle(objectives);
        _queue = new Queue<Location>(objectives);
    }

    public Location Next()
    {
        var objective = _queue.Dequeue();
        _queue.Enqueue(objective);
        return objective;
    }
    
    private static void Shuffle(List<Location> objectives)
    {
        // Fisher-Yates in-place shuffle
        for (var i = 0; i < objectives.Count; i++)
        {
            var randomIndex = Random.Range(i, objectives.Count);
            (objectives[i], objectives[randomIndex]) = (objectives[randomIndex], objectives[i]);
        }
    }
    
    private static List<Location> Collect()
    {
        var collection = new List<Location>();

        Plugin.Log.LogInfo("Collecting quests POIs");

        foreach (var trigger in Object.FindObjectsOfType<TriggerWithId>())
        {
            if (trigger.transform == null)
                continue;
            
            AddValid(collection, trigger.name, LocationCategory.Quest, trigger.transform.position);
        }
        
        foreach (var container in Object.FindObjectsOfType<LootableContainer>())
        {
            if (container.transform == null || !container.enabled || container.Template == null)
                continue;
            
            AddValid(collection, container.name, LocationCategory.ContainerLoot, container.transform.position);
        }
        
        return collection;
    }

    private static void AddValid(List<Location> collection, string name, LocationCategory category, Vector3 position)
    {
        if (NavMesh.SamplePosition(position, out var target, 10, NavMesh.AllAreas))
        {
            collection.Add(new Location(name, category, position));
            Plugin.Log.LogInfo($"{category} added as location: {name}");
        }
        else
        {
            Plugin.Log.LogInfo($"{category} too far from navmesh: {name}");
        }
    }
}