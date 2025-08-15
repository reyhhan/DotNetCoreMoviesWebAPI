using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Movies.Api.Auth;
using Movies.Api.Endpoints;
using Movies.Api.Health;
using Movies.Api.Mapping;
using Movies.Api.Swagger;
using Movies.Application;
using Movies.Application.Database;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x=>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        ValidateIssuer = true,
        ValidateAudience = true
    };
});
builder.Services.AddAuthorization(x =>
{
    //x.AddPolicy(AuthConstants.AdminUserPolicyName, policy =>
    //{
    //    policy.RequireClaim(AuthConstants.AdminUserClaimName, "true");
    //});
    x.AddPolicy(AuthConstants.AdminUserPolicyName,
       p => p.AddRequirements(new AdminAuthRequirement(config["ApiKey"]!)));

    x.AddPolicy(AuthConstants.TrustedMemberPolicyName, policy =>
    {
        policy.RequireAssertion(context =>
            context.User.HasClaim(m => m is { Type : AuthConstants.AdminUserClaimName, Value : "true" }) ||
            context.User.HasClaim(m => m is { Type: AuthConstants.TrustedMemberClaimName, Value: "true" }));
    });
});

var connectionString = builder.Configuration.GetSection("Database:ConnectionString").Get<string>();
// Add services to the container.
builder.Services.AddScoped<ApiKeyAuthFilter>();

builder.Services.AddApiVersioning(x =>
{
    x.DefaultApiVersion = new ApiVersion(1.0);
    x.AssumeDefaultVersionWhenUnspecified = true;
    x.ReportApiVersions = true;
    x.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
}).AddApiExplorer();

//builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddResponseCaching();
builder.Services.AddOutputCache(x =>
{
    x.AddBasePolicy(c => c.Cache());
    x.AddPolicy("MovieCache", c =>
        c.Cache()
        .Expire(TimeSpan.FromMinutes(1))
        .SetVaryByQuery(new[] { "title", "year", "sortBy", "page", "pageSize" })
        .Tag("movies"));
});

//builder.Services.AddControllers();
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("Database");

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(x=>x.OperationFilter<SwaggerDefaultValues>());

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddApplication();
builder.Services.AddDatabase(connectionString!);

//builder.Services.AddDatabase(config["Database:ConnectionString"]!);


var app = builder.Build();
app.CreateApiVersionSet();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            x.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName);
        }
    });
}

app.MapHealthChecks("_health");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//app.UseResponseCaching();
//Default - 200OK is cache, Only GET and HEAD are cached
app.UseOutputCache();

app.UseMiddleware<ValidationMappingMiddleware>();
//app.MapControllers();
app.MapApiEndpoints();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

app.Run();
