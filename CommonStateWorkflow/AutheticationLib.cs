namespace CommonStateWorkFlow;

/// <summary>
/// An example of an injected service into the state
/// machine example. This provides credential validation.
/// </summary>
public class AuthenticationLib : IAuthenticate
{
    public string? IsAuthenticatedUser(string userName, string password)
    {
        if (!string.IsNullOrWhiteSpace(userName)
            && string.Compare(userName, password) == 0)
            return userName.ToLower();
        else return null;
    }
}
