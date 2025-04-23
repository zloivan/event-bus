namespace IKhom.EventBusSystem.Runtime.abstractions
{
    /// <summary>
    /// Marker interface for all events in the event bus system.
    /// This interface doesn't provide any methods or properties but serves
    /// as a type constraint for EventBus and EventBinding classes.
    /// Events can be implemented as either structs or classes.
    /// </summary>
    /// <example>
    /// <code>
    /// public struct PlayerDamagedEvent : IEvent 
    /// {
    ///     public int DamageAmount;
    ///     public string DamageSource;
    /// }
    /// </code>
    /// </example>
    public interface IEvent
    {
    }
}