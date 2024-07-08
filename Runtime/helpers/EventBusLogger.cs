using UnityEngine;

namespace Utilities.EventBuss.helpers
{
    public interface ILogger
    {
        public void Log(object message);
        public void LogWarning(object message);
        public void LogError(object message);
    }

    public class EventBusLogger : ILogger
    {
        public void Log(object message)
        {
#if DEBUG_EVENT_BUS
            if (Debug.isDebugBuild || Application.isEditor)
            {
                Debug.Log($"<b>EVENTBUS:</b> {message}");
            }
#endif
        }

        public void LogWarning(object message)
        {
            if (Debug.isDebugBuild || Application.isEditor)
            {
                Debug.LogWarning($"<b>EVENTBUS:</b> {message}");
            }
        }

        public void LogError(object message)
        {
            if (Debug.isDebugBuild || Application.isEditor)
            {
                Debug.LogError($"<b>EVENTBUS:</b> {message}");
            }
        }
    }
}