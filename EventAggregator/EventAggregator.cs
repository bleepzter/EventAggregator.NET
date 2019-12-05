using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace EventAggregator {

	/// <summary>
	/// An object that can route messages to 0, 1 or many subscribers.
	/// </summary>
	public class EventAggregator : DisposableObject, IEventAggregator
	{
		private ConcurrentBag<Subscriber> subscribers;

		/// <summary>
		/// An object that can route messages to 0, 1 or many subscribers.
		/// </summary>
		public EventAggregator()
		{
			this.subscribers = new ConcurrentBag<Subscriber>();
		}

		/// <summary>
		/// Publishes a particular type of message to all subscribers that implement <see cref="IEventSubscriber{T}"/>. 
		/// The subscribers will receive the message as long as the type specified in 
		/// their implementation of <see cref="IEventSubscriber{T}"/> matches the type of 
		/// the published message
		/// </summary>
		/// <typeparam name="T">Defines the type of message to be published</typeparam>
		/// <param name="message">Defines the message to be published</param>
		public void Publish<T>(T message)
		{
			if (message == null) {
				throw new ArgumentNullException(paramName:nameof(message), message:"Unable to publish a null message");
			}

			var subscribersToNotify = this.subscribers.ToArray();
			var messageType = message.GetType();
			var deadSubscribers = new List<Subscriber>();

			foreach (var subscriber in subscribersToNotify)
			{
				if (subscriber.IsListeningForMessageType(typeof(T)))
				{
					bool result = subscriber.ReceiveMessage(message);

					if (!result) {
						deadSubscribers.Add(subscriber);
					}
				}
			}

			if (deadSubscribers.Any())
			{
				for (int i = deadSubscribers.Count; i-- > 0;)
				{
					var deadSubscriber = deadSubscribers[i];
					deadSubscribers.RemoveAt(i);

					this.subscribers = new ConcurrentBag<Subscriber>(collection:this.subscribers.Except(new[] { deadSubscriber }));
					
					deadSubscriber.Dispose();
				}
			}
		}

		/// <summary>
		/// Subscribes an object so that object can receive messages of type T, as long as the object implements <see cref="IEventSubscriber{T}"/>.
		/// </summary>
		/// <param name="subscriber">The object that wants to receive messages.</param>
		public void Subscribe(object subscriber)
		{
			if (subscriber == null) {
				throw new ArgumentNullException(nameof(subscriber));
			}

			if (this.subscribers.Any(x => x.Matches(subscriber))) {
				return;
			}

			this.subscribers.Add(new Subscriber(subscriber));
		}

		/// <summary>
		/// Checks the list of subscribers to see if any one of them can handle the message type 
		/// </summary>
		/// <param name="messageType">The type of message for which we are looking for subscribers</param>
		public bool SubscriberExistsFor(Type messageType)
		{
			return this.subscribers.Any(subscriber => subscriber.IsListeningForMessageType(messageType));
		}

		/// <summary>
		/// Removes all message subscriptions for given object.
		/// </summary>
		/// <param name="subscriber">The object that wants to unsubscribe from receiving messages.</param>
		public void Unsubscribe(object subscriber)
		{
			if (subscriber == null) {
				throw new ArgumentNullException(paramName:nameof(subscriber), message:"Unable to unsubscribe a null reference");
			}

			var found = this.subscribers.FirstOrDefault(x => x.Matches(subscriber));

			if (found != null) {
				this.subscribers = new ConcurrentBag<Subscriber>(collection:this.subscribers.Except(new[] { found }));
			}
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				for (var i = this.subscribers.Count; i-- > 0;)
				{
					var deadSubscriber = this.subscribers.ElementAt(i);
					this.subscribers = new ConcurrentBag<Subscriber>(collection: this.subscribers.Except(new[] { deadSubscriber }));
					deadSubscriber.Dispose();
				}
			}

			base.Dispose(disposing);
		}
	}

}