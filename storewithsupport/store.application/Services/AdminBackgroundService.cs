using Core.Models;
using Infrastructure.Context;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace store.application.Services;

public class AdminBackgroundService : IHostedService
{
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdminBackgroundService> _logger;

    public AdminBackgroundService(
        IHostApplicationLifetime appLifetime,
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<AdminBackgroundService> logger)
    {
        _appLifetime = appLifetime;
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _appLifetime.ApplicationStarted.Register(async () => { await DoWorkAsync(cancellationToken); });
    }

    private async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var admins = await dbContext.Users
                .Include(u => u.Role)
                .Where(u => u.Role.Name == "admin")
                .Select(u => new UserConnection { UserName = u.Email, ChatRoom = $"admin-{u.Email}" })
                .ToListAsync(cancellationToken);
            foreach (var admin in admins)
            {
                var connection = new HubConnectionBuilder()
                    .WithUrl(_configuration["SignalR:HubUrl"] ?? "http://localhost:5137/techSupportHub")
                    .Build();

                try
                {
                    await connection.StartAsync(cancellationToken);
                    _logger.LogInformation("Connected to SignalR hub as admin: {User}", admin.UserName);

                    await connection.InvokeAsync("JoinRoom", admin, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error connecting admin {User} to chat", admin.UserName);
                }
            }
            await Task.Delay(60000, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}