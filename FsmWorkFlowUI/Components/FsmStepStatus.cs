namespace FsmWorkFlowUI.Components
{
    /// <summary>
    /// Enum used to choose the icon and colour
    /// for the steps in the workflow
    /// </summary>
    
    public enum FsmStepStatus
    {
        None = 0,           // Not set to anything
        ToDo = 1,           // Not visited, or nothing done
        InProgress = 2,     // Visited but not completed
        Done = 3,           // Completed step
        Warning = 4,        // A problem with this step
        Blocked = 5         // Major problem with this step
    }
}
