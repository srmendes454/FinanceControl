using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using FinanceControl.Application.Extensions.RequestContainer;
using FinanceControl.Extensions.AppSettings;
using FinanceControl.Extensions.BaseEnvironment;
using FinanceControl.WebApi.Extensions.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Enrichers.HttpContextData;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace FinanceControl.Application.Startup;

public class BaseStartup
{
    #region [ Fields ]
    public readonly string _apiName;
    public readonly IConfiguration _configuration;
    public readonly IWebHostEnvironment _env;
    #endregion

    #region [ Constructor ]

    public BaseStartup(IConfiguration configuration, IWebHostEnvironment env, string apiName = "")
    {
        _env = env;
        _apiName = apiName;
        _configuration = configuration;

        if (env.IsProductionEnviroment())
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("System", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithCorrelationId()
                .Enrich.WithHttpContextData()
                .Enrich.WithExceptionStackTraceHash()
                .WriteTo.Console(new CompactJsonFormatter())
                .CreateLogger();
        }
        else
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("System", LogEventLevel.Verbose)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithCorrelationId()
                .Enrich.WithHttpContextData()
                .Enrich.WithExceptionStackTraceHash()
                .WriteTo.Console(new CompactJsonFormatter())
                .CreateLogger();
        }

        Log.Logger.ForContext("environment", env.EnvironmentName)
                  .ForContext("apiName", apiName)
                  .ForContext<BaseStartup>()
                  .Information("Startup");

    }
    #endregion

    #region [ Public Methods ]
    public void BaseConfigure(IApplicationBuilder app)
    {
        if (_env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseAuthentication();
        app.UseRouting();
        app.UseCors(x => x
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        if (_env.IsDevelopmentEnviroment())
        {
            ConfigureSwagger(app);
        }
    }
    private void ConfigureSwagger(IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("./swagger/v1/swagger.json", _apiName);
            c.RoutePrefix = string.Empty;
        });
    }
    public void BaseConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddDbContext<IContextMongoDBDatabase, ContextMongoDBDatabase>(ServiceLifetime.Singleton, ServiceLifetime.Singleton);
        services.AddSingleton<IAppSettings, AppSettings>();

        // Container injetado cotendo os dados de userId
        services.AddScoped<IRequestContainer>(a =>
        {
            var httpContext = services.BuildServiceProvider().GetService<IHttpContextAccessor>();
            if (httpContext.HttpContext.Request.Path.Value.Equals("/v1/user/register") || httpContext.HttpContext.Request.Path.Value.Equals("/v1/user/login"))
            {
                return new RequestContainer();
            }
            else
            {
                var headerAuthorization = httpContext.HttpContext.Request.Headers.FirstOrDefault(p => p.Key.Equals("Authorization")).Value.ToString();
                if (headerAuthorization == null || string.IsNullOrEmpty(headerAuthorization))
                {
                    throw new NotImplementedException("TOKEN_INVALID");
                }

                var jwtToken = new JwtSecurityToken(jwtEncodedString: headerAuthorization.Substring(7));
                var userId = jwtToken.Claims.FirstOrDefault(p => p.Type.ToLower().Equals("primarysid"))?.Value;
                if (userId == null)
                    throw new InvalidOperationException("TOKEN_INVALID");

                var name = jwtToken.Claims.FirstOrDefault(p => p.Type.ToLower().Equals("name"))?.Value;
                if (name == null)
                    throw new InvalidOperationException("TOKEN_INVALID");

                var email = jwtToken.Claims.FirstOrDefault(p => p.Type.ToLower().Equals("email"))?.Value;
                if (email == null)
                    throw new InvalidOperationException("TOKEN_INVALID");

                return new RequestContainer { UserId = Guid.Parse(userId), Email = email, Name = name };
            }
        });

        //---------------------------------------------------------------------------
        services.AddCors();
        services.AddControllers();

        ConfigureServicesSwagger(services);
        ConfigureServicesFilter(services);

        ConfigureServicesAuthentication(services);

        services.Configure<FormOptions>(o =>
        {
            o.ValueLengthLimit = int.MaxValue;
            o.MultipartBodyLengthLimit = int.MaxValue;
            o.MemoryBufferThreshold = int.MaxValue;
        });
    }
    #endregion

    #region [ Private Methods ]
    private void ConfigureServicesSwagger(IServiceCollection services)
    {
        if (_env.IsDevelopmentEnviroment())
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Finance Control",
                });

                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement { { jwtSecurityScheme, Array.Empty<string>() } });

                try
                {
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                }
                catch { }
            });
        }
    }
    private void ConfigureServicesFilter(IServiceCollection services)
    {
        // services.AddSingleton<ApiUserValidateFilter>();
        // services.AddSingleton<UserPermisionFilter>();
    }
    private void ConfigureServicesAuthentication(IServiceCollection services)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]);
        services.AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
                .AddJwtBearer(cfg =>
                {
                    cfg.SaveToken = true;
                    cfg.RequireHttpsMetadata = false;

                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = false,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };

                });
    }
    #endregion
}