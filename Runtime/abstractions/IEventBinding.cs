using System;

namespace IKhom.EventBusSystem.Runtime.abstractions
{
    internal interface IEventBinding<T>
    {
        public Action<T> OnEvent { get; set; }
        public Action OnEventNoArgs { get; set; }
    }
}