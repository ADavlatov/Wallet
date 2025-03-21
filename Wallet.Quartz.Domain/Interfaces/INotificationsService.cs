﻿using Wallet.Quartz.Domain.Entities;

namespace Wallet.Quartz.Domain.Interfaces;

public interface INotificationsService
{
    Task<Notification> ScheduleNotification(Guid id, long telegramUserId, string name, string description,
        DateTime notificationTime, CancellationToken cancellationToken);
}