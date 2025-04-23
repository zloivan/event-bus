using System;

namespace IKhom.EventBusSystem.Runtime.abstractions
{
    /// <summary>
    /// Interface for event binding implementations that handle event callbacks.
    /// Event bindings connect event sources to event handlers in the EventBus system.
    /// </summary>
    /// <typeparam name="T">The type of the event, which must implement IEvent interface.</typeparam>
    internal interface IEventBinding<T>
    {
        /// <summary>
        /// Gets or sets the callback to invoke when an event is raised with the event data.
        /// </summary>
        public Action<T> OnEvent { get; set; }

        /// <summary>
        /// Gets or sets the callback to invoke when an event is raised without passing the event data.
        /// This is useful for events where you only care that they happened, not their specific data.
        /// </summary>
        public Action OnEventNoArgs { get; set; }
    }
}