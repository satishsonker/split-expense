using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
// Add this class in your project
namespace SplitExpense.Middleware
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var formFileParams = context.ApiDescription.ActionDescriptor.Parameters
                .Where(p => p.ParameterType == typeof(IFormFile) ||
                           (p.ParameterType.IsGenericType && p.ParameterType.GetGenericTypeDefinition() == typeof(List<>) &&
                            p.ParameterType.GetGenericArguments()[0] == typeof(IFormFile)));

            if (!formFileParams.Any()) return;

            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = context.ApiDescription.ParameterDescriptions
                                .ToDictionary(
                                    x => x.Name,
                                    x => x.Type == typeof(IFormFile) ?
                                        new OpenApiSchema { Type = "string", Format = "binary" } :
                                        context.SchemaGenerator.GenerateSchema(x.Type, context.SchemaRepository)
                                )
                        }
                    }
                }
            };
        }
    }
}
