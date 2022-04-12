using api.Extensions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//IConfiguration Initialization
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
string[] origins = {"https://localhost:4200", "https://localhost:5001"};
//////////////////////////////////////////////

//DATABASE AND TOKEN INITIALIZATION
builder.Services.AddApplicationServices(configuration);
//////////////////////////////////////////////

//ADD JWT AUTHENTICATION SERVICE
builder.Services.AddIdentityServices(configuration);
////////////////////////////////////////////////

// ADD SESSION

//CORS Initialization
builder.Services.AddCors();
///////////////////////////////////////////////

//Controller Initialization
builder.Services.AddControllers();
///////////////////////////////////////////////

/*
builder.Services.AddDataProtection()
.UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
{
    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM,
    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
});*/

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


// GRAB JWT IF FOUND IN SESSION
/*app.Use(async (context, next) =>
{
    var JWToken = context.Session.GetString("XSRF_Auth");
    if (!String.IsNullOrEmpty(JWToken))
    {
        context.Request.Headers.Add("Authorization", "Bearer " + JWToken);
    }
    await next();
});*/

//EXECUTE AUTHENTICATION BEFORE USER AUTHORIZATION
app.UseAuthentication();

//EXECUTE AUTHORIZATION TO DIRECT USERS TO THEIR ALLOWED DESTINATIONS
app.UseAuthorization();

app.MapControllers();

app.Run();
