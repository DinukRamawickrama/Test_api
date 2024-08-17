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



builder.Services.AddDbContext<AppDbContext>(dbOptions => dbOptions.UseMySQL("Server = sqlserver.mysql.database.azure.com; Database = webapp; User Id = dkr; Password = SGxd2544; "));


var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
