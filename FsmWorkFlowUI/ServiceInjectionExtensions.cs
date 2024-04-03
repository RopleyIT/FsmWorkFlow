using FsmWorkFlowUI.Data;

namespace FsmWorkFlowUI
{
    /// <summary>
    /// Used to put all the application specific service
    /// injections into one place.
    /// </summary>
    public static class ServiceInjectionExtensions
    {
        public static IServiceCollection AddCustomServices
            (this IServiceCollection services, IConfiguration configuration)
        {
            string? useMocks = configuration["test:servicemocks"];
            if(string.IsNullOrEmpty(useMocks))
            {
                services.AddScoped<ExampleService>();
                services.AddScoped<IAuthService, AuthService>();
            }
            else
            {
                // Inject mock services here, using the configuraton
                // data to select or setup the mocks
            }
            return services;
        }
    }
}
