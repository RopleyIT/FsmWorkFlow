namespace FsmWorkFlow;

/// <summary>
/// An example of the model to be bound to the view.
/// The visible panels in the view match the steps
/// the workflow transits through, mapping a different
/// view to each state.
/// </summary>

public class TestModel
{
    public long? TestRunId { get; set; }
    public string? Analyte { get; set; }
    public double? Concentration { get; set; }
    public string? Units { get; set; }
    public string? FirstApprover { get; set; }
    public string? SecondApprover { get; set; }
    public string? ValidationError { get; set; }
    public string? CompletionResult { get; set; }

    /// <summary>
    /// An example of a validation function, whose
    /// job is to check the TestModel has been
    /// filled with valid data. Used as an example
    /// of a conditional check on one of the events
    /// that causes a transition.
    /// </summary>
    /// <returns>True if the condition met, false
    /// if not</returns>
    
    public bool Validate()
    {
        if (TestRunId == null)
        {
            ValidationError = "Missing TestRunId";
            return false;
        }
        return true;
    }

    /// <summary>
    /// An example of an action function to be executed
    /// when an event is fired and its condition is true
    /// </summary>
    
    public void GenerateTestOutput()
    {
        // Place holder for completing the analysis
        CompletionResult = "Displaying test output!";
    }
}
