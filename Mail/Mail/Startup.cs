using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mail.Data;
using Mail.Filters;
using Mail.Helper;
using Mail.Services;

namespace SampleApp
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
            services.AddControllers();

            #region snippet_AddRazorPages
            services.AddRazorPages()
                .AddRazorPagesOptions(options =>
                    {
                        options.Conventions
                            .AddPageApplicationModelConvention("/StreamedSingleFileUploadDb",
                                model =>
                                {
                                    model.Filters.Add(
                                        new GenerateAntiforgeryTokenCookieAttribute());
                                    model.Filters.Add(
                                        new DisableFormValueModelBindingAttribute());
                                });
                        options.Conventions
                            .AddPageApplicationModelConvention("/StreamedSingleFileUploadPhysical",
                                model =>
                                {
                                    model.Filters.Add(
                                        new GenerateAntiforgeryTokenCookieAttribute());
                                    model.Filters.Add(
                                        new DisableFormValueModelBindingAttribute());
                                });
                    });
            #endregion
            services.AddSingleton<IAppSetting, AppSetting>();
            services.AddSingleton<IAccountFileProvider, AccountFileProvider>();
            services.AddSingleton<IBalanceFileProvider, BalanceFileProvider>();
            services.AddSingleton<IMailFileProvider, MailFileProvider>();
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMemoryDb"));
            services.AddSingleton<IExchangeSendMailError, ExchangeSendMailError>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
