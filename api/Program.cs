using api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//IConfiguration Initialization
var provider = builder.Services.BuildServiceProvider();
var configuration = provider.GetRequiredService<IConfiguration>();
string[] origins = {"https://localhost:4200","http://localhost:4200"};
//////////////////////////////////////////////

//Database Initialization
builder.Services.AddDbContext<DataContext>(options => {
    options.UseSqlite(configuration.GetConnectionString("DevConnection"));
});
///////////////////////////////////////////////

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

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins(origins));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
