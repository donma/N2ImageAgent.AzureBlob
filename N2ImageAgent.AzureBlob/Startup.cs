using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace N2ImageAgent.AzureBlob
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static string ServerToekn { get; set; }
        public static string BlobName { get; set; }
        public static string AzureStorageConnectionString { get; set; }
        public static int UserTokenLifeSeconds { get; set; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddRazorPagesOptions(
                               opts => opts.Conventions.AddPageRoute("/source", "source/{id?}")
                                                       .AddPageRoute("/image", "image/{id?}/{w?}/{h?}")
                                                        .AddPageRoute("/info", "info/{id?}")

           );

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            ServerToekn = Configuration.GetValue<string>("uploadtoken");
            BlobName = Configuration.GetValue<string>("bloname");
            AzureStorageConnectionString = Configuration.GetValue<string>("a9connesctionstring");

            var tmpS = 90;
            int.TryParse(Configuration.GetValue<string>("user_token_life_seconds"), out tmpS);

            UserTokenLifeSeconds = tmpS;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(options);
            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
