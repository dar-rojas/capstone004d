using Api.Services;
using Api.Models;
using api.Utils.DTOs;

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

        // get optimized data between two datetimes by gridId
        app.MapGet("get_optimized_data", async (HttpRequest request, OptimizedDataService optDataService) =>
        {
            var optDataRequest = await request.ReadFromJsonAsync<OptimizedDataRequest>();
            var optDataList = await optDataService.GetOptimizedDataAsync(optDataRequest);
            return Results.Ok(optDataList);
        });
    }
}