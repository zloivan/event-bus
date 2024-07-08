using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Utilities.EventBuss.abstractions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utilities.EventBuss.helpers
{
    public static class EventBusUtilities
    {
        private static IReadOnlyList<Type> EventType { get; set; }
        private static IReadOnlyList<Type> EventBusTypes { get; set; }
        private static ILogger _logger;

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        public static void InitializeEditor()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
            EditorApplication.playModeStateChanged += OnPlayModeStateChange;
        }

        /// <summary>
        /// Clear all busses on exit from play mode
        /// </summary>
        private static void OnPlayModeStateChange(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                ClearAllBuses();
            }
        }
#endif

        /// <summary>
        /// Initializes the event buses at runtime before the scene loads.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            _logger = new EventBusLogger();
            EventType = PredefinedAssemblyUtils.GetType(typeof(IEvent));
            EventBusTypes = InitializeAllBuses();
        }

        /// <summary>
        /// Initializes all event buses for the predefined event types.
        /// </summary>
        /// <returns>A list of initialized event bus types.</returns>
        private static List<Type> InitializeAllBuses()
        {
            var allBusTypes = new List<Type>();
            var typedef = typeof(EventBus<>);

            foreach (var eventType in EventType)
            {
                var busType = typedef.MakeGenericType(eventType);
                allBusTypes.Add(busType);
                _logger.Log($"Initialized EventBus<{eventType.Name}>");
            }

            return allBusTypes;
        }

        /// <summary>
        /// Clears all event buses.
        /// </summary>
        public static void ClearAllBuses()
        {
            _logger.Log("Clearing all buses...");

            foreach (var buss in EventBusTypes)
            {
                var clearMethod = buss.GetMethod("Clear", BindingFlags.Static | BindingFlags.NonPublic);
                if (clearMethod == null)
                {
                    _logger.LogWarning(
                        $"No clear method found for EventBus<{buss.Name}>, update method name in EventBusUtilities class if method were renamed!");
                    continue;
                }

                clearMethod.Invoke(null, null);
            }
        }
    }
}