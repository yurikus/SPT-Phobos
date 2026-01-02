using UnityEngine;
using UnityEngine.AI;

namespace Phobos.Components;

public class Movement
{
    public Vector3 Target;
    public Vector3[] Path;
    public NavMeshPathStatus Status;

    public int CurrentCorner;
    public int Retry;

    public float Speed = 1f;
    public float Pose = 1f;
    public bool Sprint = false;
    public bool Prone = false;

    public override string ToString()
    {
        return
            $"Movement(Path: {CurrentCorner}/{Path?.Length}, Status: {Status} Retry: {Retry} Speed: {Speed}, Pose: {Pose} Sprint {Sprint}, Prone: {Prone})";
    }
}