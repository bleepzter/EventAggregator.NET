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

### Example - Message Definitions

Messages can be any POCO/CLR/DTO (or whatever you want to call them) objects. 
Keep in mind that messages are not immutable and are passed by reference. That means if a handler modifies it,
the next handler that receives it will see the modified changes and not necessary the original message.


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
 
```

### Example - Event/Message Publisher

A publisher simply needs to call the `Publish(T message)` method of the event aggregator to send an event/message to 
listening consumers. So long as the consumers implement the `IEventSubscriber<T>` interface for the message type they should
be able to receive the message.

```csharp

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
 
```

### Example - Event/Message Subscriber

A subscriber needs to implement an `IEventSubscriber<T>` interface for the message type they want to listen to.
In this example the subscriber listens to messages from both publishers defined above.


```csharp

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
 
```
