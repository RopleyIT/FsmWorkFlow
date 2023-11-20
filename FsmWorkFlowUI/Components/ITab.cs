using Microsoft.AspNetCore.Components;

namespace FsmWorkFlowUI.Components;

public interface ITab
{
    RenderFragment? ChildContent { get; }
}
