using Api.Utils.DTOs;
using Api.Services;

namespace Api.Endpoints;

public static class HistoricalDataEndpoints
{
    public static void RegisterHistoricalDataEndpoints(this IEndpointRouteBuilder app)
    {   
        app.MapPost("/add_historical_data", async (HistoricalDataRequest historicalDataRequest, HistoricalDataService historicalDataService) => 
        {
            var historicalDataEntries = await historicalDataService.AddHistoricalDataAsync(historicalDataRequest);
            return Results.Created($"/historicalData", historicalDataEntries);
        });
    }
}