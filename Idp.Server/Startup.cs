// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Idp.Server.DbContexts;
using Idp.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Reflection;

namespace Idp.Server
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get;  }

        private const string IdpConnectionStringName = "IdpConnectionString";
        private const string UsersDataConnectionStringName = "UsersDataConnectionString";

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString(IdpConnectionStringName);

            services.AddDbContextPool<IdentityDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString(UsersDataConnectionStringName)));
            services.AddControllersWithViews();

            var builder = services.AddIdentityServer(options =>
            {
                // see https://identityserver4.readthedocs.io/en/latest/topics/resources.html
                options.EmitStaticAudienceClaim = true;
            });

            builder.AddProfileService<ProfileService>();

            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            builder.AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder =>
                {
                    builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationAssembly));
                };
            });

            builder.AddOperationalStore(options =>
            {
                options.ConfigureDbContext = builder =>
                {
                    builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationAssembly));
                };
            });

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();
            services.AddTransient<IUserService, UserService>();
        }

        public void Configure(IApplicationBuilder app, ConfigurationDbContext context)
        {
            InitializeDatabase(app);
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add MVC
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetService<IServiceScopeFactory>().CreateScope();

            serviceScope.ServiceProvider
                .GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

            var context = serviceScope.ServiceProvider
                .GetRequiredService<ConfigurationDbContext>();
            context.Database.Migrate();
            if (!context.Clients.Any())
            {
                foreach (var client in Config.Clients)
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var scopes in Config.ApiScopes)
                {
                    context.ApiScopes.Add(scopes.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in Config.ApiResources)
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            var idpUsers = serviceScope.ServiceProvider
                .GetRequiredService<IdentityDbContext>();

            idpUsers.Database.Migrate();
            if (!idpUsers.Users.Any())
            {
                foreach (var user in Config.Users)
                {
                    idpUsers.Users.Add(user);
                }
                idpUsers.SaveChanges();
            }
        }
    }
}
