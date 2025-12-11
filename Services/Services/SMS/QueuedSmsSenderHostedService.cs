using Data.Repositories;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services
{
    public class QueuedSmsSenderHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<QueuedSmsSenderHostedService> _logger;

        public QueuedSmsSenderHostedService(IServiceScopeFactory scopeFactory, ILogger<QueuedSmsSenderHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessBatchAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطا در پردازش صف پیامک");
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task ProcessBatchAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRepository<SmsQueue>>();
            var smsService = scope.ServiceProvider.GetRequiredService<ISMSService>();
            var settings = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<Common.ProjectSettings>>().Value?.ProjectSetting;

            if (settings == null || string.IsNullOrWhiteSpace(settings.SMSToken) || string.IsNullOrWhiteSpace(settings.BaseUrl))
                return;

            var items = await repo.Table
                .Where(x => !x.IsSent && (x.NextAttemptAt == null || x.NextAttemptAt <= DateTime.Now))
                .OrderBy(x => x.Id)
                .Take(10)
                .ToListAsync(cancellationToken);

            foreach (var item in items)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    var res = await smsService.SendSMSAsync("3209c1dc-1cae-4823-b24b-7c41fc470019", "https://localhost:7279", item.Phone, item.Text);
                    if (res.IsSuccess)
                    {
                        item.IsSent = true;
                        item.SentAt = DateTime.Now;
                        item.LastError = null;
                    }
                    else
                    {
                        item.LastError = res.Description;
                        item.AttemptCount += 1;
                        item.NextAttemptAt = DateTime.Now.AddMinutes(1);
                    }
                }
                catch (Exception ex)
                {
                    item.LastError = ex.Message;
                    item.AttemptCount += 1;
                    item.NextAttemptAt = DateTime.Now.AddMinutes(1);
                    _logger.LogError(ex, "خطا در ارسال پیامک به {Phone}", item.Phone);
                }

                await repo.UpdateAsync(item, cancellationToken);
            }
        }
    }
}
