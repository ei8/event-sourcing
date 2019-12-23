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
// Modifications copyright(C) 2017 ei8.works/Elmer Bool

using works.ei8.EventSourcing.Common;

namespace works.ei8.EventSourcing.Port.Adapter.IO.Persistence.Events.SQLite
{
    public class NotificationLogInfo
    {
        public NotificationLogInfo(NotificationLogId notificationLogId, long totalLogged)
        {
            this.notificationLogId = notificationLogId;
            this.totalLogged = totalLogged;
        }

        readonly NotificationLogId notificationLogId;

        public NotificationLogId NotificationLogId
        {
            get { return notificationLogId; }
        }

        readonly long totalLogged;

        public long TotalLogged
        {
            get { return totalLogged; }
        } 
    }
}
