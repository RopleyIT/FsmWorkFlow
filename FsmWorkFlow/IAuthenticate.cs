namespace FsmWorkFlow;

public interface IAuthenticate
{
    /// <summary>
    /// Return the user name (official user name) if
    /// authenticated, null if not
    /// </summary>
    /// <param name="userName">One of the user's login names</param>
    /// <param name="password">The user's password</param>
    /// <returns>The user's name if authenticated, null if not</returns>
    public string? IsAuthenticatedUser(string userName, string password);
}
