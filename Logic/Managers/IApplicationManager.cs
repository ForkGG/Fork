using System.Threading.Tasks;
using ProjectAveryCommon.Model.Application;

namespace ProjectAvery.Logic.Managers;

public interface IApplicationManager
{
    public string AppPath { get; }
    public AppSettings AppSettings { get; }
    public string EntityPath { get; }
    public string UserAgent { get; }
}