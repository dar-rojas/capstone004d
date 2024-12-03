using Api.Services;
using Api.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Api.Endpoints;

public static class GridEndpoints
{
    public static void RegisterGridEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/add_grid", async (HttpRequest request, GridService gridService) => 
        {
            // catch possible json convertion error   
             var newGrid = await request.ReadFromJsonAsync<Grid>();
             var grid = await gridService.AddGridAsync(newGrid);
             return Results.Created($"/grid/{grid.Id}", grid);
        });

        // Get grid by id
        app.MapGet("/get_grid", async (string gridId, GridService gridService) =>
        {
            var grid = await gridService.GetGridAsync(gridId);
            return Results.Ok(grid);
        });

        // Update existing grid
        app.MapPut("update_grid", async (HttpRequest request, GridService gridService) =>
        {
            var newGrid = await request.ReadFromJsonAsync<Grid>();
            var grid = await gridService.UpdateGridAsync(newGrid);
            return Results.Ok(grid);
        });
    }
}