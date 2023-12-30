using Authentication.Api.Data;
using Authentication.Api.Services;
using Authentication.Api.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database
builder.Services.AddDbContext<AccountDbContext>(options =>
{
#pragma warning disable CS8604 // Possible null reference argument.
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"));
#pragma warning restore CS8604 // Possible null reference argument.
});

// Configure Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 7;

}).AddEntityFrameworkStores<AccountDbContext>()
    .AddDefaultTokenProviders();

// Configure authentication.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
#pragma warning disable CS8604 // Possible null reference argument.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration.GetValue<string>("AuthSettings:Audience"),
        ValidIssuer = builder.Configuration.GetValue<string>("AuthSettings:Issuer"),
        RequireExpirationTime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("AuthSettings:Key"))),
        ValidateIssuerSigningKey = true,
    };
#pragma warning restore CS8604 // Possible null reference argument.
});

// Configure services.
builder.Services.AddScoped<IUserService, UserService>();

// Configure CORS.
string defaultCorsPolicy = "_defaultCorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: defaultCorsPolicy,
        policy =>
        {
            policy.WithOrigins("http://localhost:5084").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseCors(defaultCorsPolicy);

app.Run();
