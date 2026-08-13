using Boilerplate.Api;
using Boilerplate.Api.Common;
using Boilerplate.Api.Endpoints;
using Boilerplate.Application;
using Boilerplate.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApi(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapPost(
        "/dev/token",
        IssueDevToken);
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints(typeof(Program).Assembly);

app.Run();

static IResult IssueDevToken(
    Guid userId,
    IConfiguration configuration)
{
    var token = JwtTokenFactory.CreateToken(
        configuration,
        userId);

    return Results.Ok(new { token });
}
