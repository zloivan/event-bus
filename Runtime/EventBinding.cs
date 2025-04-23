using System;
using IKhom.EventBusSystem.Runtime.abstractions;
using JetBrains.Annotations;

namespace IKhom.EventBusSystem.Runtime
{
    /// <summary>
    /// A concrete implementation of IEventBinding that handles event callbacks.
    /// EventBinding allows for subscribing to events with or without parameters.
    /// </summary>
    /// <typeparam name="T">The type of the event, which must implement IEvent interface.</typeparam>
    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        private Action<T> _onEvent = _ => { };
        private Action _onEventNoArgs = () => { };

        /// <summary>
        /// Gets or sets the callback to invoke when an event is raised with the event data.
        /// </summary>
        Action<T> IEventBinding<T>.OnEvent
        {
            get => _onEvent;
            set => _onEvent = value;
        }

        /// <summary>
        /// Gets or sets the callback to invoke when an event is raised without passing the event data.
        /// </summary>
        Action IEventBinding<T>.OnEventNoArgs
        {
            get => _onEventNoArgs;
            set => _onEventNoArgs = value;
        }

        /// <summary>
        /// Initializes a new instance of the EventBinding class with a callback that receives the event data.
        /// </summary>
        /// <param name="onEvent">The callback to invoke when an event is raised.</param>
        [PublicAPI] public EventBinding(Action<T> onEvent) => _onEvent = onEvent;

        /// <summary>
        /// Initializes a new instance of the EventBinding class with a callback that doesn't receive the event data.
        /// </summary>
        /// <param name="onEventNoArgs">The callback to invoke when an event is raised.</param>
        [PublicAPI]
        public EventBinding(Action onEventNoArgs) => _onEventNoArgs = onEventNoArgs;

        /// <summary>
        /// Adds a callback to invoke when an event is raised with the event data.
        /// </summary>
        /// <param name="onEvent">The callback to add.</param>
        [PublicAPI]
        public void Add(Action<T> onEvent) => _onEvent += onEvent;

        /// <summary>
        /// Removes a callback that would be invoked when an event is raised with the event data.
        /// </summary>
        /// <param name="onEvent">The callback to remove.</param>
        [PublicAPI]
        public void Remove(Action<T> onEvent) => _onEvent -= onEvent;

        /// <summary>
        /// Adds a callback to invoke when an event is raised without passing the event data.
        /// </summary>
        /// <param name="onEvent">The callback to add.</param>
        [PublicAPI]
        public void Add(Action onEvent) => _onEventNoArgs += onEvent;

        /// <summary>
        /// Removes a callback that would be invoked when an event is raised without passing the event data.
        /// </summary>
        /// <param name="onEvent">The callback to remove.</param>
        [PublicAPI]
        public void Remove(Action onEvent) => _onEventNoArgs -= onEvent;
    }
}