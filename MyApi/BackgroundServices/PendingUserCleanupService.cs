using Marqelle.Domain.Entities;
using Marqelle.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Marqelle.Api.BackgroundServices
{
    public class PendingUserCleanupService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<PendingUserCleanupService> _logger;

        private readonly TimeSpan _interval = TimeSpan.FromMinutes(10);

        public PendingUserCleanupService(IServiceProvider services, ILogger<PendingUserCleanupService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Pending user cleanup service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_interval, stoppingToken);

                try
                {
                    using var scope = _services.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IGenericRepository<Users>>();

                    var allUsers = await repo.GetAllAsync();

                    var expired = allUsers
                        .Where(u => u.Status == "Pending" && u.OtpExpiry < DateTime.UtcNow)
                        .ToList();

                    foreach (var user in expired)
                        repo.Delete(user);

        
                    var expiredEmailChanges = allUsers
                        .Where(u => u.UpdatingEmail != null && u.OtpExpiry < DateTime.UtcNow)
                        .ToList();

                    foreach (var user in expiredEmailChanges)
                    {
                        user.UpdatingEmail = null;
                        user.OtpCode = null;
                        user.OtpExpiry = null;
                        repo.Update(user);
                    }

                    if (expired.Any() || expiredEmailChanges.Any())
                    {
                        await repo.SaveAsync();
                        _logger.LogInformation($"Cleanup: {expired.Count} expired pending user(s), {expiredEmailChanges.Count} expired email change(s).");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during pending user cleanup.");
                }
            }
        }
    }
}