var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

string? apiUsername = Environment.GetEnvironmentVariable("API_USERNAME");
string? apiPassword = Environment.GetEnvironmentVariable("API_PASSWORD");

if (string.IsNullOrWhiteSpace(apiUsername) || string.IsNullOrWhiteSpace(apiPassword))
{
    throw new InvalidOperationException("API_USERNAME and API_PASSWORD environment variables must be set.");
}

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/swagger"))
    {
        await next();
        return;
    }

    string authHeader = context.Request.Headers.Authorization;
    if (authHeader != null && authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
    {
        var encodedUsernamePassword = authHeader["Basic ".Length..].Trim();
        var decodedUsernamePassword = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword));
        var parts = decodedUsernamePassword.Split(':', 2);
        var username = parts[0];
        var password = parts.Length > 1 ? parts[1] : "";

        if (username == apiUsername && password == apiPassword)
        {
            await next();
            return;
        }
    }

    context.Response.Headers.WWWAuthenticate = "Basic";
    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    await context.Response.WriteAsync("Unauthorized");
    return;
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
