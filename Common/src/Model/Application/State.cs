using System.Collections.Generic;
using ForkCommon.Model.Entity.Pocos;

namespace ForkCommon.Model.Application;

/// <summary>
///     The class that is filled with all state data of the application to be passed to the frontend
/// </summary>
public class State
{
    public State(List<IEntity> entities)
    {
        Entities = entities;
    }

    public List<IEntity> Entities { get; set; }
}