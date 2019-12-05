# EventAggregator.NET
An in-memory publish-subscribe messaging pattern used for inter-component communication.

## Usage

### Event / Message Publisher
A publisher needs to get a reference to the event aggregator in order to be able to publish messages through it.

### Event / Message Consumer
A consumer needs to implement `IEventSubscriber<T>` where T represents the type of message it is listening for.
The `IEventSubscriber<T>` interface forces a subscriber to implement a message handler when the message is received. 
The method signature of the handler takes in a single parameter of type T (where T represents the message type):

`public void OnAggregateEvent(T message)`

Multiple subscriptions for different messages are possible as each implementation of the `OnAggregateEvent(T message)` message
handler become overloads of each other.

### Example

```csharp
 
 /// <summary>
 /// Represents the message being published or listened for 
 /// </summary>
 public class MessageType1 {
    ...
 }
 
 /// <summary>
 /// Represents the message being published or listened for 
 /// </summary>
 public class MessageType2 { 
    ...
 }


 /// <summary>
 /// Represents a single publisher.
 /// </summary>
 public class Publisher1 {
 
    private readonly IEventAggregator eventAggregator;
    
    /// <summary>
    /// A constructor for an event publisher that takes an IEventAggregator as a dependency...
    /// </summary>
    public Publisher1(IEventAggregator eventAggregator) {
    
      if(eventAggregator == null) {
        throw new ArgumentNullException(paramName: nameof(eventAggregator), message: "The event aggregator is required.");
      }
    
      this.eventAggregator = eventAggregator;
    }
    
    /// <summary>
    /// Publishes an event/message to listening consumers.
    /// </summary>
    public void PublishMessageType1() {
    
      this.eventAggregator.Publish(new MessageType1());
    
    }
 }
 
 /// <summary>
 /// Represents another publisher.
 /// </summary>
 public class Publisher2 {
 
    private readonly IEventAggregator eventAggregator;
    
    /// <summary>
    /// A constructor for an event publisher that takes an IEventAggregator as a dependency...
    /// </summary>
    public Publisher1(IEventAggregator eventAggregator) {
    
      if(eventAggregator == null) {
        throw new ArgumentNullException(paramName: nameof(eventAggregator), message: "The event aggregator is required.");
      }
    
      this.eventAggregator = eventAggregator;
    }
    
    /// <summary>
    /// Publishes an event/message to listening consumers.
    /// </summary>
    public void PublishMessageType2() {
    
      this.eventAggregator.Publish(new MessageType2());
    
    }
 } 
 
 /// <summary>
 /// Represents a subscriber that listens to message types from both publishers.
 /// </summary>
 public class Subscriber : IEventSubscriber<MessageType1>, IEventSubscriber<MessageType2> {
 
    /// <summary>
    /// Event handler executed when a message/event of type MessageType1 is received.
    /// </summary>
    public void OnAggregateEvent(MessageType1 message) {
      ...
    }
 
 
    /// <summary>
    /// Event handler executed when a message/event of type MessageType2 is received.
    /// </summary>
    public void OnAggregateEvent(MessageType2 message) {
      ...
    }
 }
