namespace FsmWorkFlow;

public class ShowingOutcome : IFsmState
{
    public FsmStateMachine Fsm { get; init; }
    public ShowingOutcome(FsmStateMachine fsm) => Fsm = fsm;
}
