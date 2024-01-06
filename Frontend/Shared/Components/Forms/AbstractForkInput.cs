using ForkFrontend.Model.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ForkFrontend.Shared.Components.Forms;

public abstract class AbstractForkInput<T> : InputBase<T>
{
    [Parameter]
    public string? Name { get; set; }

    [Parameter]
    public string? Label { get; set; }
}