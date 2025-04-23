using System;
using IKhom.EventBusSystem.Runtime.abstractions;

namespace IKhom.EventBusSystem.Runtime.helpers
{
    /// <summary>
    /// Extension methods for the EventBus system to simplify common operations
    /// </summary>
    public static class EventBusExtensions
    {
        // Extension methods for static EventBus
        
        /// <summary>
        /// Registers a callback function with the global EventBus for a specific event type
        /// </summary>
        public static EventBinding<T> RegisterHandler<T>(this Action<T> handler) where T : IEvent
        {
            var binding = new EventBinding<T>(handler);
            EventBus<T>.Register(binding);
            return binding;
        }
        
        /// <summary>
        /// Registers a parameterless callback function with the global EventBus for a specific event type
        /// </summary>
        public static EventBinding<T> RegisterHandler<T>(this Action handler) where T : IEvent
        {
            var binding = new EventBinding<T>(handler);
            EventBus<T>.Register(binding);
            return binding;
        }
        
        /// <summary>
        /// Deregisters a binding from the global EventBus
        /// </summary>
        public static void DeregisterHandler<T>(this EventBinding<T> binding) where T : IEvent
        {
            EventBus<T>.Deregister(binding);
        }

        // Extension methods for non-static IEventBus
        
        /// <summary>
        /// Registers a callback function with a local EventBus for a specific event type
        /// </summary>
        public static EventBinding<T> RegisterHandler<T>(this IEventBus<T> eventBus, Action<T> handler) where T : IEvent
        {
            var binding = new EventBinding<T>(handler);
            eventBus.Register(binding);
            return binding;
        }
        
        /// <summary>
        /// Registers a parameterless callback function with a local EventBus for a specific event type
        /// </summary>
        public static EventBinding<T> RegisterHandler<T>(this IEventBus<T> eventBus, Action handler) where T : IEvent
        {
            var binding = new EventBinding<T>(handler);
            eventBus.Register(binding);
            return binding;
        }
    }
}