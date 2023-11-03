namespace FsmWorkFlow;

public class AuthenticatingFirstSigner : IFsmState
{
    public FsmStateMachine Fsm { get; init; }
    public AuthenticatingFirstSigner(FsmStateMachine fsm) => Fsm = fsm;
    public void Authenticate(string user, string password)
    {
        string? authUser = Fsm.AuthLib.IsAuthenticatedUser(user, password);
        if (authUser != null)
        {
            Fsm.TestModel.FirstApprover = authUser;
            Fsm.CurrState = Fsm.AuthenticatingSecondSigner;
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
