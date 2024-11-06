using MongoDB.Driver;
using Api.Models;
using Api.Utils.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

var mongoConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var mongoClient = new MongoClient(mongoConnectionString);
var database = mongoClient.GetDatabase(builder.Configuration.GetConnectionString("Database"));

var app = builder.Build();

app.MapPost("/add_grid", async (HttpRequest request) => {
    try{
        var newGrid = await request.ReadFromJsonAsync<Grid>();
        // validates grid values
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(newGrid);

        if (!Validator.TryValidateObject(newGrid, validationContext, validationResults, true))
        {
            var errors = validationResults.Select(vr => new
            {
                Property = vr.MemberNames.FirstOrDefault(),
                Error = vr.ErrorMessage
            });
            return Results.BadRequest(new { Errors = errors });
        }

        // saves grid and return the new object
        var collection = database.GetCollection<Grid>("grid");
        await collection.InsertOneAsync(newGrid);
        return Results.Created($"/grid/{newGrid.Id}", newGrid);
    }
    catch (JsonException ex)
    {   
        // gets the property of the error
        var match = Regex.Match(ex.Message, @"Path:\s*\$\.(\w+)");
        var property = match.Success ? match.Groups[1].Value : "Grid";
        // creates a list with the error (it is a list for formatting reasons)
        var errors = new List<object>
        { 
            new { Property = property, Error = ex.Message.Split(". Path:")[0] }
        };

        return Results.BadRequest(new { Errors = errors });
    }
});

app.MapPost("/add_historical_data", async (HistoricalDataRequest historicalDataRequest) => 
{
    // Validate that the GridId exists
    var gridCollection = database.GetCollection<Grid>("grid");
    var grid = await gridCollection.Find(g => g.Id == historicalDataRequest.GridId).FirstOrDefaultAsync();
    if (grid == null)
    {
        return Results.NotFound($"Grid with Id {historicalDataRequest.GridId} not found.");
    }

    // Validate the length of Timestamps and DemandPowers lists
    if (historicalDataRequest.Timestamps.Count != historicalDataRequest.DemandPowers.Count)
    {
        return Results.BadRequest("The number of timestamps and demand powers must match.");
    }

    // Create a list of HistoricalData objects to insert
    var historicalDataEntries = historicalDataRequest.Timestamps.Select((timestamp, index) => new HistoricalData
    {
        Timestamp = timestamp,
        GridId = historicalDataRequest.GridId,
        DemandPower = historicalDataRequest.DemandPowers[index]
    }).ToList();

    // Insert all entries into the database
    var historicalDataCollection = database.GetCollection<HistoricalData>("historicalData");
    await historicalDataCollection.InsertManyAsync(historicalDataEntries);
    
    return Results.Created($"/historicalData", historicalDataEntries);
});

app.Run();