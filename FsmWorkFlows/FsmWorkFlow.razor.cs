using Microsoft.AspNetCore.Components;

namespace FsmWorkFlows;

public partial class FsmWorkFlow
{
    /// <summary>
    /// The injected model being manipulated by this
    /// workflow. Ideally we should strongly type this
    /// using an @typeparam in the .razor file. However
    /// this needs the template type to be propagated
    /// right down through all the FsmWorkflow elements
    /// and becomes a big piece of work for the future!
    /// For now, when extracting the model in your code
    /// down at the FsmStepBody level in the hierarchy,
    /// just cast it to the expected type.
    /// </summary>

    [Parameter]
    public object? Model { get; set; }

    /// <summary>
    /// ChildContent is used by the Blazor runtime to
    /// determine exacly where the content between the
    /// opening and closing tags should be placed in
    /// the output HTML
    /// </summary>

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The name of the step to jump to if an exception is
    /// fired while executing a transition between steps.
    /// Usually this would be an FsmDialog name so that a
    /// dialog is displayed revealing the error details. It
    /// could potentially be an ordinary state, maybe with
    /// the Hidden property set to true so that the error
    /// is reported within the tab rectangle of the workflow.
    /// </summary>

    [Parameter]
    public string? ErrDialog { get; set; }

    /// <summary>
    /// The exception most recently thrown. Only non-null
    /// if the error dialog step is active.
    /// </summary>

    public Exception? CaughtException { get; set; }

    /// <summary>
    /// The set of steps in this workflow, or states
    /// in this finite state machine
    /// </summary>

    public List<FsmStep>? States { get; set; } = new();

    /// <summary>
    /// The current active step in the workflow,
    /// or active state in the state machine
    /// </summary>

    private FsmStep? activeStep = null;

    public FsmStep? ActiveState
    {
        get
        {
            if (activeStep == null && States != null && States.Any())
                activeStep = States.First();
            return activeStep;
        }
        set
        {
            activeStep = value;
            InvokeAsync(StateHasChanged);
        }
    }

    /// <summary>
    /// The previous state is captured for a transition
    /// labelled with the "$back" pseudo-value for the
    /// "Then" attribute
    /// </summary>

    private FsmStep? PreviousState { get; set; }

    /// <summary>
    /// Given the name of a state, find the actual
    /// FsmStep object that represents that state
    /// </summary>
    /// <param name="name">The name of the state</param>
    /// <returns>The state object itself</returns>

    public FsmStep? StepFromName(string? name)
        => States?.FirstOrDefault(s => s.Name == name);

    /// <summary>
    /// Find the index of the step with the specified name
    /// </summary>
    /// <param name="name">the name of the step to find</param>
    /// <returns>0 if step not found, 1-based index if
    /// found</returns>

    public int IndexOfStep(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return 0;
        return IndexOfStep(StepFromName(name));
    }

    /// <summary>
    /// Find the index of the specified step
    /// </summary>
    /// <param name="step">The step to find</param>
    /// <returns>0 if step not found, 1-based index if
    /// found</returns>

    public int IndexOfStep(FsmStep? step)
    {
        if (step == null || States == null)
            return 0;
        return 1 + States.IndexOf(step);
    }

    /// <summary>
    /// From the list of transitions available in the
    /// current step, find those with the specified
    /// event name. If none are found, return an
    /// empty enumerable.</summary>
    /// <param name="eventName">The name of the event
    ///  to find</param>
    /// <returns>The found transitions</returns>

    public IEnumerable<FsmEvent> EventsFromName(string eventName)
        => ActiveState?.Transitions?
            .Where(e => e.On == eventName)
            ?? Enumerable.Empty<FsmEvent>();

    /// <summary>
    /// Given a selected transition, find the next step
    /// to jump to if that transition is fired. Also
    /// instigates caching of the next state to reduce
    /// subsequent lookups of the state's name.
    /// </summary>
    /// <param name="e">The FsmEvent transition whose
    ///  next step is required</param>
    /// <returns>The next step object, or null
    ///  if not found</returns>

