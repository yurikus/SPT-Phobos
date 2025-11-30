using System.Collections.Generic;
using Phobos.ECS.Entities;
using Phobos.Extensions;

namespace Phobos.ECS;

public class BaseActorSystem : IActorSystem
{
    protected readonly List<Actor> Actors = [];
    
    public void AddActor(Actor actor)
    {
        Actors.Add(actor);
    }

    public void RemoveActor(Actor actor)
    {
        Actors.RemoveSwap(actor);
    }
}