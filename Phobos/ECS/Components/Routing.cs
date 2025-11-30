using Phobos.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Phobos.ECS.Components;

public class Routing
{
    public readonly NavPath Path = new();
    public Vector3 Destination;
    public NavMeshPathStatus Status = NavMeshPathStatus.PathInvalid;

    public void Set(NavJob job)
    {
        Path.Set(job);
        Destination = job.Destination;
        Status = job.Status;
    }

    public override string ToString()
    {
        return $"Routing(corners: {Path.Corners.Length}, status: {Status})";
    }
}