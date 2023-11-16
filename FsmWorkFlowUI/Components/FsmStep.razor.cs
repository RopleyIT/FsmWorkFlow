using Microsoft.AspNetCore.Components;
using Radzen;
using System.ComponentModel.Design;

namespace FsmWorkFlowUI.Components;

public partial class FsmStep
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The parent workflow (state machine) to which
    /// this step (state) belongs
    /// </summary>

    [CascadingParameter]
    public FsmWorkFlow? FsmWorkFlow { get; set; }

    /// <summary>
    /// The name of this state. Used for resolving
    /// links between states when building the FSM
    /// data structures.
    /// </summary>

    [Parameter]
    public string? Name { get; set; }

    /// <summary>
    /// The status of this step. Captured from validation
    /// of the information in the content part of the
    /// control, from whether this step is reachable from
    /// the current step, and from direct assignment.
    /// </summary>

    [Parameter]
    public FsmStepStatus Status { get; set; }

    /// <summary>
    /// If set, the step is reachable and can be clicked
    /// to navigate to it. If unset, the step is not
    /// reachable from the current state, and will be greyed
    /// out to indicate this.
    /// </summary>

    [Parameter]
    public bool Enabled { get; set; }

    /// <summary>
    /// Is this the currently selected step?
    /// </summary>
    /// <returns>True if the current active step
    /// in the workflow</returns>
    
    public bool Active() 
        => Name == FsmWorkFlow?.ActiveState?.Name;

    /// <summary>
    /// Direct links to the set of possible transitions
    /// from this step (state) to other steps
    /// </summary>

    public List<FsmEvent>? Transitions { get; set; }

    protected override void OnInitialized()
    {
        Transitions = new();
        FsmWorkFlow?.States?.Add(this);
    }

    /// <summary>
    /// Enables the content part of the step if it
    /// is the currently selected step in the workflow
    /// </summary>
    
    private string Hidden => Active() ? "" : "hidden";

    /// <summary>
    /// Generate the CSS class to use based on the
    /// state of the step, e.g. selected, not selected
    /// </summary>
    
    private string StepClass
    {
        get
        {
            if (Name == FsmWorkFlow?.ActiveState?.Name)
                return "wfactivestep";
            else
                return "wfstep";
        }
    }

    private string Icon => Status switch
    {
        FsmStepStatus.ToDo => "unpublished",
        FsmStepStatus.InProgress => "edit_note",
        FsmStepStatus.Done => "check_circle",
        FsmStepStatus.Warning => "report_problem",
        FsmStepStatus.Blocked => "cancel",
        _ => "question_mark"
    };

    private string IconColour
    {
        get
        {
            if (!Enabled)
                return Colors.Base600;
            else
                return Status switch
                {
                    FsmStepStatus.ToDo => Colors.WarningDark,
                    FsmStepStatus.InProgress => Colors.WarningDark,
                    FsmStepStatus.Done => Colors.Success,
                    FsmStepStatus.Warning => Colors.Warning,
                    FsmStepStatus.Blocked => Colors.DangerDark,
                    _ => Colors.Base600
                };
        }
    }
}
