namespace CommonStateWorkFlow;

/// <summary>
/// The finite state machine created for the lifetime
/// of a workflow. It's purpose is to oversee the
/// sequence of interactions between a user at the UI
/// and the model (TestModel). At the end of the
/// workflow, the state machine is discarded but the
/// completed/updated TestModel remains as the outcome.
/// </summary>

public class FsmStateMachine 
{
    // The test model to be filled in by the workflow
    public TestModel TestModel { get; set; }

    // An injected service. This one authenticates users.
    internal IAuthenticate AuthLib { get; init; }

    // Each of the state objects that encapsulate the
    // behaviour to be executed for each event in that
    // state.
    internal State FillingFormState { get; init; }
    internal State AuthenticatingFirstSigner { get; init; }
    internal State AuthenticatingSecondSigner { get; init; }
    internal State ShowingOutcome { get; init; }

    // The current state the state machine is in. Set
    // to null when the state machine terminates
    internal State? CurrState { get; set; }

    // For testing only
    public string GetCurrentStateName()
    {
        if (CurrState == FillingFormState)
            return "Filling Form";
        if (CurrState == AuthenticatingFirstSigner)
            return "Authenticating First Signer";
        if (CurrState == AuthenticatingSecondSigner)
            return "Authenticating Second Signer";
        if (CurrState == ShowingOutcome)
            return "Showing Outcome";
        return "No State";
    }

    /// <summary>
    /// Constructor. Injected authorisation service.
    /// </summary>
    /// <param name="authLib">The injected service</param>
    public FsmStateMachine(IAuthenticate authLib)
    {
        // Handle dependency injections

        AuthLib = authLib;

        // Create an empty test model
        TestModel = new();

        // Create and initialise all the state objects here.
        // By using a single common state class that has delegates
        // instead of actual member functions, we can avoid the
        // need to use multiple classes for the states.

        FillingFormState = new State(this)
        {
            Start = () => TestModel = new(),
            SubmitTestRunForm = () =>
            {
                if (TestModel.Validate())
                    CurrState = AuthenticatingFirstSigner;
            }
        };

        AuthenticatingFirstSigner = new State(this)
        {
            Start = () =>
            {
                TestModel = new();
                CurrState = FillingFormState;
            },
            Authenticate = (user, password) =>
            {
                string? authUser = AuthLib.IsAuthenticatedUser(user, password);
                if (authUser != null)
                {
                    TestModel.FirstApprover = authUser;
                    CurrState = AuthenticatingSecondSigner;
                }
            }
        };

        AuthenticatingSecondSigner = new State(this)
        {
            Start = () =>
            {
                TestModel = new();
                CurrState = FillingFormState;
            },
            Authenticate = (user, password) =>
            {
                string? authUser = AuthLib.IsAuthenticatedUser(user, password);
                if (authUser != null)
                {
                    TestModel.SecondApprover = authUser;
                    // Run the action associated with this transition
                    TestModel.GenerateTestOutput();
                    CurrState = ShowingOutcome;
                }
            }
        };

        ShowingOutcome = new State(this);

        // Set the starting state for the machine
        CurrState = FillingFormState;

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
}
