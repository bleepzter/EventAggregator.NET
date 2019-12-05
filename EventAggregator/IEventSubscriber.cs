namespace EventAggregator {

	/// <summary>
	/// Defines an object that subscries to receive messages
	/// </summary>
	public interface IEventSubscriber { }

	/// <summary>
	/// Defines an object that subscribes to receive messages of a specific type
	/// </summary>
	/// <typeparam name="T">Defines the type of message</typeparam>
	public interface IEventSubscriber<T> : IEventSubscriber
	{
		/// <summary>
		/// Defines a method that allows the subscriber to receive messages of type T received from an event aggregator.
		/// </summary>
		/// <param name="message">The message that is received as an aggregate event</param>
		void OnAggregateEvent(T message);
	}

}