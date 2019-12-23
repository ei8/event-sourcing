using CQRSlite.Events;
using Moq;
using org.neurul.Common.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.EventSourcing.Common;
using works.ei8.EventSourcing.Port.Adapter.Common;
using Xunit;

namespace works.ei8.EventSourcing.Port.Adapter.IO.Persistence.Events.SQLite.Test.EventStoreFixture.given
{
    public abstract class ProperlyConstructedContext : TestContext<EventStore>
    {
        protected override void Given()
        {
            base.Given();
            this.GivenInit();
        }

        protected virtual void GivenInit()
        {
            this.sut = new EventStore();    
        }

        protected virtual Action<IEvent, CancellationToken> PublishEventCallback => (e, c) => {};
    }

    public abstract class InitializedContext : ProperlyConstructedContext
    {
        protected override void GivenInit()
        {
            base.GivenInit();
            Environment.SetEnvironmentVariable(EnvironmentVariableKeys.DatabasePath, ":{0}:");
            // TODO: Task.Run(() => this.sut.Initialize("memory")).Wait();
        }
    }

    // TODO:
    //public class When_saving_event 
    //{
    //    public class When_store_is_empty : InitializedContext
    //    {
    //        private IEnumerable<IEvent> events;
    //        private List<IEvent> publishedEventsList;
    //        private Guid guid;
    //        private Guid terminalGuid;
    //        private Guid authorId;

    //        protected override void GivenInit()
    //        {
    //            base.GivenInit();

    //            this.guid = Guid.NewGuid();
    //            this.terminalGuid = Guid.NewGuid();
    //            this.authorId = Guid.NewGuid();

    //            this.events = new List<IEvent>() {
    //                new NeuronCreated(this.guid, string.Empty, this.authorId) { Version = 1 },
    //                new TerminalCreated(this.terminalGuid, Guid.NewGuid(), this.guid, NeurotransmitterEffect.Excite, 1f, this.authorId) { Version = 1 }
    //            };

    //            this.publishedEventsList = new List<IEvent>();
    //        }

    //        protected override Action<IEvent, CancellationToken> PublishEventCallback => (e, c) => this.publishedEventsList.Add(e);

    //        protected override void When()
    //        {
    //            base.When();

    //            Task.Run(async () => await this.sut.Save(this.events)).Wait();
    //        }

    //        [Fact]
    //        public async Task Then_should_contain_correct_typename_prefixes()
    //        {
    //            var events = await this.sut.Get(this.guid, 0);
    //            Assert.StartsWith("works.ei8.EventSourcing.Domain.Model", ((IAuthoredEvent) events.Last()).ToNotification(this.serializer).TypeName);
    //        }

    //        [Fact]
    //        public async Task Then_should_contain_correct_typename_of_first_event()
    //        {
    //            var events = await this.sut.Get(this.guid, 0);
    //            Assert.StartsWith("works.ei8.EventSourcing.Domain.Model.Neurons.NeuronCreated", ((IAuthoredEvent) events.First()).ToNotification(this.serializer).TypeName);
    //        }

    //        [Fact]
    //        public async Task Then_should_contain_correct_typename_of_second_event()
    //        {
    //            var events = await this.sut.Get(this.terminalGuid, 0);
    //            Assert.StartsWith("works.ei8.EventSourcing.Domain.Model.Neurons.TerminalCreated", ((IAuthoredEvent) events.Last()).ToNotification(this.serializer).TypeName);
    //        }

    //        [Fact]
    //        public void Then_should_publish_two_events()
    //        {
    //            Assert.Equal(2, this.publishedEventsList.Count);
    //        }
    //    }
    //}

    public abstract class ContainingEventsContext : InitializedContext, IDisposable
    {
        protected List<Notification> notificationList;
        protected long sequenceId;

        protected override void GivenInit()
        {
            base.GivenInit();

            this.sequenceId = 1;
            this.notificationList = new List<Notification>();
        }

        protected override void Given()
        {
            base.Given();

            Task.Run(async () => await this.sut.Save("memory", new Notification[] {
                    new Notification() { SequenceId = 1 },
                    new Notification() { SequenceId = 2 },
                    new Notification() { SequenceId = 3 },
                    new Notification() { SequenceId = 4 },
                    new Notification() { SequenceId = 5 },
                    new Notification() { SequenceId = 6 },
                    new Notification() { SequenceId = 7 },
                    new Notification() { SequenceId = 8 },
                    new Notification() { SequenceId = 9 }                    
                }
            )).Wait();
        }

