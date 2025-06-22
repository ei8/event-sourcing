using ei8.EventSourcing.Application;
using ei8.EventSourcing.Common;
using neurUL.Common.Domain.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using dmIEventStore = ei8.EventSourcing.Domain.Model.IEventStore;

namespace ei8.EventSourcing.Port.Adapter.IO.Persistence.Events.SQLite
{
    public class EventStore : dmIEventStore
    {
        public const int EVENTS_PER_LOG = 20;
        private static readonly IDictionary<string, SQLiteAsyncConnection> connections = new Dictionary<string, SQLiteAsyncConnection>();
        private readonly ISettingsService settingsService;

        // TODO: Use classic approach to enable connection closure
        // https://www.codeproject.com/Tips/1057992/Using-SQLite-An-Example-of-CRUD-Operations-in-Csha
        public EventStore(ISettingsService settingsService)
        {
            AssertionConcern.AssertArgumentNotNull(settingsService, nameof(settingsService));

            this.settingsService = settingsService;
        }

        public async Task<IEnumerable<Notification>> Get(Guid aggregateId, int fromVersion, CancellationToken cancellationToken = default(CancellationToken))
        {
            var connection = await this.GetCreateConnection();

            string id = aggregateId.ToString();
            // When called from CacheRepository.Get<T>, fromVersion is obtained from the AggregateRoot.Version (CQRSlite) value.
            // CacheRepository is trying to obtain only "newer" events and thus the "> fromVersion".
            var query = connection.Table<Notification>().Where(e => e.Id == id && e.Version > fromVersion);
            var results = await query.ToListAsync();
            // TODO: await this.CloseConnection(connection);
            return results;
        }

        public async Task<NotificationLog> GetLog(CancellationToken cancellationToken = default)
        {
            var connection = await this.GetCreateConnection();
            var totalCount = await connection.Table<Notification>().CountAsync();
            var nli = EventStore.CalculateCurrentNotificationLogId(totalCount);
            return await this.GetLogCore(nli.NotificationLogId, totalCount, connection);
        }

        public async Task<NotificationLog> GetLog(NotificationLogId logId, CancellationToken cancellationToken = default)
        {
            var connection = await this.GetCreateConnection();
            var totalCount = await connection.Table<Notification>().CountAsync();
            return await this.GetLogCore(logId, totalCount, connection);
        }

        private async Task<NotificationLog> GetLogCore(NotificationLogId logId, int totalCount, SQLiteAsyncConnection connection, CancellationToken cancellationToken = default)
        {
            AssertionConcern.AssertMinimumMaximumValid(logId.Low, logId.High, nameof(logId.Low), nameof(logId.High));
            AssertionConcern.AssertMinimum(logId.Low, totalCount > 0 ? 1 : 0, nameof(logId.Low));

            var query = connection.Table<Notification>().Where(e => e.SequenceId >= logId.Low && e.SequenceId <= logId.High);
            var results = await query.ToListAsync();
            // TODO: await this.CloseConnection(connection);

            return await EventStore.CreateNotificationLog(logId, totalCount, results);
        }

        public async Task Save(IEnumerable<Notification> notifications, CancellationToken cancellationToken = default(CancellationToken))
        {
            var connection = await this.GetCreateConnection();
            await connection.RunInTransactionAsync(c => c.InsertAll(notifications));
            // TODO: await this.CloseConnection(connection);
        }

        private async Task<SQLiteAsyncConnection> GetCreateConnection()
        {
            if (!EventStore.connections.ContainsKey(this.settingsService.DatabasePath))
            {
                var result = new SQLiteAsyncConnection(this.settingsService.DatabasePath);
                await result.CreateTableAsync<Notification>();
                connections.Add(this.settingsService.DatabasePath, result);
            }

            return EventStore.connections[this.settingsService.DatabasePath];
        }

        // TODO: private async Task CloseConnection(SQLiteAsyncConnection connection)
        //{
        //    await connection.CloseAsync();
        //    connection = null; 
        //    GC.Collect(); 
        //    GC.WaitForPendingFinalizers();
        //}

        public static async Task<NotificationLog> CreateNotificationLog(NotificationLogId notificationLogId, long notificationCount, IEnumerable<Notification> notificationList)
        {
            AssertionConcern.AssertArgumentValid(
                l => (l % EVENTS_PER_LOG) == 0,
                notificationLogId.High,
                $"LogId 'High' value must be divisible by '{EVENTS_PER_LOG}'",
                nameof(notificationLogId)
                );
            AssertionConcern.AssertArgumentValid(
                l => (notificationCount == 0 && l == 0) || ((l - 1 == 0) || ((l - 1) % EVENTS_PER_LOG) == 0),
                notificationLogId.Low,
                $"LogId 'Low' value must be equal to 1 or, 1 plus a number divisible by '{EVENTS_PER_LOG}'",
                nameof(notificationLogId)
                );

            return await EventStore.CreateNotificationLog(new NotificationLogInfo(notificationLogId, notificationCount), notificationCount, notificationList);
        }

        public static NotificationLogInfo CalculateCurrentNotificationLogId(long notificationCount)
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

        public static async Task<NotificationLog> CreateNotificationLog(NotificationLogInfo notificationLogInfo, long notificationCount, IEnumerable<Notification> notificationList)
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
