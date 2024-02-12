using FsmWorkFlowUI.Data;

namespace FsmWorkFlowUI
{
    /// <summary>
    /// Used to put all the application specific service
    /// injections into one place.
    /// </summary>
    public static class ServiceInjectionExptensions
    {
        public static IServiceCollection AddCustomService(this IServiceCollection services, IConfiguration configuration)
        {
            string? useMocks = configuration["test:servicemocks"];
            if(string.IsNullOrEmpty(useMocks))
            {
                services.AddSingleton<ExampleService>();
                services.AddSingleton<IAuthService>(new AuthService());
            }
            return services;
        }
    }
}
