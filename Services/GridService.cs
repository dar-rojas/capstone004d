using Api.Database;
using Api.Models;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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
        ValidateGrid(grid);
        // saves grid and return the new object
        await _collection.InsertOneAsync(grid);
        return grid;
    }

    public async Task<Grid> GetGridAsync(string gridId)
    {
        // verify if gridId is a 24 digit hex string
        CheckIdFormat(gridId);
        //gets grid from db
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
        ValidateGrid(grid);
        await GetGridAsync(grid.Id);
        var gridFilter = Builders<Grid>.Filter.Eq(g => g.Id, grid.Id);
        await _collection.ReplaceOneAsync(gridFilter, grid);
        return grid;
    }

    public void CheckIdFormat(string gridId)
    {
        // verify if gridId is a 24 digit hex string
        string hex24_pattern = @"^[a-fA-F0-9]{24}$";
        if (!Regex.IsMatch(gridId, hex24_pattern))
        {
            throw new ArgumentException("gridId must be a 24 digit hex string");
        }
    }

    private static void ValidateGrid(Grid grid)
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
    }
}
