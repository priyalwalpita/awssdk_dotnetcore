using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AWSSDKWebApp.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AWSSDKWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            
            var awsOptions = Configuration.GetAWSOptions();
            LoadAwsConfig(awsOptions.ProfilesLocation, awsOptions.Region.SystemName);
            
            services.AddSingleton<ISecureEnclave, SecureEnclave>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        
        private void LoadAwsConfig(string fileLocation, string region)
        {
            try
            {
                string line;
                int counter = 0;
                System.IO.StreamReader file =
                    new System.IO.StreamReader(fileLocation);
                while ((line = file.ReadLine()) != null)
                {
                    if (counter == 0)
                        Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", line);
                    if (counter == 1)
                        Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", line);
                    counter++;
                }
                Environment.SetEnvironmentVariable("AWS_REGION", region);
                file.Close();
            }
            catch{}
        }
    }
}