using Api.Models;
using Api.Utils.DTOs;
using Api.Optimize;
using MongoDB.Driver;
using Api.Database;
using MongoDB.Bson;

namespace Api.Services;

public class OptimizedDataService 
{
    
    private readonly IMongoDatabase _database;
    private readonly GridService _gridService;
    private readonly HistoricalDataService _historicalDataService;

    private readonly IMongoCollection<OptimizedData> _collection;

    public OptimizedDataService()
    {
        _database = DBConnection.Instance.GetDatabase();
        _gridService = new GridService();
        _historicalDataService = new HistoricalDataService();
        _collection = _database.GetCollection<OptimizedData>("optimizedData");
    }

    public async Task<OptimizedData[]> OptimizeDataAsync(string gridId)
    {
        // gets grid by id
        Grid grid = await _gridService.GetGridAsync(gridId);

        // gets latest historical data by gridId
        List<HistoricalData> historicalDataList= await _historicalDataService.GetLastHistoricalDataAsync(gridId, 192);
        
        // optimization
        var optimizeAdapter = new OptimizeAdapter();
        var output = new OptimizationOutput();
        optimizeAdapter.Optimize(grid, historicalDataList, out output);

        // updates grid variables
        grid.MaxPower1 = output.MaxPower1 > grid.MaxPower1 ? output.MaxPower1 : grid.MaxPower1;
        grid.MaxPower2 = output.MaxPower2 > grid.MaxPower2 ? output.MaxPower2 : grid.MaxPower2;
        grid.MonthMaxPowerPH = output.MaxPowerPH > grid.MonthMaxPowerPH ? output.MaxPowerPH : grid.MonthMaxPowerPH;
        grid.InitialEnergy = output.BatteryEnergy[0];
        grid = await _gridService.UpdateGridAsync(grid);

        // gets optimized data to save
        var optimizedDataList = output.BatteryPowers.Select((batteryPower, index) => new OptimizedData
        {
                BatteryPower = batteryPower,
                BatteryEnergy = output.BatteryEnergy[index],
                Timestamp = historicalDataList[index].Timestamp,
                GridId = grid.Id
        }).ToList();
        // saves optimized data
        var optimizedDataArray = await UpsertOptimizedDataAsync(optimizedDataList);
        return optimizedDataArray;
    }

    public async Task<OptimizedData[]> UpsertOptimizedDataAsync(List<OptimizedData> optimizedDataList)
    {
        var upsertTask = optimizedDataList.Select(async newOptimizedData =>
        {   
            // Timestamp and grid Id filter
            var filter = Builders<OptimizedData>.Filter.And(
                Builders<OptimizedData>.Filter.Eq(op => op.Timestamp, newOptimizedData.Timestamp),
                Builders<OptimizedData>.Filter.Eq(op => op.GridId, newOptimizedData.GridId)
            );
            var oldOptmizedData = await _collection.Find(filter).FirstOrDefaultAsync();
            // Asign id
            newOptimizedData.Id = oldOptmizedData == null ? ObjectId.GenerateNewId().ToString() : oldOptmizedData.Id;
            // update or insert
            await _collection.ReplaceOneAsync(filter, newOptimizedData, new ReplaceOptions { IsUpsert = true });
            return newOptimizedData;
        });

        var optimizedDataArray = await Task.WhenAll(upsertTask); 
        return optimizedDataArray;
    }
}