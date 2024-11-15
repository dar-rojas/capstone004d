using Api.Services;
using Api.Models;

namespace Api.Endpoints;

public static class OptimizedDataEndpoints
{
    public static void RegisterOptimizedDataEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("optimize", async (string gridId, OptimizedDataService optimizedDataService) =>
        {
            var optimizedDataArray = await optimizedDataService.OptimizeDataAsync(gridId);
            return Results.Created($"/optimizedData", optimizedDataArray);
        });
    }
}