    public FsmStep? NextStep(FsmEvent? e)
    {
        if (e == null)
            return null;
        if (e.NextStep == null)
            e.NextStep = StepFromName(e.Then);
        return e.NextStep;
    }

    /// <summary>
    /// Obtain the set of navigable transitions from the
    /// active step. To be navigable, they must have the
    /// specified event name, and must either be without
    /// a condition function (When) or when called, the
    /// condition function must return true.
    /// </summary>
    /// <param name="eventName">The name given to this
    /// particular event</param>
    /// <returns>The set of navigable events from which
    /// we might choose to navigate away from this
    /// step</returns>

    public IEnumerable<FsmEvent> ValidTransitions(string eventName)
        => EventsFromName(eventName)
                .Where(e => e.When == null || e.When());

    /// <summary>
    /// It is just possible that there is more than
    /// one transition from the current step to the
    /// target step, probably with differing guard
    /// conditions and names. This method returns
    /// the target step only if there is a single
    /// transition that matches the search criteria.
    /// This method is used to decide whether to
    /// grey out a tab. If there is a clear single
    /// unambiguous transition to the target step,
    /// the tab will be treated as a button firing
    /// that event. If not, the tab is greyed, and
    /// the buttons explicitly placed in the tab
    /// must call Fire(stepName) to select the
    /// next step.
    /// </summary>
    /// <param name="target">The destination step</param>
    /// <returns>The event object that yields this
    /// transition</returns>

    public FsmEvent? SingleValidTransitionTo
        (FsmStep target)
    {
        if (ActiveState == null || ActiveState.Transitions == null)
            return null;
        var targetTransitions
            = ActiveState.Transitions
                .Where(e => e.Then != null
                && e.Then == target.Name
                && (e.When == null || e.When()))
                .ToArray();
        if (targetTransitions.Length == 1)
            return targetTransitions[0];
        else
            return null;
    }

    /// <summary>
    /// Call this method from your code in the
    /// FsmStepBody to fire an event
    /// </summary>
    /// <param name="eventName">The name of the
    /// event you wish to trigger</param>

    public void Fire(string eventName)
    {
        try
        {
            CaughtException = null;

            // Deal with the "$back" transition. Technically
            // this is not a valid transition for the workflow,
            // but is used for implementing modal dialogs, or
            // for returning from hidden states to their
            // predecessor state.

            if (eventName == "$back")
            {
                ActiveState = PreviousState;
                return;
            }

            // Is this a valid event for the current state?
            // If so, find the transition object for it.

            IEnumerable<FsmEvent> transitions
                = EventsFromName(eventName);
            if (!transitions.Any())
                return;

            // Check the guard conditions in the order the events
            // were listed to find the first permitted transition 
            // with this event name for the current step

            FsmEvent? validEvent = transitions.FirstOrDefault
                (e => e.When == null || e.When());

            // If an event was found, execute the action associated
            // with the transition, then jump to the new state

            if (validEvent != null)
            {
                validEvent.Do?.Invoke();
                FsmStep? nextStep = NextStep(validEvent);
                if (nextStep != null)
                {
                    PreviousState = ActiveState;
                    ActiveState = nextStep;
                }
            }
        }
        catch (Exception ex)
        {
            // When an unexpected exception occurs, we show
            // a dialog that has been nominated and designed
            // by the developer. It is housed in an FsmDialog
            // whose name is referenced by the ErrDialog
            // property of this FsmWorkFlow. On cancellation of
            // that dialog, the workflow attempts to return to
            // the state it was in before the exception was
            // thrown. When in the error dialog, the exception
            // object can be accessed via the CaughtException
            // property of the parent workflow. Access this via
            // the @ref parameter for the workflow.

            FsmStep? errStep = StepFromName(ErrDialog);
            if (errStep != null)
            {
                CaughtException = ex;
                PreviousState = ActiveState;
                ActiveState = errStep;
            }
        }
    }

    /// <summary>
    /// The first rendering doesn't happen without this
    /// forced render, as the initialisation is all
    /// being done without firing events or changing
    /// exposed parameters.
    /// </summary>
    /// <param name="firstRender">True if this is the
    /// very first rendering of the component</param>

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && ShouldRender())
            StateHasChanged();
    }
}
