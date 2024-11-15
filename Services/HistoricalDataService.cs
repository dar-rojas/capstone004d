using Api.Database;
using Api.Models;
using Api.Utils.DTOs;
using MongoDB.Driver;

namespace Api.Services;

public class HistoricalDataService
{
    private readonly IMongoDatabase _database;

    public HistoricalDataService()
    {
        _database = DBConnection.Instance.GetDatabase();
    }

    public async Task<List<HistoricalData>> AddHistoricalDataAsync(HistoricalDataRequest historicalDataRequest)
    {
        // Validate that the GridId exists
        var gridCollection = _database.GetCollection<Grid>("grid");
        var grid = await gridCollection.Find(g => g.Id == historicalDataRequest.GridId).FirstOrDefaultAsync();
        if (grid == null)
        {
            throw new ArgumentException($"Grid with Id {historicalDataRequest.GridId} not found.");
        }

        // Validate the length of Timestamps and DemandPowers lists
        if (historicalDataRequest.Timestamps.Count != historicalDataRequest.DemandPowers.Count)
        {
            throw new ArgumentException("The number of timestamps and demand powers must match.");
        }

        // Create a list of HistoricalData objects to insert
        var historicalDataEntries = historicalDataRequest.Timestamps.Select((timestamp, index) => new HistoricalData
        {
            Timestamp = timestamp,
            GridId = historicalDataRequest.GridId,
            DemandPower = historicalDataRequest.DemandPowers[index]
        }).ToList();

        // Insert all entries into the database
        var historicalDataCollection = _database.GetCollection<HistoricalData>("historicalData");
        await historicalDataCollection.InsertManyAsync(historicalDataEntries);
        return historicalDataEntries;
    }

    // gets the more recent historical data
    public async Task<List<HistoricalData>> GetLastHistoricalDataAsync(string gridId, int limit) {
        
        var historicalDataCollection = _database.GetCollection<HistoricalData>("historicalData");
        
        var filter = Builders<HistoricalData>.Filter.Eq(h => h.GridId, gridId); // filter to match gridId

        var historicalDataList = await historicalDataCollection
            .Find(filter)
            .SortByDescending(h => h.Timestamp) // most recent data
            .Limit(limit)
            .ToListAsync();
        
        // Validates that grid historical data exists
        if (historicalDataList.Count == 0)
        {
            throw new ArgumentException($"Grid with Id {gridId} has no historical data.");
        }

        return historicalDataList;
    }
}