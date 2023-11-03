using CommonStateWorkFlow;

namespace FsmWorkFlowTest;

[TestClass]
public class CommonStateTests
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
        Assert.AreEqual
            (fsm.GetCurrentStateName(), "Showing Outcome");
        Assert.AreEqual
            ("Displaying test output!", fsm.TestModel.CompletionResult);
        fsm.Terminate();
        Assert.AreEqual(fsm.GetCurrentStateName(), "No State");
    }

    [TestMethod]
    public void HandlesIncompleteFormFIlling()
    {
        FsmStateMachine fsm = new(new AuthenticationLib());
        fsm.SubmitTestRunForm();
        Assert.AreEqual(fsm.GetCurrentStateName(), "Filling Form");
    }

    [TestMethod]
    public void HandlesFailedFirstAuth()
    {
        FsmStateMachine fsm = new(new AuthenticationLib());
        fsm.TestModel.TestRunId = 1;
        fsm.SubmitTestRunForm();
        fsm.Authenticate("fred", "badpass");
        Assert.AreEqual(fsm.GetCurrentStateName(), "Authenticating First Signer");
    }

    [TestMethod]
    public void ResetsInputForm()
    {
        FsmStateMachine fsm = new(new AuthenticationLib());
        fsm.TestModel.TestRunId = 1;
        fsm.SubmitTestRunForm();
        fsm.Authenticate("fred", "fred");
        fsm.Start();
        Assert.AreEqual(fsm.GetCurrentStateName(), "Filling Form");
    }

    [TestMethod]
    public void TerminatesPrematurely()
    {
        FsmStateMachine fsm = new(new AuthenticationLib());
        fsm.TestModel.TestRunId = 1;
        fsm.SubmitTestRunForm();
        fsm.Authenticate("fred", "fred");
        fsm.Terminate();
        Assert.AreEqual(fsm.GetCurrentStateName(), "No State");
    }
}
