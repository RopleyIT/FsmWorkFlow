using Microsoft.AspNetCore.Components;

namespace FsmWorkFlows;

public partial class FsmEvent
{
    /// <summary>
    /// The parent FsmStep object that this and
    /// other FsmEvents describe transitions from
    /// </summary>

    [CascadingParameter]
    protected FsmStep? FsmStep { get; set; }

    /// <summary>
    /// The name of the event that is being fired
    /// </summary>

    [Parameter]
    public string? On { get; set; }

    /// <summary>
    /// The guard condition that must be true when
    /// the event is fired for the action to take
    /// place and the transition to the next state
    /// to happen
    /// </summary>

    [Parameter]
    public Func<bool>? When { get; set; }

    /// <summary>
    /// The action to be executed while transiting
    /// between states
    /// </summary>
    [Parameter]
    public Func<Action<string>, Task>? Do { get; set; }

    /// <summary>
    /// The name of the next state to jump to
    /// </summary>

    [Parameter]
    public string? Then { get; set; }

    /// <summary>
    /// Reference to the ensuing step (state) for
    /// this transition. Initialized using lazy
    /// initialization.
    /// </summary>

    public FsmStep? NextStep { get; set; }

    protected override void OnInitialized()
    {
        // Add this event to the list of transitions
        // recognised as valid transitions from the
        // parent step.

        FsmStep?.Transitions?.Add(this);
    }
}
