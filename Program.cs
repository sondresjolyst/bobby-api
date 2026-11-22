var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

string? apiUsername = builder.Configuration["API_USERNAME"];
string? apiPassword = builder.Configuration["API_PASSWORD"];

if (string.IsNullOrWhiteSpace(apiUsername) || string.IsNullOrWhiteSpace(apiPassword))
{
    throw new InvalidOperationException("API_USERNAME and API_PASSWORD environment variables must be set.");
}

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/swagger")
        || context.Request.Path.Equals("/health", StringComparison.OrdinalIgnoreCase))
    {
        await next();
        return;
    }

    string? authHeader = context.Request.Headers.Authorization;
    if (authHeader != null && authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
    {
        var encodedUsernamePassword = authHeader["Basic ".Length..].Trim();
        var decodedBytes = new byte[encodedUsernamePassword.Length];
        if (Convert.TryFromBase64String(encodedUsernamePassword, decodedBytes, out var bytesWritten))
        {
            var decodedUsernamePassword = System.Text.Encoding.UTF8.GetString(decodedBytes, 0, bytesWritten);
            var parts = decodedUsernamePassword.Split(':', 2);
            if (parts.Length == 2
                && FixedTimeEquals(parts[0], apiUsername)
                && FixedTimeEquals(parts[1], apiPassword))
            {
                await next();
                return;
            }
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

// Probes reach the pod over plain HTTP and send no X-Forwarded-Proto, so a redirect
// would fail them if an HTTPS port is ever configured.
app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/health"),
    branch => branch.UseHttpsRedirection());
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health").AllowAnonymous();
app.Run();

static bool FixedTimeEquals(string a, string b) =>
    System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
        System.Text.Encoding.UTF8.GetBytes(a),
        System.Text.Encoding.UTF8.GetBytes(b));
