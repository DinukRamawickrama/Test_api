using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using WebAPP;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebAPP.Services;
using Microsoft.AspNetCore.Hosting.Server;
using WebAPP.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("JWTAuth") 
    .AddJwtBearer("JWTAuth" ,option => { }) ;

builder.Services.AddScoped<WebAPP.Services.IUserService, UserService>();



builder.Services.AddDbContext<AppDbContext>(dbOptions => dbOptions.UseMySQL("Server=52.23.160.92;Database=webapp;User Id=dkr;Password=SGxd2544;"));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowAllOrigins",
                      builder =>
                      {
                          builder.AllowAnyOrigin(); // Allows requests from any origin
                          builder.AllowAnyMethod(); // Allows all methods (GET, POST, PUT, DELETE, etc.)
                          builder.AllowAnyHeader(); // Allows all headers
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
