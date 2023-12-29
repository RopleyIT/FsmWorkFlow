# FsmWorkFlow - A Blazor workflow component
FsmWorkFlow is a Blazor component suite that enables the creation of a workflow. A workflow is made up of a sequence of steps, each of which displays content in the component such as a group of form fields for example. The user navigates from step to step, either by clicking controls in the step body, or by clicking on headers.

The component contains a sequence of headers, one header for each step in the workflow. Each header is numbered and contains a caption. The currently active step in the workflow has its header highlighted so that the user can see which step they have reached in the workflow.

Although there is a default expected sequence the user is expected to follow when working through the workflow, sometimes it might be permitted for the user to jump back to previous steps, or to skip over future steps, depending on what data they have entered or what options they have selected in the controls on each step. The workflow component has been designed so that it highlights the headers for those steps navigable from the current active step, and grays out those headers that cannot be selected.

Because the navigational model is not linear, the actual set of workflow steps is implemented as a finite state machine. To understand how this works, we shall look at an example.

## A login workflow
The picture below represents a three-step workflow implementing a login process.

![Login state transition diagram](Docs/LoginWorkflow.svg)

The states in the diagram are represented by the round-cornered boxes. Each state represents a period of time in which the workflow is displaying a particular tab page of form controls or other content. Unless the user does something, like clicking a button for example, the workflow component will stay in that state until such an event happens.

A transition from one state to another is represented by an arrow joining the two states showing the direction of navigation. In order for a transition to be taken, an event must be fired. The labels on the arrows denote the events that fire.

For example, while resting in the 'Showing login tab' state, the 'Click next' event may cause a transition to the 'Request password' state.

When resting in a state, if an event is fired the event may also carry a guard condition. The guard condition must evaluate to 'true' for the transition to take place. Guard conditions on the picture are represented by text in square brackets. For example, if in the 'Showing login tab' state and the 'Click next' event occurs, the transition will only be taken if the username has been typed.

Each time a transition takes place, some action can be taken by the component. The programmer can plug in a function to be executed that will be called when the transition takes place. On the diagram, actions are denoted by text after a forward slash character. For example, if resting in the 'Request password' state and the 'Click login' event occurs, provided the password is valid the action taken is to issue an authentication token.

In summary then, a workflow consists of a set of states, usually followed in a standard sequence, but where the sequence can be varied. Each state corresponds to a displayed tab area and a selected tab at the top of the workflow control. Navigation between tabs is caused either by clicking any tabs that are enabled, or explicitly by wiring up a button within the body of a tab. Control over the routes through the set of states is maintained by guard conditions evaluable at each step of the workflow.

### Programming a workflow
To use the workflow components you will need the FsmWorkFlowUI project added to your solution. At present, this has not been rehomed in a standalone Blazor component library.

To create an application-specific workflow of your own, create a new .razor component or page and add it to your project. For example, if we were trying to create the login workflow of the previous diagram, we might create a SimpleLogin.razor page.

To place a workflow into the page, we need to add references to the workflow component itself, and then appropriate using statements in the Blazor files that consume the workflow component. We also are going to make use of a simple authentication service, and therefore need to inject it as a dependency into the page. At the top of your new Blazor page (assuming the references have been added) place the following using statements:

```
@using FsmWorkFlowUI.Components
@inject IAuthService authService
```

Choose the position in the page where you would like the workflow to appear, and use the following top-level tags to create the workflow:

```
<FsmWorkFlow Model="@authToken" @ref=workFlow>
</FsmWorkFlow>
@code {
    string? authToken = null;
    FsmWorkFlow? workFlow;
}
```

Note that we have included some attributes bound to variables in the code section. The `Model` attribute can be used to reference any data object that holds the state data in the workflow while we are moving through its steps. The `@ref` attribute is strongly typed, and is a reference to the underlying FsmWorkFlow object itself. This is useful shold you wish to look at the status of its steps, fire an event at it, or interrogate it to find out what step we are in, for example.

Next we are going to add some steps to the workflow, as shown in the previous diagram for the login workflow. Between the opening and closing `<FsmWorkFlow>` tags, add the steps as shown in the code below:

```
<FsmWorkFlow Model="@authToken" @ref=workFlow>
    <FsmStep Name="Please login"> </FsmStep>
    <FsmStep Name="Password?"> </FsmStep>
    <FsmStep Name="Logged in options"> </FsmStep>
</FsmWorkFlow>
```

