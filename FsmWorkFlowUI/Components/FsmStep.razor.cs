using Microsoft.AspNetCore.Components;
using Radzen;
using System.ComponentModel.Design;

namespace FsmWorkFlowUI.Components;

public partial class FsmStep
{
    /// <summary>
    /// Set to true for the step to not appear with a tab at the top.
    /// Used for steps that implement modal dialogs or error messages.
    /// </summary>
    [Parameter]
    public bool Hidden { get; set; } = false;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Propagate the tab content up to the parent where
    /// it will be used to infill the area beneath the buttons
    /// </summary>

    public RenderFragment? TabBody => StepBody?.ChildContent;

    /// <summary>
    /// The content container for the body of the
    /// tab that will appear beneath the step button
    /// </summary>
    
    public ITab? StepBody { get; set; }

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
    /// out to indicate this. Note that if the state is the
    /// current state, it is deemed reachable because we have
    /// reached it!
    /// </summary>

    public bool Enabled 
        => Active() || SingleValidEventToHere != null;

    /// <summary>
    /// If there is only one event that could be fired
    /// successfully in the active step that would
    /// get us to this step, find the name of the event
    /// so that we can Fire(eventName) from the tab
    /// when clicked on.
    /// </summary>
    
    public string? SingleValidEventToHere
    {
        get
        {
            FsmEvent? toHere = FsmWorkFlow?
                .SingleValidTransitionTo(this);
            if (toHere != null)
                return toHere.On;
            else
                return null;
        }
    }

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

    /// <summary>
    /// Propagate the event firing to the workflow
    /// manager parent
    /// </summary>
    /// <param name="eventName">The name of the
    /// event to be fired</param>
    
    public void Fire(string eventName)
        => FsmWorkFlow?.Fire(eventName);

    /// <summary>
    /// If there is a single valid event from
    /// the active state to this state, fire it
    /// </summary>
    
    public void FireDefault()
    {
        string? toFire = SingleValidEventToHere;
        if (toFire != null)
            Fire(toFire);
    }

    protected override void OnInitialized()
    {
        Status = FsmStepStatus.ToDo;
        Transitions = new();
        FsmWorkFlow?.States?.Add(this);
    }

    private int StepNumber => FsmWorkFlow?.IndexOfStep(this)??0;

    /// <summary>
    /// Generate the CSS class to use based on the
    /// state of the step, e.g. selected, not selected
    /// </summary>

    private string StepClass 
        => Active() ? "wfactivestep" : "wfstep";

    private string Icon => Status switch
    {
        FsmStepStatus.ToDo => "unpublished",
        FsmStepStatus.InProgress => "edit_note",
        FsmStepStatus.Done => "check_circle",
        FsmStepStatus.Warning => "report_problem",
        FsmStepStatus.Blocked => "cancel",
        _ => "question_mark"
    };

    private string TextStyle
        => Enabled ? "color: black;" : "color: #999;";

    private string StepColour
    {
        get
        {
            if (!Enabled)
                return "#999";
            else
                return Status switch
                {
                    FsmStepStatus.ToDo => "black",
                    FsmStepStatus.InProgress => "orange",
                    FsmStepStatus.Done => "limegreen",
                    FsmStepStatus.Warning => "tomato",
                    FsmStepStatus.Blocked => "red",
                    _ => "999"
                };
        }
    }
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
