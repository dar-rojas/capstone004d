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
        catch (Exception ex) 
        { 
            context.Response.StatusCode = StatusCodes.Status500InternalServerError; 
            await context.Response.WriteAsJsonAsync(
                new { Error = "An unexpected error occurred.", Details = ex.Message }
                ); 
        }
    }
}