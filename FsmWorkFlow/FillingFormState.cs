namespace FsmWorkFlow;

public class FillingFormState : IFsmState
{
    public FillingFormState(FsmStateMachine fsm) => Fsm = fsm;

    public FsmStateMachine Fsm { get; init; }

    public void Start() => Fsm.TestModel = new();

    public void SubmitTestRunForm()
    {
        if (Fsm.TestModel.Validate())
            Fsm.CurrState = Fsm.AuthenticatingFirstSigner;
    }
}
