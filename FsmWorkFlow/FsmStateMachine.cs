namespace FsmWorkFlow;

/// <summary>
/// The finite state machine created for the lifetime
/// of a workflow. It's purpose is to oversee the
/// sequence of interactions between a user at the UI
/// and the model (TestModel). At the end of the
/// workflow, the state machine is discarded but the
/// completed/updated TestModel remains as the outcome.
/// </summary>

public class FsmStateMachine : IFsmState
{
    // The test model to be filled in by the workflow
    public TestModel TestModel { get; set; }

    // An injected service. This one authenticates users.
    internal IAuthenticate AuthLib { get; init; }

    // Each of the state objects that encapsulate the
    // behaviour to be executed for each event in that
    // state.
    internal IFsmState FillingFormState { get; init; }
    internal IFsmState AuthenticatingFirstSigner { get; init; }
    internal IFsmState AuthenticatingSecondSigner { get; init; }
    internal IFsmState ShowingOutcome { get; init; }

    // The current state the state machine is in. Set
    // to null when the state machine terminates
    public IFsmState? CurrState { get; set; }

    /// <summary>
    /// Constructor. Injected authorisation service.
    /// </summary>
    /// <param name="authLib">The injected service</param>
    public FsmStateMachine(IAuthenticate authLib)
    {
        // Handle dependency injections
        AuthLib = authLib;

        // Create all the state objects here
        FillingFormState = new FillingFormState(this);
        AuthenticatingFirstSigner = new AuthenticatingFirstSigner(this);
        AuthenticatingSecondSigner = new AuthenticatingSecondSigner(this);
        ShowingOutcome = new ShowingOutcome(this);

        // Set the starting state for the machine
        CurrState = FillingFormState;

        // Create an empty test model
        TestModel = new();

        // In case some other initialisation takes place
        CurrState.Start();
    }

    // Delegate all the events across to the current state
    // object, whichever state that happens to be

    public void Authenticate(string user, string password)
        => CurrState?.Authenticate(user, password);

    public void Start() => CurrState?.Start();

    public void SubmitTestRunForm()
        => CurrState?.SubmitTestRunForm();

    public void Terminate() => CurrState?.Terminate();

    /// <summary>
    /// Here to satisfy the IFsmState interface requirements
    /// </summary>

    public FsmStateMachine Fsm => this;
}
