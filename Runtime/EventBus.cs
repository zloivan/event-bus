using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IKhom.EventBusSystem.Runtime.abstractions;
using IKhom.EventBusSystem.Runtime.helpers;
using JetBrains.Annotations;
using ILogger = IKhom.EventBusSystem.Runtime.helpers.ILogger;
using System.Threading.Tasks;
#if UNITASK_SUPPORT
using Cysharp.Threading.Tasks;

#endif

namespace IKhom.EventBusSystem.Runtime
{
    /// <summary>
    /// A generic event bus that facilitates communication between different parts of an application
    /// using a publish-subscribe pattern. It manages event registration, de-registration, and invocation.
    /// Also includes event caching to allow late subscribers to access the most recently raised event.
    /// </summary>
    /// <typeparam name="T">The type of the event, which must implement IEvent interface.</typeparam>
    public static class EventBus<T> where T : IEvent
    {
        private static readonly HashSet<IEventBinding<T>> _bindings = new();
        private static readonly ILogger _logger = new EventBusLogger();
        private static readonly object _lock = new();

        private static T _lastEvent;
        private static bool _hasLastEvent;

        /// <summary>
        /// Clears all registered bindings and the cached event.
        /// Used internally by the EventBusUtilities when exiting play mode.
        /// </summary>
        [PublicAPI] private static void Clear()
        {
            lock (_lock)
            {
                _logger.Log($"Clearing {typeof(T).Name} bindings...");
                _bindings.Clear();
                _lastEvent = default;
                _hasLastEvent = false;
            }
        }

        /// <summary>
        /// Registers an event binding for this type of event.
        /// </summary>
        /// <param name="binding">The event binding to register.</param>
        [PublicAPI] public static void Register(EventBinding<T> binding)
        {
            lock (_lock)
            {
                _logger.Log($"Registering binding for {typeof(T).Name}");
                _bindings.Add(binding);
            }
        }

        /// <summary>
        /// De-registers an event binding for this type of event.
        /// </summary>
        /// <param name="binding">The event binding to deregister.</param>
        [PublicAPI] public static void Deregister(EventBinding<T> binding)
        {
            lock (_lock)
            {
                _logger.Log($"De-registering binding for {typeof(T).Name}");
                _bindings.Remove(binding);
            }
        }

        /// <summary>
        /// Raises an event of this type, invoking all registered callbacks with the event data.
        /// The event is also cached for future retrieval.
        /// </summary>
        /// <param name="event">The event to raise.</param>
        [PublicAPI] public static void Raise(T @event)
        {
            IEventBinding<T>[] bindingsCopy;
            lock (_lock)
            {
                _logger.Log($"Raising event for {typeof(T).Name} with {_bindings.Count} bindings");

                _lastEvent = @event;
                _hasLastEvent = true;

                // Создаем копию коллекции для безопасного перебора
                bindingsCopy = _bindings.ToArray();
            }

            // Перебираем копию вне блокировки
            foreach (var binding in bindingsCopy)
            {
                binding.OnEvent?.Invoke(@event);
                binding.OnEventNoArgs?.Invoke();
            }
        }

        /// <summary>
        /// Tries to get the last raised event of this type.
        /// </summary>
        /// <param name="lastEvent">When this method returns, contains the last event 
        /// if one was raised, or the default value if no event has been raised.</param>
        /// <returns>True if an event was previously raised, false otherwise.</returns>
        [PublicAPI] public static bool TryGetLastEvent(out T lastEvent)
        {
            lock (_lock)
            {
                lastEvent = _lastEvent;
                return _hasLastEvent;
            }
        }

        /// <summary>
        /// Gets the last raised event of this type.
        /// </summary>
        /// <returns>The last raised event, or default value if no event has been raised.</returns>
        [PublicAPI] public static T GetLastEvent()
        {
            lock (_lock)
            {
                return _hasLastEvent ? _lastEvent : default;
            }
        }

        /// <summary>
        /// Checks if an event of this type has been raised.
        /// </summary>
        /// <returns>True if an event was previously raised, false otherwise.</returns>
        [PublicAPI] public static bool HasLastEvent()
        {
            lock (_lock)
            {
                return _hasLastEvent;
            }
        }

        /// <summary>
        /// Clears the cached last event.
        /// </summary>
        [PublicAPI]
        public static void ClearLastEvent()
        {
            lock (_lock)
            {
                _lastEvent = default;
                _hasLastEvent = false;
                _logger.Log($"Cleared last event for {typeof(T).Name}");
            }
        }


#if UNITASK_SUPPORT
        /// <summary>
        /// Waits asynchronously for an event of this type to be raised using UniTask.
        /// </summary>
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
        /// <returns>A UniTask that completes with the raised event.</returns>
        [PublicAPI]
        public static UniTask<T> WaitForEventUniTaskAsync(CancellationToken cancellationToken = default)
        {
            var source = new UniTaskCompletionSource<T>();
            EventBinding<T> binding = null;

            binding = new EventBinding<T>(e =>
            {
                Deregister(binding);
                source.TrySetResult(e);
            });

            Register(binding);

            cancellationToken.Register(() =>
            {
                Deregister(binding);
                source.TrySetCanceled(cancellationToken);
            });

            return source.Task;
        }

        /// <summary>
        /// Waits asynchronously for an event of this type to be raised using UniTask, with a timeout.
        /// </summary>
        /// <param name="timeout">The maximum time to wait for the event.</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
        /// <returns>A UniTask that completes with the raised event, or throws TimeoutException if the timeout elapses.</returns>
        [PublicAPI]
        public static async UniTask<T> WaitForEventUniTaskAsync(TimeSpan timeout,
            CancellationToken cancellationToken = default)
        {
            var timeoutTask = UniTask.Delay(timeout, cancellationToken: cancellationToken);
            var eventTask = WaitForEventUniTaskAsync(cancellationToken);

            var (hasValue, value) = await UniTask.WhenAny(eventTask, timeoutTask);

            if (hasValue)
            {
                return value;
            }

            throw new TimeoutException($"Timeout waiting for {typeof(T).Name} event");
        }

#endif
        /// <summary>
        /// Waits asynchronously for an event of this type to be raised.
        /// </summary>
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
        /// <returns>A task that completes with the raised event.</returns>
        [PublicAPI]
        public static Task<T> WaitForEventAsync(CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<T>();
            EventBinding<T> binding = null;

            binding = new EventBinding<T>(e =>
            {
                Deregister(binding);
                tcs.TrySetResult(e);
            });

            Register(binding);

            cancellationToken.Register(() =>
            {
                Deregister(binding);
                tcs.TrySetCanceled(cancellationToken);
            });

            return tcs.Task;
        }

        /// <summary>
        /// Waits asynchronously for an event of this type to be raised, with a timeout.
        /// </summary>
        /// <param name="timeout">The maximum time to wait for the event.</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation.</param>
        /// <returns>A task that completes with the raised event, or throws TimeoutException if the timeout elapses.</returns>
        [PublicAPI]
        public static async Task<T> WaitForEventAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            using var timeoutCts = new CancellationTokenSource(timeout);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken);

            try
            {
                return await WaitForEventAsync(linkedCts.Token);
            }
            catch (OperationCanceledException) when (timeoutCts.Token.IsCancellationRequested)
            {
                throw new TimeoutException($"Timeout waiting for {typeof(T).Name} event");
            }
        }

    }
}