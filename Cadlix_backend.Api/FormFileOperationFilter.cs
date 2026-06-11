using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cadlix_backend.Api;

public class FormFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var formFileParams = context.MethodInfo.GetParameters()
            .Where(p => p.ParameterType == typeof(IFormFile))
            .ToList();

        if (formFileParams.Count == 0)
            return;

        var schema = new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Properties = formFileParams.ToDictionary<System.Reflection.ParameterInfo, string, IOpenApiSchema>(
                p => p.Name ?? "file",
                p => new OpenApiSchema { Type = JsonSchemaType.String, Format = "binary" }
            ),
            Required = new HashSet<string>(
                formFileParams
                    .Where(p => p.HasDefaultValue == false)
                    .Select(p => p.Name ?? "file")
            )
        };

        operation.Parameters?.Clear();

        operation.RequestBody = new OpenApiRequestBody
        {
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = schema
                }
            }
        };
    }
}
