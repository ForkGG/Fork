using System.Threading.Tasks;
using ForkCommon.Model.Application;

namespace Fork.Logic.Managers;

public interface IApplicationManager
{
    public string AppPath { get; }
    public AppSettings AppSettings { get; }
    public string EntityPath { get; }
    public string UserAgent { get; }
}