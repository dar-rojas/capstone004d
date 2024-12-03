using Api.Database;
using Api.Utils;
using Api.Endpoints;
using Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(_ => DBConnection.Instance.GetDatabase());

builder.Services.AddTransient<GridService>();
builder.Services.AddTransient<HistoricalDataService>();
builder.Services.AddTransient<OptimizedDataService>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.RegisterGridEndpoints();
app.RegisterHistoricalDataEndpoints();
app.RegisterOptimizedDataEndpoints();

app.Run();