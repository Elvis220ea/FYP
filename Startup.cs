using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FYP
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services
               .AddAuthentication("SingRoom")
               .AddCookie("SingRoom",
                        options =>
                        {
                            options.LoginPath = "/SRAccount/Login/";
                            options.AccessDeniedPath = "/SRAccount/Forbidden/";
                        });


            services
               .AddAuthentication("PetHotel")
               .AddCookie("PetHotel",
                        options =>
                        {
                            options.LoginPath = "/PHAccount/Login/";
                            options.AccessDeniedPath = "/PHAccount/Forbidden/";
                        });



        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
