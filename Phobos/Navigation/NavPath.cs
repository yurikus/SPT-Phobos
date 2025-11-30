using UnityEngine;

namespace Phobos.Navigation;

public class NavPath(Vector3[] corners)
{
    public Vector3[] Corners = corners;

    public NavPath() : this([])
    {
    }

    public void Set(NavJob job)
    {
        Corners = job.Corners;
    }
}