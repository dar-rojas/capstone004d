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
            try
            {
                var newGrid = await request.ReadFromJsonAsync<Grid>();
                var grid = await gridService.AddGridAsync(newGrid);
                return Results.Created($"/grid/{grid.Id}", grid);
            }
            catch (JsonException ex)
            {   
                // gets the property of the error
                var match = Regex.Match(ex.Message, @"Path:\s*\$\.(\w+)");
                var property = match.Success ? match.Groups[1].Value : "Grid";
                // creates a new error
                var errors = new 
                { 
                    Property = property, Error = ex.Message.Split(". Path:")[0]
                };
                throw new ArgumentException(errors.ToString());
            }
        });
    }
}