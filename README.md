# FsmWorkFlow - A Blazor workflow component
FsmWorkFlow is a Blazor component suite that enables the creation of a workflow. A workflow is made up of a sequence of steps, each of which displays content in the component such as a group of form fields for example. The user navigates from step to step, either by clicking controls in the step body, or by clicking on headers.

The component contains a sequence of headers, one header for each step in the workflow. Each header is numbered and contains a caption. The currently active step in the workflow has its header highlighted so that the user can see which step they have reached in the workflow.

Although there is a default expected sequence the user is expected to follow when working through the workflow, sometimes it might be permitted for the user to jump back to previous steps, or to skip over future steps, depending on what data they have entered or what options they have selected in the controls on each step. The workflow component has been designed so that it highlights the headers for those steps navigable from the current active step, and grays out those headers that cannot be selected.

Because the navigational model is not linear, the actual set of workflow steps is implemented as a finite state machine. To understand how this works, we shall look at an example.

### A login workflow
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

To create an application-specific workflow of your own, create a new .razor component or page and add it to your project. For example, if we were trying to create the login workflow of the previous diagram, we might create a SimpleLogin.razor page. In the code example found in this repository, we have implemented the example code directly as the Index.razor page.

To place a workflow into the page, we need to add references to the workflow component itself, and then appropriate using statements in the Blazor files that consume the workflow component. We also are going to make use of a simple authentication service, and therefore need to inject it as a dependency into the page. At the top of your new Blazor page (assuming the references have been added) place the following using statements:

```
@using FsmWorkFlowUI.Components
@using FsmWorkFlowUI.Data
@inject IAuthService authService
@page "/"
```

Choose the position in the page where you would like the workflow to appear, and use the following top-level tags to create the workflow:

```
<FsmWorkFlow Model="@model" @ref=workFlow>
</FsmWorkFlow>
@code {
    // Variables used in the markup
    LoginModel? model;
    FsmWorkFlow? workFlow;
    string error = string.Empty;
    protected override void OnInitialized()
    {
        model = authService.CreateLoginModel();
    }
}
```

Note that we have included some attributes bound to variables in the code section. The `Model` attribute can be used to reference any data object that holds the state data in the workflow while we are moving through its steps. The `@ref` attribute is strongly typed, and is a reference to the underlying FsmWorkFlow object itself. This is useful shold you wish to look at the status of its steps, fire an event at it, or interrogate it to find out what step we are in, for example.

In this case, we are using the specimen authentication service that has been injected into the page to get the model, which is an instance of the class LoginModel. The source code for these can be inspected in the Data folder of the FsmWorkFlowUI project.

Next we are going to add some steps to the workflow, as shown in the previous diagram for the login workflow. Between the opening and closing `<FsmWorkFlow>` tags, add the steps as shown in the code below:

```
<FsmWorkFlow Model="@model" @ref=workFlow>
    <FsmStep Name="Please login"> </FsmStep>
    <FsmStep Name="Password?"> </FsmStep>
    <FsmStep Name="Logged in options"> </FsmStep>
</FsmWorkFlow>
```

### Events and transitions between steps
We are now going to add some navigation between the steps of the workflow. Add a transition from the first to the second step by including an `FsmEvent` element between the opening and closing tags of the first state:

```
<FsmWorkFlow Model="@model" @ref=workFlow>
    <FsmStep Name="Please login"> 
        <FsmEvent On="Login" When="@UserNameTyped" Then="Password?" />
    </FsmStep>
    <FsmStep Name="Password?"> </FsmStep>
    <FsmStep Name="Logged in options"> </FsmStep>
</FsmWorkFlow>
```
We also need to provide a function called `UserNameTyped` that will return true when the user has typed their name into a text box on the workflow. We don't have the text box yet, but we can at least allocate a variable that will hold the typed name in the future. Add the following two lines of code to the `@code { ... }` section:

```
    bool UserNameTyped() => !string.IsNullOrWhiteSpace(model?.UserName);
```
At this point, it would be helpful to describe the attributes of the FsmStep and FsmEvent elements. In the case of the FsmStep element the attributes are as follows:

