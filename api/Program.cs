using api.Data;
using api.Entities;
using api.Extensions;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
string[] origins = {"https://localhost:4200", "https://localhost:5001"};

builder.Services.AddApplicationServices(configuration);

builder.Services.AddIdentityServices(configuration);

builder.Services.AddCors();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// using var scope = app.Services.CreateScope();
// var services = scope.ServiceProvider;
// try{
//     var context = services.GetRequiredService<DataContext>();
//     var userManager = services.GetRequiredService<UserManager<AppUser>>();
//     var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
//     await context.Database.MigrateAsync();
//     await Seed.SeedUsers(userManager, roleManager);
// }catch(Exception e){
//     throw e;
// }

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins(origins));

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
