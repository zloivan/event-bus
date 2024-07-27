using IKhom.EventBusSystem.Runtime.abstractions;

namespace IKhom.EventBusSystem.Samples.EventBusExamples
{
    public struct TestEvent : IEvent
    {
    }

    public struct PlayerTestEvent : IEvent
    {
        public int Health;
        public string Name;
    }
}