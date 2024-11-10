using MongoDB.Driver;
using Api.Models;
using Api.Utils.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.RegularExpressions;
using Api.Optimize;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;

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

// Creates a new optimization
app.MapPost("optimize", async (string gridId) =>
{
    // Validate that the GridId exists
    var gridCollection = database.GetCollection<Grid>("grid");
    var gridFilter = Builders<Grid>.Filter.Eq(g => g.Id, gridId);
    var grid = await gridCollection.Find(gridFilter).FirstOrDefaultAsync();
    if (grid == null)
    {
        return Results.NotFound($"Grid with Id {gridId} not found.");
    }

    // Validates that grid historical data exists
    var historicalDataCollection = database.GetCollection<HistoricalData>("historicalData");
    
    var filter = Builders<HistoricalData>.Filter.Eq(h => h.GridId, gridId); // filter to match gridId

    var historicalDataList = await historicalDataCollection
        .Find(filter)
        .SortByDescending(h => h.Timestamp) // most recent data
        .Limit(192) // 48 hours 
        .ToListAsync();  
    if (historicalDataList.Count == 0)
    {
        return Results.NotFound($"Grid with Id {gridId} has no historical data.");
    }
    
    // optimization
    var optimizeAdapter = new OptimizeAdapter();
    var output = new OptimizationOutput();
    optimizeAdapter.Optimize(grid, historicalDataList, out output);
    // updates grid variables
    grid.MonthMaxPower = output.MaxPower > grid.MonthMaxPower ? output.MaxPower : grid.MonthMaxPower;
    grid.MonthMaxPowerPH = output.MaxPowerPH > grid.MonthMaxPowerPH ? output.MaxPowerPH : grid.MonthMaxPower;
    grid.InitialEnergy = output.BatteryEnergy[0];
    var gridUpdate = await gridCollection.ReplaceOneAsync(gridFilter, grid);

    // saves optimized values
    var optimizedCollection = database.GetCollection<OptimizedData>("optimizedData");
    // gets data to save
    var optimizedDataEntries = output.BatteryPowers.Select((batteryPower, index) => new OptimizedData
    {
            BatteryPower = batteryPower,
            BatteryEnergy = output.BatteryEnergy[index],
            Timestamp = historicalDataList[index].Timestamp,
            GridId = grid.Id
    }).ToList();
    // saves data
    var upsertTask = optimizedDataEntries.Select(async newOptimizedData =>
    {   
        // Timestamp and grid Id filter
        var filter = Builders<OptimizedData>.Filter.And(
            Builders<OptimizedData>.Filter.Eq(op => op.Timestamp, newOptimizedData.Timestamp),
            Builders<OptimizedData>.Filter.Eq(op => op.GridId, newOptimizedData.GridId)
        );
        var oldOptmizedData = await optimizedCollection.Find(filter).FirstOrDefaultAsync();
        // Asign id
        newOptimizedData.Id = oldOptmizedData == null ? ObjectId.GenerateNewId().ToString() : oldOptmizedData.Id;
        // update or insert
        await optimizedCollection.ReplaceOneAsync(filter, newOptimizedData, new ReplaceOptions { IsUpsert = true });
        return newOptimizedData;
    }).ToList();

    var upsertedDataEntries = await Task.WhenAll(upsertTask); 
    return Results.Ok(upsertedDataEntries);
});
app.Run();