using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
// ReSharper disable ArrangeAccessorOwnerBody

namespace EventAggregator {

	/// <summary>
	/// Defines a subscriber and the message types it listens for.
	/// </summary>
	internal class Subscriber : DisposableObject
	{
		private WeakReference objectPointer;
		private Dictionary<Type, MethodInfo> eventHandlingMethods = new Dictionary<Type, MethodInfo>();

		/// <summary>
		/// Defines a subscriber and the message types it listens for.
		/// </summary>
		public Subscriber(object subscriber)
		{
			if (subscriber == null) {
				throw new Exception(message: "Subscribers to the event aggregator have to be alive.");
			}

			this.objectPointer = new WeakReference(subscriber);

			var eventSubscriberInterfaces = subscriber
				.GetType()
				.GetInterfaces()
				.Where(x => typeof(IEventSubscriber).IsAssignableFrom(x) && x.GetTypeInfo().IsGenericType)
				.ToList();

			if (!eventSubscriberInterfaces.Any()) {
				throw new Exception(message:"The subscriber needs to implement IEventSubscriber<T> in order to receive messages from the event aggregator.");
			}

			foreach (var eventSubscriberInterface in eventSubscriberInterfaces)
			{
				var messageType = eventSubscriberInterface.GetGenericArguments()[0];
				var eventHandlingMethod = eventSubscriberInterface.GetMethod(nameof(IEventSubscriber<object>.OnAggregateEvent), types:new Type[] { messageType });

				if (eventHandlingMethod != null)
				{
					this.eventHandlingMethods[messageType] = eventHandlingMethod;
				}
			}
		}

		/// <summary>
		/// Check to see if the actual object represented by the notion of a subscriber is still alive and not GC'd
		/// </summary>
		public bool IsAlive
		{
			get { return this.objectPointer.IsAlive; }
		}

		/// <summary>
		/// Checks to see if the actual object represented by the notion of a subscriber, matches another subscriber
		/// </summary>
		/// <param name="otherSubscriber"></param>
		/// <returns></returns>
		public bool Matches(object otherSubscriber)
		{
			if (!this.IsAlive) {
				return false;
			}

			return this.objectPointer.Target == otherSubscriber;
		}

		/// <summary>
		/// A message is relayed to the subscriber so they can handle it as needed.
		/// </summary>
		public bool ReceiveMessage(object message)
		{
			var messageType = message.GetType();

			if (!this.eventHandlingMethods.ContainsKey(messageType)) {
				return false;
			}

			if (!this.IsAlive) {
				return false;
			}

			foreach (var eventHandlingMethod in this.eventHandlingMethods)
			{
				var methodParameter = eventHandlingMethod.Key;
				var methodPointer = eventHandlingMethod.Value;

				if (methodParameter.IsAssignableFrom(messageType))
				{
					try
					{
						methodPointer.Invoke(this.objectPointer.Target, parameters:new[] { message });
						return true;
					}
					catch
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Checks to see if the subscriber is listening for a message of particular type
		/// </summary>
		/// <param name="messageType"></param>
		/// <returns></returns>
		public bool IsListeningForMessageType(Type messageType)
		{
			return this.eventHandlingMethods.ContainsKey(messageType);
		}

		/// <summary>
		/// Kill the subscrition
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.objectPointer = null;
				this.eventHandlingMethods.Clear();
				this.eventHandlingMethods = null;
			}
			base.Dispose(disposing);
		}
	}
}