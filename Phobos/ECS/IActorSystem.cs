using Phobos.ECS.Entities;

namespace Phobos.ECS;

public interface IActorSystem
{
    public void AddActor(Actor actor);
    public void RemoveActor(Actor actor);
}