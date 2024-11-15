using Api.Database;
using Api.Models;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations;

namespace Api.Services;

public class GridService
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<Grid> _collection;

    public GridService()
    {
        _database = DBConnection.Instance.GetDatabase();
        _collection = _database.GetCollection<Grid>("grid");
    }

    public async Task<Grid> AddGridAsync(Grid grid)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(grid);

        if (!Validator.TryValidateObject(grid, validationContext, validationResults, true))
        {
            var error = string.Join(", ", validationResults.Select(vr =>
                $"{vr.MemberNames.FirstOrDefault()}: {vr.ErrorMessage}"
            ));
            throw new ArgumentException(error);
        }

        // saves grid and return the new object
        await _collection.InsertOneAsync(grid);
        return grid;
    }

    public async Task<Grid> GetGridAsync(string gridId)
    {
        var gridFilter = Builders<Grid>.Filter.Eq(g => g.Id, gridId);
        var grid = await _collection.Find(gridFilter).FirstOrDefaultAsync();
        // Validate that the GridId exists
        if (grid == null)
        {
            throw new ArgumentException($"Grid with id '{gridId}' doesn't exists");
        }
        return grid;
    }

    public async Task<Grid> UpdateGridAsync(Grid grid)
    {
        var gridFilter = Builders<Grid>.Filter.Eq(g => g.Id, grid.Id);
        await _collection.ReplaceOneAsync(gridFilter, grid);
        return grid;
    }
}
