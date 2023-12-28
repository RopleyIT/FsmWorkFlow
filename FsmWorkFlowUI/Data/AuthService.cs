namespace FsmWorkFlowUI.Data
{
    public class AuthService : IAuthService
    {
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

        public long IssueKey(string? name, string? password)
        {
            if (name != null
                && password != null
                && name == password) // Ugh!
            {
                int hash = name.GetHashCode();
                return 4093 + hash - hash % 4093;
            }
            else
                return 0;
        }

        /// <summary>
        /// Check to see if a previously issued key is
        /// for a valid user
        /// </summary>
        /// <param name="key">The key value</param>
        /// <returns>True if a valid key</returns>

        public bool ValidUserKey(long? key)
            => key.HasValue && key != 0 && key % 4093 == 0;
    }
}
