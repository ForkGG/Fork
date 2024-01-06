using ForkFrontend.Model.Forms;
using Microsoft.AspNetCore.Components;

namespace ForkFrontend.Shared.Components.Forms;

public interface IValueListInput<T, TU>
{
    [Parameter] 
    public List<ForkFormEntity<T, TU>>? Values { get; set; }
}