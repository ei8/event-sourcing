﻿using org.neurul.Common.Domain.Model;
using org.neurul.Common.Events;
using works.ei8.EventSourcing.Port.Adapter.Common;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using dmIEventStore = works.ei8.EventSourcing.Domain.Model.IEventStore;

namespace works.ei8.EventSourcing.Port.Adapter.IO.Persistence.Events.SQLite
{
    public class EventStore : dmIEventStore
    {
        public const int EVENTS_PER_LOG = 20;

        public EventStore()
        {
        }

        public async Task<IEnumerable<Notification>> Get(string storeId, Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default(CancellationToken))
        {
            AssertionConcern.AssertArgumentNotNull(storeId, nameof(storeId));
            AssertionConcern.AssertArgumentNotEmpty(storeId, $"'{nameof(storeId)}' cannot be empty.", nameof(storeId));

            var connection = await this.CreateConnection(storeId);

            string id = aggregateId.ToString();
            // When called from CacheRepository.Get<T>, fromVersion is obtained from the AggregateRoot.Version (CQRSlite) value.
            // CacheRepository is trying to obtain only "newer" events and thus the "> fromVersion".
            var query = connection.Table<Notification>().Where(e => e.Id == id && e.Version > fromVersion);
            var results = await query.ToListAsync();
            await this.CloseConnection(connection);
            return results;
        }

        public async Task<NotificationLog> GetLog(string storeId, CancellationToken cancellationToken = default)
        {
            AssertionConcern.AssertArgumentNotNull(storeId, nameof(storeId));
            AssertionConcern.AssertArgumentNotEmpty(storeId, $"'{nameof(storeId)}' cannot be empty.", nameof(storeId));

            var connection = await this.CreateConnection(storeId);
            var totalCount = await connection.Table<Notification>().CountAsync();
            var nli = EventStore.CalculateCurrentNotificationLogId(totalCount);
            return await this.GetLogCore(storeId, nli.NotificationLogId, totalCount, connection);
        }

        public async Task<NotificationLog> GetLog(string storeId, NotificationLogId logId, CancellationToken cancellationToken = default)
        {
            AssertionConcern.AssertArgumentNotNull(storeId, nameof(storeId));
            AssertionConcern.AssertArgumentNotEmpty(storeId, $"'{nameof(storeId)}' cannot be empty.", nameof(storeId));

            var connection = await this.CreateConnection(storeId);
            var totalCount = await connection.Table<Notification>().CountAsync();
            return await this.GetLogCore(storeId, logId, totalCount, connection);
        }

        public async Task<NotificationLog> GetLogCore(string storeId, NotificationLogId logId, int totalCount, SQLiteAsyncConnection connection, CancellationToken cancellationToken = default)
        {
            AssertionConcern.AssertMinimumMaximumValid(logId.Low, logId.High, nameof(logId.Low), nameof(logId.High));
            AssertionConcern.AssertMinimum(logId.Low, 1, nameof(logId.Low));

            var query = connection.Table<Notification>().Where(e => e.SequenceId >= logId.Low && e.SequenceId <= logId.High);
            var results = await query.ToListAsync();
            await this.CloseConnection(connection);

            return await EventStore.CreateNotificationLog(logId, totalCount, results);
        }

        public async Task Save(string storeId, IEnumerable<Notification> notifications, CancellationToken cancellationToken = default(CancellationToken))
        {
            AssertionConcern.AssertArgumentNotNull(storeId, nameof(storeId));
            AssertionConcern.AssertArgumentNotEmpty(storeId, $"'{nameof(storeId)}' cannot be empty.", nameof(storeId));

            var connection = await this.CreateConnection(storeId);
            await connection.RunInTransactionAsync(c => c.InsertAll(notifications));
            await this.CloseConnection(connection);
        }

        private async Task<SQLiteAsyncConnection> CreateConnection(string storeId)
        {
            SQLiteAsyncConnection result = null;
            string databasePath = string.Format(Environment.GetEnvironmentVariable(EnvironmentVariableKeys.DatabasePath), storeId);

            if (!databasePath.Contains(":memory:"))
                AssertionConcern.AssertPathValid(databasePath, nameof(databasePath));

            result = new SQLiteAsyncConnection(databasePath);
            await result.CreateTableAsync<Notification>();
            return result;
        }

        private async Task CloseConnection(SQLiteAsyncConnection connection)
        {
            await connection.CloseAsync();
            connection = null; 
            GC.Collect(); 
            GC.WaitForPendingFinalizers();
        }

        private static async Task<NotificationLog> CreateNotificationLog(NotificationLogId notificationLogId, long notificationCount, IEnumerable<Notification> notificationList)
        {
            AssertionConcern.AssertArgumentValid<long>(
                l => (notificationLogId.High % EVENTS_PER_LOG) == 0,
                notificationLogId.High,
                $"LogId 'High' value must be divisible by '{EVENTS_PER_LOG}'",
                nameof(notificationLogId)
                );
            AssertionConcern.AssertArgumentValid<long>(
                l => (notificationLogId.Low - 1 == 0) || ((notificationLogId.Low - 1) % EVENTS_PER_LOG) == 0,
                notificationLogId.Low,
                $"LogId 'Low' value must be equal to 1 or, 1 plus a number divisible by '{EVENTS_PER_LOG}'",
                nameof(notificationLogId)
                );

            return await EventStore.CreateNotificationLog(new NotificationLogInfo(notificationLogId, notificationCount), notificationCount, notificationList);
        }

        private static NotificationLogInfo CalculateCurrentNotificationLogId(long notificationCount)
        {
            long low, high;
            if (notificationCount > 0)
            {
                var remainder = notificationCount % EVENTS_PER_LOG;
                if (remainder == 0)
                {
                    remainder = EVENTS_PER_LOG;
                }
                low = notificationCount - remainder + 1;
                high = low + EVENTS_PER_LOG - 1;
            }
            else
                low = high = 0;

            return new NotificationLogInfo(new NotificationLogId(low, high), notificationCount);
        }

        private static async Task<NotificationLog> CreateNotificationLog(NotificationLogInfo notificationLogInfo, long notificationCount, IEnumerable<Notification> notificationList)
        {
            NotificationLogId first = null, next = null, previous = null;
            var isArchived = false;
            if (notificationLogInfo.TotalLogged > 0)
            {
                first = notificationLogInfo.NotificationLogId.First(EVENTS_PER_LOG, notificationLogInfo.TotalLogged);
                next = notificationLogInfo.NotificationLogId.Next(EVENTS_PER_LOG, notificationLogInfo.TotalLogged);
                previous = notificationLogInfo.NotificationLogId.Previous(EVENTS_PER_LOG, notificationLogInfo.TotalLogged);
                var currentLog = EventStore.CalculateCurrentNotificationLogId(notificationCount);
                isArchived =
                    notificationLogInfo.NotificationLogId.High < currentLog.NotificationLogId.Low ||
                    (
                        notificationLogInfo.TotalLogged >= EVENTS_PER_LOG &&
                        notificationLogInfo.TotalLogged == notificationLogInfo.NotificationLogId.High &&
                        notificationLogInfo.NotificationLogId.High == currentLog.NotificationLogId.High
                    );
            }

            return new NotificationLog(
                new NotificationLogId(notificationLogInfo.NotificationLogId),
                first,
                next,
                previous,
                notificationList,
                isArchived,
                (int) notificationCount
                );
        }
    }
}
