using Phobos.Navigation;
using UnityEngine.AI;

namespace Phobos.ECS.Components;

public class Routing
{
    public readonly NavPath Path = new();
    public NavMeshPathStatus Status = NavMeshPathStatus.PathInvalid;

    public void Set(NavJob job)
    {
        Path.Set(job);
        Status = job.Status;
    }
}