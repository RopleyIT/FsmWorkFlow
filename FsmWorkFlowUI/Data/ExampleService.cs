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

        public ExampleModel GetModel() => new ExampleModel();
    }
}
