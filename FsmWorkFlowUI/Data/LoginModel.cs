namespace FsmWorkFlowUI.Data
{
    /// <summary>
    /// Holds the data for the example login workflow
    /// </summary>
    public class LoginModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public long AuthToken { get; set; } = 0;
    }
}