### Events and transitions between steps
We are now going to add some navigation between the steps of the workflow. Add a transition from the first to the second step by including an `FsmEvent` element between the opening and closing tags of the first state:

```
<FsmWorkFlow Model="@authToken" @ref=workFlow>
    <FsmStep Name="Please login"> 
        <FsmEvent On="Login" When="@UserNameTyped" Then="Password?" />
    </FsmStep>
    <FsmStep Name="Password?"> </FsmStep>
    <FsmStep Name="Logged in options"> </FsmStep>
</FsmWorkFlow>
```
We also need to provide a function called `UserNameTyped` that will return true when the user has typed their name into a text box on the workflow. We don't have the text box yet, but we can at least allocate a variable that will hold the typed name in the future. Add the following two lines of code to the `@code { ... }` section:

```
    string? name;
    bool UserNameTyped() => !string.IsNullOrWhiteSpace(name);
```
At this point, it would be helpful to describe the attributes of the FsmStep and FsmEvent elements. In the case of the FsmStep element the attributes are as follows:

| Attribute | Type | Purpose |
| ---       | ---  | ---     |
| `Name` | `string?` | The unique name of this step in the workflow. Note that this is also used as the tab description, so should be readable and can include spaces. |
| `Status` | `FsmStepStatus` | An `enum` giving a number of different status indications for the step. These apply colour highlighting to the tab header, and can be set dynamically while the workflow is running.|

The values of the FsmStepStatus enum are as follows:

| Value | Colour | Expected interpretation |
| --- | --- |--- |
| ToDo | Black | Step not visited, or nothing done in this step so far |
| InProgress | Yellow | Visited but not completed |
| Done | Green | Used to denote a completed step |
| Warning | Orange | A problem with this step that needs attention |
| Blocked | Red | A major problem with this step |

There is also an `Enabled` public property of the FsmStep element that indicates whether the step can be navigated to by clicking on its header. If the step is not enabled, its status colour will be grey.

The attributes of the `FsmEvent` element match the fields of the transitions on the state diagram seen earlier in this document. They are listed in the table here:

| Attribute | Type | Purpose |
| --- | --- | --- |
| `On` | `string` | Gives a name to the event that must be fired for this transition to take place. |
| `When` | `Func<bool>` | A function that computes the guard condition that must be true if this transition is to be taken. If omitted, the transition can always be taken when this event is fired. |
| `Do` | `Action` | A void function that will be executed as the transition is taken. If omitted, no action is taken but the transition still happens. |
| `Then` | `string` | Then name of the next state or step to transit to. |

If we complete all the remaining events for the three states in the workflow, they would look like the following. Note that the required guard functions and actions have also been added to the code section:

```
<FsmWorkFlow Model="@authToken" @ref=workFlow>
    <FsmStep Name="Please login"> 
        <FsmEvent On="Login" When="@UserNameTyped" Then="Password?" />
        <FsmEvent On="Login" Do="@NeedAUserName" />
    </FsmStep>
    <FsmStep Name="Password?">
        <FsmEvent On="Login" When="@PasswordValid" Do="@IssueAuthToken" Then="Logged in options" />
        <FsmEvent On="Login" Do="@ShowLoginError" Then = "Please login" />
    </FsmStep>
    <FsmStep Name="Logged in options"> 
        <FsmEvent On="Logout" Do="@RescindToken" Then="Please login" />
    </FsmStep>
</FsmWorkFlow>

@code {
    // Variables used in the markup
    long authToken = 0;
    FsmWorkFlow? workFlow;
    string? name;
    string? password;
    string? error;

    // Guard functions
    bool PasswordValid() => authService.PasswordValid(name, password); 
    bool UserNameTyped() => !string.IsNullOrWhiteSpace(name);

    // Action functions
    void IssueAuthToken() { authToken = authService.IssueKey(name, password); }
    void ShowLoginError() { error = "Name or password invalid";  }
    void NeedAUserName() { error = "Please provide a valid user name"; }
    void RescindToken() { authToken = 0; }
}
```
### Providing content for each tab
So far we have only defined the steps in the workflow and the permitted transitions between them. Now for each of the tabs in the workflow, we need to provide some markup. This can either be done inline inside the `<FsmStep>` elements, or can be done using separate razor components referenced from the FsmSteps. Here we shall look at inline content first.

