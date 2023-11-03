namespace FsmWorkFlow;

/// <summary>
/// Interface describing the lists of events for each state
/// </summary>

public interface IFsmState
{
    /// <summary>
    /// Reference back to the state machine itself so that
    /// the event handlers can see the models they need to access
    /// </summary>

    public FsmStateMachine Fsm { get; }

    /// <summary>
    /// Event fired when launching or restarting the workflow
    /// </summary>
    public void Start() { /* Logging maybe here */ }

    /// <summary>
    /// Event fired when the input data for the test run
    /// has been captured
    /// </summary>
    public void SubmitTestRunForm() { /* Logging maybe here */ }

    /// <summary>
    /// Event fired when form completion has been authorised
    /// by first signer
    /// </summary>
    /// <param name="user">Credentials</param>
    /// <param name="password">Credentials</param>
    public void Authenticate(string user, string password) 
    { 
        /* Logging maybe here */ 
    }

    /// <summary>
    /// Event fired when use case is over
    /// </summary>
    /// <returns>The null state if successful</returns>
    public void Terminate() { Fsm.CurrState = null; }
}