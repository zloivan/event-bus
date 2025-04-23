using System;
using System.Threading;
using System.Threading.Tasks;

namespace IKhom.EventBusSystem.Runtime.abstractions
{
    /// <summary>
    /// Interface for EventBus implementations
    /// </summary>
    public interface IEventBus<T> where T : IEvent
    {
        void Register(EventBinding<T> binding);
        void Deregister(EventBinding<T> binding);
        void Raise(T @event);
        bool TryGetLastEvent(out T lastEvent);
        T GetLastEvent();
        bool HasLastEvent();
        void ClearLastEvent();
        Task<T> WaitForEventAsync(CancellationToken cancellationToken = default);
        Task<T> WaitForEventAsync(TimeSpan timeout, CancellationToken cancellationToken = default);
#if UNITASK_SUPPORT
        Cysharp.Threading.Tasks.UniTask<T> WaitForEventUniTaskAsync(CancellationToken cancellationToken = default);
        Cysharp.Threading.Tasks.UniTask<T> WaitForEventUniTaskAsync(TimeSpan timeout,
            CancellationToken cancellationToken = default);
#endif
    }
}