| Attribute | Type | Purpose |
| ---       | ---  | ---     |
| `Name` | `string?` | The unique name of this step in the workflow. Note that this is also used as the tab description, so should be readable and can include spaces. |
| `Status` | `FsmStepStatus` | An `enum` giving a number of different status indications for the step. These apply colour highlighting to the tab header, and can be set dynamically while the workflow is running. |
| `Hidden` | `bool` | Set to true to hide this step from the set of tabs at the top of the workflow. Should be used sparingly, as this creates a mystery tab that has no clues on how to navigate to it. Used as part of the dialog mechanism. |

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
| `Then` | `string` | The name of the next state or step to transit to. Note that the state transition engine has a memory of one previous state. Using the value `"$back"` for this attribute value will move to the previous state, regardless of what that previous state was. This is provided for dialog support, and should not be used under normal workflow circumstances.|

If we complete all the remaining events for the three states in the workflow, they would look like the following. Note that the required guard functions and actions have also been added to the code section:

```
<FsmWorkFlow Model="@model" @ref=workFlow>
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
    LoginModel? model;
    FsmWorkFlow? workFlow;
    string error = string.Empty;
    protected override void OnInitialized()
    {
        model = authService.CreateLoginModel();
    }

    // Guard functions
    bool PasswordValid() => authService.PasswordValid(model?.UserName, model?.Password);
    bool UserNameTyped() => !string.IsNullOrWhiteSpace(model?.UserName);

    // Action functions
    void IssueAuthToken() { model.AuthToken = authService.IssueKey(model?.UserName, model?.Password); }
    void ShowLoginError() { error = "Name or password invalid"; }
    void NeedAUserName() { error = "Please provide a valid user name"; }
    void RescindToken() { model = authService.CreateLoginModel(); error = string.Empty; }
}
```
Some things to note about the event elements:
1. Each FsmEvent element does not need to have unique Name attributes. However, they will need different When attributes so that different guard conditions can evaluate which transition to take. The guards are evaluated in the order they appear inside the paren FsmStep element, so don't put the guardless FsmEvent at the top of the list!
2. Omission of a When attribute behaves as if the guard function always evaluates to true. Therefore you can only have a single When-less FsmEvent for each event name.
3. Omission of a Do attribute means no action function is invoked on the transition between steps.
4. Omission of a Then attribute causes the current active step to remain the same. A 'Do' action function will still be invoked if one was identified in the FsmEvent.

### Providing content for each tab
So far we have only defined the steps in the workflow and the permitted transitions between them. Now for each of the tabs in the workflow, we need to provide some markup. This can either be done inline inside the `<FsmStep>` elements, or can be done using separate razor components referenced from the FsmSteps. We shall look at inline content first.

Add an FsmStepBody element beneath the FsmEvent elements inside the topmost FsmStep element. The FsmStepBody contains the markup we want to appear in the corresponding step of the workflow. In the example below we use a text entry box and a submit button to capture the user's name:

```
    <FsmStep Name="Please login"> 
        <FsmEvent On="Login" When="@UserNameTyped" Then="Password?" />
        <FsmEvent On="Login" Do="@NeedAUserName" />
        <FsmStepBody>
            <p>Please type your login name</p>
            <EditForm Model="@model" OnSubmit=@ToPasswordStep>
                <div class="form-group">
                    <label for="username">Login name</label>
                    <InputText @bind-Value="model.UserName" id="username" />
                </div>
                <input type="submit" class="btn btn-primary" value="Next" />
            </EditForm>
            <p style="color:red;">@error</p>
        </FsmStepBody>
    </FsmStep>
```
To fire events that will cause a transition to another step of the workflow, we use the `FsmWorkFlow.Fire(string eventName)` method. Since we used the `@ref` attribute to obtain a reference to the workflow, that is the object on which we invoke the `Fire` method. Add the following function to the `@code` section for the missing submit button event handler in the `EditForm`:

```
    // Transition functions
    void ToPasswordStep() => workFlow?.Fire("Login");
```
Next we shall see how to put the content of a step into a different component instead of inline. First, let us create a component to capture the password. Create a LoginPassword.razor component and populate it with the following content:

