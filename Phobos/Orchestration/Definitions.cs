using System.Collections.Generic;
using Phobos.Components;
using Phobos.Components.Squad;
using Phobos.Data;

namespace Phobos.Orchestration;

public class Definitions
{
    public readonly List<IComponentArray> AgentComponents = [new ComponentArray<Objective>(id => new Objective(id))];
    public readonly List<IComponentArray> SquadComponents = [new ComponentArray<SquadObjective>(id => new SquadObjective(id))];
}