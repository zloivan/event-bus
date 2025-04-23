using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using IKhom.EventBusSystem.Runtime.abstractions;
using IKhom.EventBusSystem.Runtime.helpers;

namespace IKhom.EventBusSystem.Runtime
{
    /// <summary>
    /// A non-static version of EventBus that can be instantiated for local event handling
    /// </summary>
    public class LocalEventBus<T> : IEventBus<T> where T : IEvent
    {
        private readonly HashSet<IEventBinding<T>> _bindings = new();
        private readonly ILogger _logger = new EventBusLogger();
        private readonly object _lock = new();

        private T _lastEvent;
        private bool _hasLastEvent;

        public void Register(EventBinding<T> binding)
        {
            lock (_lock)
            {
                _logger.Log($"Registering binding for {typeof(T).Name}");
                _bindings.Add(binding);
            }
        }

        public void Deregister(EventBinding<T> binding)
        {
            lock (_lock)
            {
                _logger.Log($"De-registering binding for {typeof(T).Name}");
                _bindings.Remove(binding);
            }
        }

        public void Raise(T @event)
        {
            lock (_lock)
            {
                _logger.Log($"Raising event for {typeof(T).Name} with {_bindings.Count} bindings");

                _lastEvent = @event;
                _hasLastEvent = true;

                foreach (var binding in _bindings)
                {
                    binding.OnEvent?.Invoke(@event);
                    binding.OnEventNoArgs?.Invoke();
                }
            }
        }

        public bool TryGetLastEvent(out T lastEvent)
        {
            lock (_lock)
            {
                lastEvent = _lastEvent;
                return _hasLastEvent;
            }
        }

        public T GetLastEvent()
        {
            lock (_lock)
            {
                return _hasLastEvent ? _lastEvent : default;
            }
        }

        public bool HasLastEvent()
        {
            lock (_lock)
            {
                return _hasLastEvent;
            }
        }

        public void ClearLastEvent()
        {
            lock (_lock)
            {
                _lastEvent = default;
                _hasLastEvent = false;
                _logger.Log($"Cleared last event for {typeof(T).Name}");
            }
        }

        // Implement the async methods from the original EventBus
        public Task<T> WaitForEventAsync(CancellationToken cancellationToken = default)
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

        public async Task<T> WaitForEventAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
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

#if UNITASK_SUPPORT
        public UniTask<T> WaitForEventUniTaskAsync(CancellationToken cancellationToken = default)
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

        public async UniTask<T> WaitForEventUniTaskAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
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
    }
}