using Common;
using Data.Repositories;
using Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services
{
    public class QueuedSmsService : IQueuedSmsService, IScopedDependency
    {
        private readonly IRepository<SmsQueue> _smsQueueRepository;

        public QueuedSmsService(IRepository<SmsQueue> smsQueueRepository)
        {
            _smsQueueRepository = smsQueueRepository;
        }

        public async Task EnqueueAsync(string phone, string text, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(text))
                return;

            var item = new SmsQueue
            {
                Phone = phone.Trim(),
                Text = text.Trim(),
                CreatedAt = DateTime.Now,
                IsSent = false,
                AttemptCount = 0,
                NextAttemptAt = DateTime.Now
            };

            await _smsQueueRepository.AddAsync(item, cancellationToken);
        }
    }
}
