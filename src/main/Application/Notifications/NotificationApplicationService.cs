// Copyright 2012,2013 Vaughn Vernon
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Modifications copyright(C) 2017 ei8/Elmer Bool

using org.neurul.Common.Domain.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.EventSourcing.Common;
using dmIEventStore = works.ei8.EventSourcing.Domain.Model.IEventStore;

namespace works.ei8.EventSourcing.Application.Notifications
{
    public class NotificationApplicationService : INotificationApplicationService
    {
        public NotificationApplicationService(dmIEventStore eventStore)
        {
            this.eventStore = eventStore;
        }

        private readonly dmIEventStore eventStore;
        
        public async Task<NotificationLog> GetCurrentNotificationLog(CancellationToken cancellationToken = default)
        {
            return await this.eventStore.GetLog(cancellationToken); 
        }

        public async Task<NotificationLog> GetNotificationLog(string notificationLogId, CancellationToken cancellationToken = default)
        {
            if (!NotificationLogId.TryParse(notificationLogId, out NotificationLogId logId))
                throw new FormatException($"Specified {nameof(notificationLogId)} value of '{notificationLogId}' was not in the expected format.");

            return await this.eventStore.GetLog(logId, cancellationToken);
        }
    }
}
