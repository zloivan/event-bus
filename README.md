# Event Bus System for Unity

## Overview
A lightweight and efficient event bus system for Unity, designed to facilitate communication between different parts of your application.

## Features
- Simple interface for event definition
- Flexible event binding
- Event caching for accessing last triggered event
- Comprehensive logging
- Editor integration for automatic cleanup
- Examples included for quick start

## Installation
To install the package, follow these steps:

1. Open your Unity project.
2. Open `Window > Package Manager`.
3. Click on the `+` button in the top left corner and select `Add package from git URL...`.
4. Enter the following URL: `https://github.com/zloivan/event-bus.git` and click `Add`.

## Usage
### Defining Events
Define your events by implementing the `IEvent` interface. Can be **struct** or **class**.

```csharp
public struct TestEvent : IEvent { }

public struct PlayerUpdatedEvent : IEvent 
{
    public int Health;
    public string Name;
}
```
### Raising Events
Use the `EventBus<T>.Raise` method to raise events.
```csharp
EventBus<PlayerUpdatedEvent>.Raise(new PlayerUpdatedEvent { Health = 100, Name = "Player1" });
```
### Handling Events
Register and handle events in your scripts.

```csharp
public class EventReceiver : MonoBehaviour
{
    private EventBinding<PlayerUpdatedEvent> _playerUpdatedEventBinding;

    private void OnEnable()
    {
        _playerUpdatedEventBinding = new EventBinding<PlayerUpdatedEvent>(HandlePlayerUpdateEvent);

        EventBus<PlayerUpdatedEvent>.Register(_playerUpdatedEventBinding);
        
        //Can add additional handlers to bindings
        _playerUpdatedEventBinding.Add(AdditionalUpdateEventHandler);
    }

    private void OnDisable()
    {
        EventBus<PlayerUpdatedEvent>.Deregister(_playerUpdatedEventBinding);
    }

    private void HandlePlayerUpdateEvent(PlayerUpdatedEvent playerUpdatedEvent)
    {
        Debug.Log($"Received PlayerUpdatedEvent: Name = {playerUpdatedEvent.Name}, Health = {playerUpdatedEvent.Health}");
    }
    
    private void AdditionalUpdateEventHandler(PlayerUpdatedEvent playerUpdatedEvent)
    {
        Debug.Log("Handlers can be added after the registration of binder");
    }
}
```

### Event Caching
You can access the most recently triggered event using the event cache:

```csharp
// Check if an event has been raised and get it
if (EventBus<PlayerUpdatedEvent>.TryGetLastEvent(out var lastEvent))
{
    Debug.Log($"Last player update: {lastEvent.Name}, Health = {lastEvent.Health}");
}

// Get the last event directly (will be default/null if none was triggered)
var lastEvent = EventBus<PlayerUpdatedEvent>.GetLastEvent();

// Check if an event has been triggered
bool wasTriggered = EventBus<PlayerUpdatedEvent>.HasLastEvent();

// Clear the cached event if needed
EventBus<PlayerUpdatedEvent>.ClearLastEvent();
```

### Automatic Binding
Binding is done via reflection and taken from assemblies, so no additional binding is required. The `EventBusUtilities` class ensures all event types are initialized automatically.

### Debug Logs
To enable debug logs, add new item to scripting define symbols `DEBUG_EVENT_BUS`

### Contributing
Contributions are welcome! Please submit a pull request or open an issue to discuss changes.