```
<p>Please enter your password, then click 'Login'</p>
    <div class="form-group">
        <label for="passwordbox">Type password</label>
        <InputText @bind-Value="@Pass" id="passwordbox" />
    </div>
    <button class="btn btn-primary" 
        @onclick=@(()=>WorkFlow?.Fire("Login"))>Login</button>

@code {
    [Parameter] public FsmWorkFlow? WorkFlow { get; set; }

    private string passwordTyped = string.Empty;

    [Parameter] public string Pass 
    { 
        get => passwordTyped; 
        set
        {
            if (value != passwordTyped)
            {
                passwordTyped = value;
                PassChanged.InvokeAsync(value);
            }
        }
    } 

    [Parameter]
    public EventCallback<string> PassChanged { get; set; }
}
```
Things to note about the code above:
1.  The combination of parameters `Pass` and `PassChanged` permits two-way binding in any parent component.
2.  The parameter `WorkFlow` is necessary so that the different razor component is still able to access the workflow's methods and properties. For example, to transition to a different step, we still need to call the `Fire` method.
3.  An alternative way to do this is the sharing of a model between several `<EditForm>` elements, one per step. To do this, the model would also either have to be passed in as a parameter, or retrieved from a service.

We now need to refer to this razor component from the workflow. Back in the parent razor file, add a step body to the `"Password?"` step as follows:

```
        <FsmStepBody>
            <LoginPassword WorkFlow="@workFlow" @bind-Pass="@model.Password" />
        </FsmStepBody>
```
This ensures that the `LoginPassword` component receives the parent workflow object, and that the password string variable in the login model is two-way bound to the `Pass` and `PassChanged` parameters of the `LoginPassword` component.

Lastly, we shall put some demo content into the body of the third step of the workflow. Be sure to include a button that will allow you to logout:

```
    <FsmStep Name="Logged in options"> 
        <FsmEvent On="Logout" Do="@RescindToken" Then="Please login" />
        <FsmStepBody>
            <p>We are successfully logged in! Our authentication token is: @model.AuthToken</p>
            <button class="btn btn-primary" 
                @onclick=@(()=>workFlow?.Fire("Logout"))>Logout</button>
        </FsmStepBody>
    </FsmStep>
```

### Status feedback
The tabs at the top of each step can be colour-coded to give feedback on whether they have been visited, completed, etc. This can be done by assigning one of the `FsmStepStatus` enum values to the Status parameter of the step, or by creating a function that computes its value. We shall add some helper functions to provide feedback in this example. Add the following functions to the bottom of the main page's `@code` segment:

```
    // Status functions
    FsmStepStatus UserNameStatus() => UserNameTyped() ? FsmStepStatus.Done : FsmStepStatus.InProgress;
    FsmStepStatus PasswordStatus() => PasswordValid() ? FsmStepStatus.Done : FsmStepStatus.InProgress;
    FsmStepStatus LoggedInStatus() => model.AuthToken != 0 ? FsmStepStatus.Done : FsmStepStatus.Blocked;
```
Now we shall decorate the three `FsmStep` elements so that their statuses are set.

For the `"Please login"` step, set the `Status` attribute to `@UserNameStatus()`.

For the `"Password?"` step, set the attribute to `@PasswordStatus()`.

For the `"Logged in options"` step, set the attribute to `@LoggedInStatus()`.

### Modal dialogs

A modal dialog component has been added to the workflow library. This dialog doesn't work inside the workflow as it stands, because of the way in which the next tab is displayed. It is provided here as a convenience tool for other parts of your UI design. It is used internally within the `FsmDialog` component to provide dialogs within the workflow. The `FsmDialog` component will be described later. Here is some sample code that uses the dialog:

```
<button class="btn btn-primary" 
        @onclick="() => dlgModal.Show = true">
    Click me!
</button>

<DlgModal @ref="@dlgModal" Title="Date and time">
        <p>The date and time are @(DateTime.Now.ToString())</p>
</DlgModal>

```

The modal dialog here has a single button with a default caption of 'Close'. It is possible to have up to three buttons, and to register event callback functions back in the parent page that are invoked once the dialog has been hidden. Even the default 'Close' button can be renamed, and can have an event callback associated with it. The properties of the `DlgModal` component are described in the following table:

