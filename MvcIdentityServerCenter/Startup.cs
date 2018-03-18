using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MvcIdentityServerCenter.Data;
using MvcIdentityServerCenter.Models;
using MvcIdentityServerCenter.Services;
using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.EntityFramework;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

namespace MvcIdentityServerCenter
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

            const string connectionString = "Server=WL000731398;Database=IdentityServerCenterStore;Trusted_Connection=True;MultipleActiveResultSets=true";
            var migtationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            //IdentityServer4.EntityFramework.DbContexts.ConfigurationDbContext
            //IdentityServer4.EntityFramework.DbContexts.PersistedGrantDbContext

            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();


            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                //.AddInMemoryClients(Config.GetClients())
                //.AddInMemoryApiResources(Config.GetApiResources())
                //.AddInMemoryIdentityResources(Config.GetIdentityResources())
                //store scope api ect
                .AddConfigurationStore(options => {
                    options.ConfigureDbContext = builder =>
                    {
                        //init sql server and migration
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migtationAssembly));
                    };
                })
                //store token grant
                .AddOperationalStore(options => {
                    options.ConfigureDbContext = builder =>
                    {
                        //init sql server and migration
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migtationAssembly));
                    };
                })
                .AddAspNetIdentity<ApplicationUser>()
                .Services.AddScoped<IProfileService, ProfileService>();
            //.AddTestUsers(Config.GetTestUsers());

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });

            /*
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.Configure<IdentityOptions>(options => 
            {
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            });
            */
            services.AddScoped<ConsentService>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            InitIdentityServerDataBase(app);

            app.UseStaticFiles();

            //app.UseAuthentication();
            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public void InitIdentityServerDataBase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if (!configurationDbContext.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        configurationDbContext.Clients.Add(client.ToEntity());
                    }

                    configurationDbContext.SaveChanges();
                }

                if (!configurationDbContext.ApiResources.Any())
                {
                    foreach (var api in Config.GetApiResources())
                    {
                        configurationDbContext.ApiResources.Add(api.ToEntity());
                    }

                    configurationDbContext.SaveChanges();
                }

                if (!configurationDbContext.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        configurationDbContext.IdentityResources.Add(resource.ToEntity());
                    }

                    configurationDbContext.SaveChanges();
                }
            }
        }
    }
}
