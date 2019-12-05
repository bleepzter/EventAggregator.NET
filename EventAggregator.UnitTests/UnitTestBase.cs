using System;
using Xunit;
// ReSharper disable ArrangeAccessorOwnerBody
// ReSharper disable ConvertToAutoProperty

namespace EventAggregator.UnitTests
{
	public class UnitTestBase {

		public class Message {
			public int Id { get; set; }
		}

		public class Publisher {

			private readonly IEventAggregator aggregator;

			public Publisher(IEventAggregator aggregator) {
				this.aggregator = aggregator;
			}

			public void Publish(Message message) {
				this.aggregator.Publish(message);
			}
		}

		protected IEventAggregator GetEventAggregator() {
			return new EventAggregator();
		}

	}
}
