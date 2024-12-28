using System.Reflection;
using Authorization.Data;
using Authorization.Features;
using Authorization.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace Authorization;

public static class DependencyInjection
{
    public static  IServiceCollection AddServices(this  IServiceCollection serviceProvider,IConfiguration configuration)
    {
        return serviceProvider.AddAuthentication()
            .AddSystemAuthorization()
            .AddApplicationDbContext(configuration)
            .AddEndpoints(Assembly.GetExecutingAssembly());
    }
    private static IServiceCollection AddEndpoints(this IServiceCollection serviceProvider,
        Assembly assembly)
    {
        var endpoints=assembly.DefinedTypes.Where(type=>type is {IsAbstract:false,IsInterface:false} &&
                                                                        type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type=>ServiceDescriptor.Transient(typeof(IEndpoint),type)).ToArray();
        serviceProvider.TryAddEnumerable(endpoints);
        return serviceProvider;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app,
        RouteGroupBuilder? routeGroupBuilder=null)
    {
        IEnumerable<IEndpoint> endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();
        IEndpointRouteBuilder builder = routeGroupBuilder is null ? app : routeGroupBuilder;
        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }
    private static IServiceCollection AddApplicationDbContext(this IServiceCollection serviceProvider,IConfiguration configuration)
    {
       return serviceProvider.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

    }
    private static IServiceCollection AddSystemAuthorization(this IServiceCollection serviceProvider)
    {
        serviceProvider.AddAuthorization();
        serviceProvider.AddScoped<IClaimsTransformation, CustomClaimsTransformation>(); 
        serviceProvider.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        serviceProvider.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
      
        return serviceProvider;
    }

    private static  IServiceCollection AddAuthentication(this  IServiceCollection serviceProvider)
    {
        serviceProvider
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    var baseAddress = "http://localhost:8080";
                    var realmName = "AuthorizationTest";
                    options.MetadataAddress = $"{baseAddress}/realms/{{realm_name}}/.well-known/openid-configuration";
                    options.RequireHttpsMetadata = false; // only for dev
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                       
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                        {
                            
                            var keyClient = new HttpClient();
                            var response = keyClient.GetStringAsync($"{baseAddress}/realms/{realmName}/protocol/openid-connect/certs").Result;
                        
                            var keys = new JsonWebKeySet(response);
                            return keys.GetSigningKeys(); 
                        },
                        ValidIssuer = $"{baseAddress}/realms/{realmName}",
                        ValidateIssuer = true,
                        ClockSkew = TimeSpan.FromMinutes(5)
                    };
                    options.Authority = baseAddress;
                }
            );
    
        return serviceProvider;

    }
    
}