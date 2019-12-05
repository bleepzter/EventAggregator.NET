using System;
using Moq;
using Xunit;

namespace EventAggregator.UnitTests {

	public class EventAggregatorTests : UnitTestBase {


		[Fact]
		public void Subscriber_Without_IEventSubscriber_Implementation_Throws_Error_At_Registration() {

			var invalidSubscriber = new object();
			var eventAggregator = this.GetEventAggregator();
			Assert.Throws<Exception>(testCode:()=> eventAggregator.Subscribe(invalidSubscriber));

		}

		[Fact]
		public void Subscriber_With_IEventSubscriber_Implementation_Registers_Properly() {

			var eventAggregator = this.GetEventAggregator();
			var validSubscriber = new Mock<IEventSubscriber<Message>>();
			validSubscriber.Setup(x => x.OnAggregateEvent(It.IsAny<Message>()));

			eventAggregator.Subscribe(validSubscriber.Object);

			Assert.True(eventAggregator.SubscriberExistsFor(messageType:typeof(Message)));
		}

		[Fact]
		public void When_Single_Subscriber_Receives_Exactly_One_Message() {

			var eventAggregator = this.GetEventAggregator();
			var message = new Message() {Id = 1};
			var publisher = new Publisher(eventAggregator);
			var validSubscriber = new Mock<IEventSubscriber<Message>>();
			validSubscriber.Setup(x => x.OnAggregateEvent(It.IsAny<Message>()));

			eventAggregator.Subscribe(validSubscriber.Object);
			publisher.Publish(message);

			
			validSubscriber.Verify(x => x.OnAggregateEvent(It.IsAny<Message>()), Times.AtMostOnce);
		}

		[Fact]
		public void When_Multiple_Subscribers_Each_Receives_Exactly_One_Message()
		{

			var eventAggregator = this.GetEventAggregator();
			var message = new Message() { Id = 1 };
			var publisher = new Publisher(eventAggregator);
			
			var validSubscriber1 = new Mock<IEventSubscriber<Message>>();
			validSubscriber1.Setup(x => x.OnAggregateEvent(It.IsAny<Message>()));

			var validSubscriber2 = new Mock<IEventSubscriber<Message>>();
			validSubscriber2.Setup(x => x.OnAggregateEvent(It.IsAny<Message>()));

			eventAggregator.Subscribe(validSubscriber1.Object);
			eventAggregator.Subscribe(validSubscriber2.Object);
			publisher.Publish(message);


			validSubscriber1.Verify(x => x.OnAggregateEvent(It.IsAny<Message>()), Times.AtMostOnce);
			validSubscriber2.Verify(x => x.OnAggregateEvent(It.IsAny<Message>()), Times.AtMostOnce);
		}
	}

}