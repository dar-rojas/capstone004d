using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Api.Utils;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    
    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest; 
            await context.Response.WriteAsJsonAsync(new { Error = ex.Message });
        }
        catch (JsonException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            var Error = ex.Message.Split(". Path:")[0]; // Omit the error path from the output
            await context.Response.WriteAsJsonAsync(new { Error });
        }
        catch (BadHttpRequestException ex)
        {
            context.Response.StatusCode= StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { Error = ex.Message });
        }
        catch (Exception ex) 
        { 
            context.Response.StatusCode = StatusCodes.Status500InternalServerError; 
            await context.Response.WriteAsJsonAsync(
                new { Error = "An unexpected error occurred.", Details = ex.Message }
                ); 
        }
    }
}