using Microsoft.AspNetCore.Components;

namespace FsmWorkFlowUI.Components;

public partial class FsmStepBody
{
    /// <summary>
    /// The parent FsmStep object that this and
    /// other FsmEvents describe transitions from
    /// </summary>

    [Parameter]
    public FsmWorkFlow? WorkFlow { get; set; }

    public string ShowTree()
    {
        string treeContent = "";
        if (WorkFlow == null)
            return "Null workflow";
        if (WorkFlow.States == null)
            return "No states in workflow";
        foreach (FsmStep step in WorkFlow.States)
        {
            if (step == WorkFlow.ActiveState)
                treeContent += "Active State:";
            treeContent += step.Name + ": ";
            if (step.Transitions == null)
                treeContent += "no transitions ";
            else
                foreach (FsmEvent e in step.Transitions)
                    treeContent += e.On + "->" + e.Then + " ";
        }
        return treeContent;
    }
}
