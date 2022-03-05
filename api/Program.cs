using System.Text;
using api.Data;
using api.Extensions;
using api.Interfaces;
using api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//IConfiguration Initialization
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
string[] origins = {"https://localhost:4200","http://localhost:4200"};
//////////////////////////////////////////////

//DATABASE AND TOKEN INITIALIZATION
builder.Services.AddApplicationServices(configuration);
//////////////////////////////////////////////

//ADD JWT AUTHENTICATION SERVICE
builder.Services.AddIdentityServices(configuration);
////////////////////////////////////////////////

//CORS Initialization
builder.Services.AddCors();
///////////////////////////////////////////////

//Controller Initialization
builder.Services.AddControllers();
///////////////////////////////////////////////

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//EXECUTE CROSS-ORIGIN CHECKING BEFORE AUTHENTICATION
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins(origins));

app.UseHttpsRedirection();

//EXECUTE AUTHENTICATION BEFORE USER AUTHORIZATION
app.UseAuthentication();

//EXECUTE AUTHORIZATION TO DIRECT USERS TO THEIR ALLOWED DESTINATIONS
app.UseAuthorization();

app.MapControllers();

app.Run();
