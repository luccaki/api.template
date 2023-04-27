using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Api.Template.ApplicationCore.Helpers;
using Api.Template.ApplicationCore.Interfaces.Repositories;
using Api.Template.ApplicationCore.Interfaces.Services;
using Api.Template.ApplicationCore.Services;
using Api.Template.Infrastructure.Repositories;
using Api.Template.Presentation.Filters;

namespace Api.Template.Presentation
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(env.ContentRootPath)
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                 .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                 .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddApiVersioning(o =>
            {
                o.ReportApiVersions = true;
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen(c =>
            {
                var provider = services.BuildServiceProvider();
                var service = provider.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (ApiVersionDescription description in service.ApiVersionDescriptions)
                {
                    c.SwaggerDoc(description.GroupName, new OpenApiInfo
                    {
                        Title = "Template Api",
                        Version = description.ApiVersion.ToString()
                    });
                }

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
                c.CustomSchemaIds(x => x.FullName);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Bearer {token}",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        }, new List<string>()
                    }
                });
            });

            services.AddSwaggerGenNewtonsoftSupport();

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidatorActionFilter));
                options.OutputFormatters.RemoveType<StringOutputFormatter>();
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddJsonOptions(options => options.SerializerSettings.Converters.Add(new StringEnumConverter()));

            Config.ConnectionString = Configuration.GetConnectionString("DataBaseConnection");
            Config.DefaultPageSize = Configuration.GetValue<int>("DefaultPageSize");
            Config.KeyVaultClientID = Configuration.GetValue<string>("KeyVault:ClientID");
            Config.KeyVaultClientSecret = Configuration.GetValue<string>("KeyVault:ClientSecret");
            Config.SomeExternalAPI = Configuration.GetValue<string>("APIs:SomeExternalAPI");

            services.AddTransient<ITemplateService, TemplateService>();
            services.AddTransient<ITemplateRepository, TemplateRepository>();

            //Azure KeyVault for SQL Column Encryption
            //// Initialize AKV provider
            //SqlColumnEncryptionAzureKeyVaultProvider sqlColumnEncryptionAzureKeyVaultProvider = new SqlColumnEncryptionAzureKeyVaultProvider(AzureActiveDirectoryAuthenticationCallback);
            //
            //// Register AKV provider
            //SqlConnection.RegisterColumnEncryptionKeyStoreProviders(
            //    customProviders: new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>(
            //    capacity: 1,
            //    comparer: StringComparer.OrdinalIgnoreCase)
            //{
            //    { SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, sqlColumnEncryptionAzureKeyVaultProvider }
            //});
        }

        private static async Task<string> AzureActiveDirectoryAuthenticationCallback(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            ClientCredential clientCred = new ClientCredential(Config.KeyVaultClientID, Config.KeyVaultClientSecret);
            AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);
            if (result == null)
            {
                throw new InvalidOperationException($"Failed to retrieve an access token for {resource}");
            }

            return result.AccessToken;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSentryTracing();

            //app.UseMiddleware<ApiExceptionMiddleware>();
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "/swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
                    c.SwaggerEndpoint($"{Configuration.GetValue<string>("VirtualDirectory")}/swagger/{description.GroupName}/swagger.json", "Template - " + description.GroupName.ToUpperInvariant());
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}