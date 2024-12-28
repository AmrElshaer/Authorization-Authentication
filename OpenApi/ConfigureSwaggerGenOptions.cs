using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Authorization.OpenApi;

public class ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider)
    : IConfigureNamedOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            var openApiInfo = new OpenApiInfo
            {
                Title = $"Authorization.Api v{description.ApiVersion}",
                Version = description.ApiVersion.ToString()
            };
            options.SwaggerDoc(description.GroupName,openApiInfo);
        }
    }

    public void Configure(string? name, SwaggerGenOptions options)
    {
       Configure(options);
    }
}