using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonStateWorkFlow;

public class State
{
    private readonly static Action EmptyAction = () => { };

    /// <summary>
    /// Constructor. Sets defaults for each of the event
    /// function delegates so that we only need to
    /// initialise the ones that need particular behaviour.
    /// </summary>
    /// <param name="fsm">The controlling state machine</param>
    
    public State(FsmStateMachine fsm)
    {
        Start = EmptyAction;
        SubmitTestRunForm = EmptyAction;
        Authenticate = (user, password) => { };
        Terminate = () => {  fsm.CurrState = null; };
    }

    /// <summary>
    /// Event fired when launching or restarting the workflow
    /// </summary>
    
    public Action Start; 

    /// <summary>
    /// Event fired when the input data for the test run
    /// has been captured
    /// </summary>
    
    public Action SubmitTestRunForm;

    /// <summary>
    /// Event fired when form completion has been authorised
    /// by first signer
    /// </summary>
    /// <param name="user">Credentials</param>
    /// <param name="password">Credentials</param>
    
    public Action<string, string> Authenticate;

    /// <summary>
    /// Event fired when use case is over
    /// </summary>
    
    public Action Terminate; 
}
