namespace FsmWorkFlow;

public class AuthenticatingSecondSigner : IFsmState
{
    public FsmStateMachine Fsm { get; init; }
    public AuthenticatingSecondSigner(FsmStateMachine fsm) => Fsm = fsm;

    public void Authenticate(string user, string password)
    {
        string? authUser = Fsm.AuthLib.IsAuthenticatedUser(user, password);
        if (authUser != null)
        {
            Fsm.TestModel.SecondApprover = authUser;
            // Run the action associated with this transition
            Fsm.TestModel.GenerateTestOutput();
            Fsm.CurrState = Fsm.ShowingOutcome;
        }
    }

    public void Start()
    {
        // This event might occur if we decide to just start
        // filling the form from scratch again.

        Fsm.TestModel = new();
        Fsm.CurrState = Fsm.FillingFormState;
    }
}