        public void Dispose()
        {
            // TODO: this.sut.CloseConnection();
        }
    }

    //public class When_counting_events
    //{
    //    public class When_store_contains_events : ContainingEventsContext
    //    {
    //        private long count;

    //        protected override void When()
    //        {
    //            base.When();

    //            Task.Run(async () => this.count = await this.sut.CountNotifications()).Wait();
    //        }

    //        [Fact]
    //        public void Then_should_be_equal_to_nine()
    //        {
    //            Assert.Equal(9, this.count);
    //        }
    //    }
    //}

    // TODO: public class When_getting_event_range
    //{
    //    public abstract class GettingRangeContext : ContainingEventsContext
    //    {
    //        protected NotificationLog resultNotification;

    //        protected override void When()
    //        {
    //            base.When();

    //            Task.Run(async () =>
    //                this.resultNotification = await this.sut.GetLog("memory", new NotificationLogId(this.LowSequenceId, this.HighSequenceId))
    //            ).Wait();
    //        }

    //        protected abstract long LowSequenceId { get; }

    //        protected abstract long HighSequenceId { get; }
    //    }

    //    public class When_store_contains_events
    //    {
    //        public class When_low_sequence_id_is_greater_than_high_sequence_id : ContainingEventsContext
    //        {
    //            [Fact]
    //            public async Task Then_should_throw_argument_exception()
    //            {
    //                await Assert.ThrowsAsync<ArgumentException>("lowSequenceId", async () => await this.sut.GetLog("memory", new NotificationLogId(2, 1)));
    //            }
    //        }

    //        public class When_high_sequence_id_is_greater_than_count : GettingRangeContext
    //        {
    //            protected override long LowSequenceId => 6;

    //            protected override long HighSequenceId => 10;

    //            [Fact]
    //            public void Then_should_return_all_events_that_can_be_returned()
    //            {
    //                Assert.Equal(4, this.resultNotification.NotificationList.Count);
    //            }
    //        }

    //        public class When_low_sequence_id_is_less_than_absolute_minimum : ContainingEventsContext
    //        {
    //            [Fact]
    //            public async Task Then_should_throw_argument_out_of_range_exception()
    //            {
    //                await Assert.ThrowsAsync<ArgumentOutOfRangeException>("lowSequenceId", async () => await this.sut.GetLog("memory", new NotificationLogId(0, 9)));
    //            }
    //        }

    //        public class When_arguments_are_valid : ContainingEventsContext
    //        {
    //            [Fact]
    //            public async Task Then_should_return_correct_count_of_events()
    //            {
    //                Assert.Equal(5, (await this.sut.GetLog("memory", new NotificationLogId(4, 8))).NotificationList.Count());
    //            }

    //            [Fact]
    //            public async Task Then_should_return_first_event_with_correct_sequence_id()
    //            {
    //                Assert.Equal(4, (await this.sut.GetLog("memory", new NotificationLogId(4, 8))).NotificationList.First().SequenceId);
    //            }

    //            [Fact]
    //            public async Task Then_should_return_last_event_with_correct_sequence_id()
    //            {
    //                Assert.Equal(8, (await this.sut.GetLog("memory", new NotificationLogId(4, 8))).NotificationList.Last().SequenceId);
    //            }
    //        }
    //    }
    //}

    //public class When_getting_all_events_since_sequence_id
    //{
    //    public class When_store_contains_events
    //    {
    //        public class When_sequence_id_is_less_than_absolute_minimum : ContainingEventsContext
    //        {
    //            [Fact]
    //            public async Task Then_should_throw_argument_out_of_range_exception()
    //            {
    //                await Assert.ThrowsAsync<ArgumentOutOfRangeException>("sequenceId", async () => await this.sut.GetAllNotificationsSince(0));
    //            }
    //        }

    //        public class When_sequence_id_is_greater_than_count : ContainingEventsContext
    //        {
    //            [Fact]
    //            public async Task Then_should_throw_argument_out_of_range_exception()
    //            {
    //                await Assert.ThrowsAsync<ArgumentOutOfRangeException>("sequenceId", async () => await this.sut.GetAllNotificationsSince(10));
    //            }
    //        }

    //        public class When_arguments_are_valid : ContainingEventsContext
    //        {
    //            [Fact]
    //            public async Task Then_should_return_correct_count_of_events()
    //            {
    //                Assert.Equal(5, (await this.sut.GetAllNotificationsSince(5)).Count());
    //            }
    //        }
    //    }
    //}
}
