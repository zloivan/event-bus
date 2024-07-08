using Utilities.EventBuss.abstractions;

namespace Utilities.EventBuss.Samples
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