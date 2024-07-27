// ReSharper disable StaticMemberInGenericType

using System.Collections.Generic;
using IKhom.EventBusSystem.Runtime.abstractions;
using IKhom.EventBusSystem.Runtime.helpers;

namespace IKhom.EventBusSystem.Runtime
{
    public static class EventBus<T> where T : IEvent
    {
        private static readonly HashSet<IEventBinding<T>> _bindings = new();
        private static readonly ILogger _logger = new EventBusLogger();
        private static readonly object _lock = new();

        
        // ReSharper disable once UnusedMember.Local
        private static void Clear()
        {
            lock (_lock)
            {
                _logger.Log($"Clearing {typeof(T).Name} bindings...");
                _bindings.Clear();
            }
        }

        public static void Register(EventBinding<T> binding)
        {
            lock (_lock)
            {
                _logger.Log($"Registering binding for {typeof(T).Name}");
                _bindings.Add(binding);
            }
        }

        public static void Deregister(EventBinding<T> binding)
        {
            lock (_lock)
            {
                _logger.Log($"Deregistering binding for {typeof(T).Name}");
                _bindings.Remove(binding);
            }
        }

        public static void Raise(T @event)
        {
            lock (_lock)
            {
                _logger.Log($"Raising event for {typeof(T).Name} with {_bindings.Count} bindings");
                foreach (var binding in _bindings)
                {
                    binding.OnEvent?.Invoke(@event);
                    binding.OnEventNoArgs?.Invoke();
                }
            }
        }
    }
}
// ReSharper restore StaticMemberInGenericType