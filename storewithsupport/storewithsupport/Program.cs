using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using store.application.Services;
using storewithsupport.Hubs;
using storewithsupport.Mappings;
using storewithsupport.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(optionsBuilder=>optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddSignalR();
builder.Services.AddHostedService<AdminBackgroundService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

//Mapping
app.MapUsers();
app.MapRoles();
app.MapHub<TechSupportHub>("/techSupportHub");
app.MapGet("/ping", () => "pong");
//EndOfMapping


app.Run();

