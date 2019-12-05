using System;

namespace EventAggregator {

	/// <summary>
	/// An object that can route messages to 0, 1 or many subscribers.
	/// </summary>
	public interface IEventAggregator : IDisposable
	{
		/// <summary>
		/// Subscribes an object so that object can receive messages of type T, as long as the object implements <see cref="IEventSubscriber{T}"/>.
		/// </summary>
		/// <param name="subscriber">The object that wants to receive messages.</param>
		void Subscribe(object subscriber);

		/// <summary>
		/// Removes all message subscriptions for given object.
		/// </summary>
		/// <param name="subscriber">The object that wants to unsubscribe from receiving messages.</param>
		void Unsubscribe(object subscriber);

		/// <summary>
		/// Publishes a particular type of message to all subscribers that implement <see cref="IEventSubscriber{T}"/>. 
		/// The subscribers will receive the message as long as the type specified in 
		/// their implementation of <see cref="IEventSubscriber{T}"/> matches the type of 
		/// the published message
		/// </summary>
		/// <typeparam name="T">Defines the type of message to be published</typeparam>
		/// <param name="message">Defines the message to be published</param>
		void Publish<T>(T message);

		/// <summary>
		/// Checks the list of subscribers to see if any one of them can handle the message type 
		/// </summary>
		/// <param name="messageType">The type of message for which we are looking for subscribers</param>
		bool SubscriberExistsFor(Type messageType);
	}

}