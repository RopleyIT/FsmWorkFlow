using FsmWorkFlow;

namespace FsmWorkFlowTest;

[TestClass]
public class TransitionTests
{
    [TestMethod]
    public void CanCreateFsm()
    {
        FsmStateMachine fsm = new(new AuthenticationLib());
        Assert.IsInstanceOfType(fsm, typeof(FsmStateMachine));
        Assert.IsNotNull(fsm.TestModel);
    }

    [TestMethod]
    public void CanRunMainSequence()
    {
        FsmStateMachine fsm = new(new AuthenticationLib());
        fsm.TestModel.TestRunId = 1;
        fsm.SubmitTestRunForm();
        fsm.Authenticate("fred", "fred");
        fsm.Authenticate("joe", "joe");
        Assert.IsInstanceOfType
            (fsm.CurrState, typeof(ShowingOutcome));
        Assert.AreEqual
            ("Displaying test output!", fsm.TestModel.CompletionResult);
        fsm.Terminate();
        Assert.IsNull(fsm.CurrState);
    }

    [TestMethod]
    public void HandlesIncompleteFormFIlling()
    {
        FsmStateMachine fsm = new(new AuthenticationLib());
        fsm.SubmitTestRunForm();
        Assert.IsInstanceOfType
            (fsm.CurrState, typeof(FillingFormState));
    }

    [TestMethod]
    public void HandlesFailedFirstAuth()
    {
        FsmStateMachine fsm = new(new AuthenticationLib());
        fsm.TestModel.TestRunId = 1;
        fsm.SubmitTestRunForm();
        fsm.Authenticate("fred", "badpass");
        Assert.IsInstanceOfType
            (fsm.CurrState, typeof(AuthenticatingFirstSigner));
    }

    [TestMethod]
    public void ResetsInputForm()
    {
        FsmStateMachine fsm = new(new AuthenticationLib());
        fsm.TestModel.TestRunId = 1;
        fsm.SubmitTestRunForm();
        fsm.Authenticate("fred", "fred");
        fsm.Start();
        Assert.IsInstanceOfType
            (fsm.CurrState, typeof(FillingFormState));
    }

    [TestMethod]
    public void TerminatesPrematurely()
    {
        FsmStateMachine fsm = new(new AuthenticationLib());
        fsm.TestModel.TestRunId = 1;
        fsm.SubmitTestRunForm();
        fsm.Authenticate("fred", "fred");
        fsm.Terminate();
        Assert.IsNull(fsm.CurrState);
    }
}