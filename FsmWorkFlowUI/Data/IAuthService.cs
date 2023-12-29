﻿namespace FsmWorkFlowUI.Data
{
    public interface IAuthService
    {
        /// <summary>
        /// Given a login name and a password, perform a check to
        /// see if these match valid login credentials. Note that
        /// the implementation here is a placeholder (obviously!)
        /// </summary>
        /// <param name="name">The name of the person attempting
        /// to authenticate</param>
        /// <param name="password">The password they entered
        /// </param>
        /// <returns>True if the credentials are valid</returns>

        bool PasswordValid(string? name, string? password);

        /// <summary>
        /// Demo of an authentiation API in a service.
        /// Note that in practice, this would not be
        /// in the same service as the model API, and
        /// would not be so fragile!
        /// </summary>
        /// <param name="name">Entered user name</param>
        /// <param name="password">The password they typed</param>
        /// <returns>A key token for the user, or 0 if
        /// not authenticated</returns>

        long IssueKey(string? name, string? password);

        /// <summary>
        /// Check to see if a previously issued key is
        /// for a valid user
        /// </summary>
        /// <param name="key">The key value</param>
        /// <returns>True if a valid key</returns>

        bool ValidUserKey(long? key);
    }
}