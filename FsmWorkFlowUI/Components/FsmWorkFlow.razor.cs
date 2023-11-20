﻿using Microsoft.AspNetCore.Components;

namespace FsmWorkFlowUI.Components;

public partial class FsmWorkFlow
{
    /// <summary>
    /// ChildContent is used by the Blazor runtime to
    /// determine exacly where the content between the
    /// opening and closing tags should be placed in
    /// the output HTML
    /// </summary>

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

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
        }
    }

    /// <summary>
    /// Given the name of a state, find the actual
    /// FsmStep object that represents that state
    /// </summary>
    /// <param name="name">The name of the state</param>
    /// <returns>The state object itself</returns>

    public FsmStep? StepFromName(string? name)
        => States?.FirstOrDefault(s => s.Name == name);

    /// <summary>
    /// From the list of transitions available in the
    /// current step, find those with the specified
    /// event name. If noe are found, return an
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
    ///  next step is required//</param>
    /// <returns>The next step object, or null
    ///  if not found</returns>

    public FsmStep? NextStep(FsmEvent? e)
    {
        if (e?.NextStep == null)
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
    /// This method is used to decide
    /// </summary>
    /// <param name="target">The destination step</param>
    /// <returns>The event object that yields this
    /// transition</returns>
    
    public FsmEvent? SingleValidTransition
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
            ActiveState = NextStep(validEvent);
        }
    }
}