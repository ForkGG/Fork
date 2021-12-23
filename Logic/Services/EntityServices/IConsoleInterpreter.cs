using System.Threading.Tasks;
using ProjectAveryCommon.Model.Entity.Pocos;

namespace ProjectAvery.Logic.Services.EntityServices;

public interface IConsoleInterpreter
{
    public Task InterpretLine(IEntity entity, string line);
}