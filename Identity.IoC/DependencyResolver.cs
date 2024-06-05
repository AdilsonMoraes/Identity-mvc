using Identity.Data;
using Identity.Services.Authentication;
using Identity.Services.Common;
using Identity.Services.Log;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.IoC
{
    public static class DependencyResolver
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<ILogService, LogService>();
            services.AddTransient<IEmailSender, EmailSender>();
        }

        public static void RegisterDB(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<IdentityContext>(option =>
            {
                option.UseSqlServer(connectionString);
            });
        }
    }
}