| Property | Type | Purpose |
| --- | --- | --- |
| `Title` | `string?` | Provides the caption at the top of the dialog |
| `OkText` | `string?` | The caption for the OK button. If left unset or empty, this button will not appear. |
| `OnOk` | `EventCallback` | A void function to be invoked when the dialog has been dismissed after the OK button was clicked. |
| `OtherText` | `string?` | The caption for the 'Other' button. If left unset or empty, this button will not appear. |
| `OnOther` |  `EventCallback` | A void function to be invoked when the dialog has been dismissed after the 'Other' button was clicked. |
| `CancelText` | `string?` | The caption for the cancel button. Caption defaults to 'Close' so that this button appears by default. Set explicitly to the empty string to hide this button. Note that the 'X' in the top right corner of the dialog is an alias for this button, and will appear and disappear depending on the visibility of the cancel button. |
| `OnCancel` | `EventCallback` |  A void function to be invoked when the dialog has been dismissed after the 'Cancel' button was clicked. |

Note that if a button caption is set but the corresponding even callback is not, the dialog will be hidden but nothing else will happen.

Acknowledgement is given to Steven Giesel who provided an article in his blog on how to implement dialogs in Blazor. Although the code here has been altered somewhat, the original idea came from his article. The original blog post can be found [here](https://steven-giesel.com/blogPost/5fc5b957-d62e-40e6-b0c4-f2a0df5c8aa1).

### Dialog steps in the workflow

As well as supporting modal dialogs outside a workflow, there is an `FsmDialog` component that wraps up a workflow step in the style of a modal dialog box. This component being an alternative kind of `FsmStep` appears as an immediate child of the parent `FsmWorkflow` at any point an `FsmStep` might be placed. Note that you can have multiple `FsmDialog` elements, but as with `FsmStep` elements, they must each have a unique name.

To show an example of this dialog at work, we are going to modify our prevous login workflow example.

First, we shall remove the code that displayed an 'incorrect password' message in the "Password?" step. Remove the `Do=...` parameter from the second `FsmEvent` of the password step. Change its `Then=` parameter to refer to a step called "Bad credentials". Your step should now look like this:

```
    <FsmStep Name="Password?" Status="@PasswordStatus()">
        <FsmEvent On="Login" When="@PasswordValid" Do="@IssueAuthToken" Then="Logged in options" />
        <FsmEvent On="Login" Then="Bad credentials" />
        <FsmStepBody>
            <LoginPassword WorkFlow="@workFlow" @bind-Pass="@model.Password" />
        </FsmStepBody>
    </FsmStep>
```

Next we want to add another `FsmEvent` so that we don't get stuck in the password step if we have forgottem our password:

```
    <FsmStep Name="Password?" Status="@PasswordStatus()">
        <FsmEvent On="Login" When="@PasswordValid" Do="@IssueAuthToken" Then="Logged in options" />
        <FsmEvent On="Login" Then="Bad credentials" />
        <FsmEvent On="StartAgain" Then="Please login" />
        <FsmStepBody>
            <LoginPassword WorkFlow="@workFlow" @bind-Pass="@model.Password" />
        </FsmStepBody>
    </FsmStep>
```
Lastly we shall add an `FsmDialog` as another step beneath this step as follows:

```
    <FsmDialog Name="Bad credentials" OkText="Start again" OnOk="StartAgain">
        <p>
            Your user name and password were not valid. Click 'Close' to
            try a different password. Click 'Start again' to provide 
            another user name and password.
        </p>
    </FsmDialog>
```

As with the modal dialog described earlier, this dialog has a default 'Close' button that brings you back to the password step. We have also added an OK button whose caption has been changed to "Start again" and that fires the "StartAgain" event once we have returned to the password step. 

Hence you can set the `OnOk` property to any of the valid `On=` values of the previous state's list of `FsmEvent` elements. This is because dismissing the dialog temporarily jumps back to the previous state, then searches its events to select the transition to be fired.

As for the modal dialog, there are up to three buttons on an `FsmDialog`, two of which can be customised in a similar way to the buttons on the modal dialog. The table below gives the details:

| Property | Type | Purpose |
| --- | --- | --- |
| `Name` | `string?` | The caption to be used as the dialg title. This is also the name to be used in an `FsmEvent` after the `Then=` parameter to indicate which dialog should be invoked. |
| `OkText` | `string?` | The caption for the OK button. Leave unset or empty to remove the button. |
| `OnOk` | `string?` | The name of the event in the previous state that we want to fire once the dialog has been dismissed. |
| `OtherText` | `string?` | The caption for the 'Other' button. Leave unset or empty to omit the button. |
| `OnOther` | `string?` |The name of the event in the previous state that we want to fire once the dialog has been dismissed. |

The one big difference is that the 'Close' button always exists, and always just returns you to the previous step. In most cases, this would be the only button you would need, leaving just the `Name=` property to be set so the dialog can be invoked.

### Exception handling

In the `When=` and the `Do=` properties of the `FsmEvent` elements, it is possible that the code you provide might throw an exception. You can do nothing, and have the exception handled by the parent page containing the workflow. However, if the exception can be survived, allowing the workflow to still be completed, we need a facility for catching the exceptions, reporting them in the workflow, and resuming the workflow if the user chooses.

To provide this capability, we can add an extra `FsmDialog` to the workflow's list of steps. We customise its appearance to display the data from the exception we choose, and provide buttons and other input elements in the dialog to repair anything that caused the exception in the first place.

The extra `FsmDialog` is named by the `ErrDialog=` property of the `FsmWorkFlow` and will be automatically jumped to when an exception occurs. We will now modify the login workflow example to demonstrate how to do this.

Find the top line of the `FsmWorkFlow` element and add an `ErrDialog` property to it as follows:

```
<FsmWorkFlow Model="@model" @ref=workFlow ErrDialog="Unexpected Exception">
```

Next we need to add an extra step to the workflow that will be jumped to on an exception. In most cases this would be implemented as an `FsmDialog` rather than an `FsmStep`, but the latter is permitted. In the sample code below, this has been placed at the top of the workflow, but it can be placed anywhere that a step can appear:

```
<FsmWorkFlow Model="@model" @ref=workFlow ErrDialog="Unexpected Exception">
    <FsmDialog Name="Unexpected Exception">
        <p>
            An exception was thrown. It's Message property was
            '@(workFlow?.CaughtException?.Message)'.
        </p>
    </FsmDialog>
    <FsmStep . . .
```

When the exception gets thrown, the exception object is placed into the `CaughtException` property of the workflow. As you can see in the code above, this property can be accessed via the `@ref` reference variable for the workflow. The `CaughtException` property is set to null except when an exception has been caught.

We are next going to artificially trigger an exception from one of the steps, so that we can demonstrate the exception handling in action. Scroll down through the steps in the workflow until you find the "Logged in options" step. We shall add an extra transition to this step with a `Do=` action function that deliberately throws an exception:

```
    <FsmStep Name="Logged in options" Status="@LoggedInStatus()">
        <FsmEvent On="Logout" Do="@RescindToken" Then="Please login" />
        <FsmEvent On="TryDlg" Then="Test Dialog" />
        <FsmEvent On="TryException" Do="@ThrowSomething" />
```

We also need to add the `ThrowSomething` function to the `@code` section of the file:

```
    // Action functions
    void ThrowSomething() 
    { 
        throw new InvalidOperationException("The thrown exception message"); 
    }
```

Lastly we need to add a button to the step body of the "Logged in options" step that fires the "TryException" event when clicked:

```
        <FsmStepBody>
            <p>Congratulations! you are logged in. The world is your oyster! 
                When you are finished, click the button below to log out.</p>
            <button class="btn btn-primary" @onclick=@(()=>workFlow?.Fire("Logout"))>Log out</button>
            <button class="btn btn-secondary" @onclick=@(()=>workFlow?.Fire("TryDlg"))>Try dialog</button>
            <button class="btn btn-secondary" @onclick=@(()=>workFlow?.Fire("TryException"))>Try exception</button>
        </FsmStepBody>
```

If you try to run the code, and manage to log in successfully, the 'Try exception' button should show the dialog with the 'Unexpected error' caption. On dismissing the dialog, it should return to the logged in options step of the workflow. 