using Microsoft.AspNetCore.Components;

namespace FsmWorkFlowUI.Components;

/// <summary>
/// Class to contain the actual content of the currently
/// active step in the workflow. Implements the logic
/// needed to initialize the parent step with a reference
/// to this content.
/// </summary>

public partial class FsmStepBody
{
    /// <summary>
    /// Implements the ITab interface so that the
    /// content can be extracted and injected into
    /// the body of the workflow control
    /// </summary>
   
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The parent FsmStep object that this and
    /// other FsmEvents describe transitions from.
    /// This is only used to initialize the parent's
    /// StepBody member to point at the content.
    /// </summary>

    [CascadingParameter]
    public FsmStep? Step { get; set; }

    /// <summary>
    /// Tell the parent about this body's markup content
    /// so that it can be painted when this is the
    /// selected step
    /// </summary>
    
    protected override void OnInitialized()
    {
        if (Step != null)
            Step.StepBody = this;
    }

    /// <summary>
    /// The link up to the model from the top-level
    /// FsmWorkFlow element. Remember that currently
    /// you need to cast this to the correct type for
    /// the model before acessing its members. At some
    /// point in the future, we'll get this to be strongly
    /// typed using @typeparam directives, and adding
    /// generic arguments to all the classes down the
    /// hierarchy.
    /// </summary>
    
    public object? Model
        => Step?.FsmWorkFlow?.Model;

    /** YOUR ADDITIONAL STEP-SPECIFIC CODE GOES BENEATH HERE **/
}
