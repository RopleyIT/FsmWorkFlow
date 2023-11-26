namespace FsmWorkFlowUI.Data
{
    public class ExampleService
    {
        public void Analyze(ExampleModel? model)
        {
            // Do something useful to compute
            // analysis results, and get back
            // an ID to use to look those results
            // up in the DB server later

            if(model != null)
                model.ResultId = 
                    (int)((DateTime.Now 
                        - new DateTime(2023, 1, 1)).TotalSeconds);
        }

        /// <summary>
        /// Retrieve a new empty model for an anlysis
        /// </summary>
        /// <returns>The new empty model</returns>
        
        public ExampleModel GetModel() 
        {
            return new ExampleModel();
        }

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
