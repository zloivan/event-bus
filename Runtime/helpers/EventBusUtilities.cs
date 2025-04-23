using System;
using System.Collections.Generic;
using System.Reflection;
using IKhom.EventBusSystem.Runtime.abstractions;
using UnityEditor;
using UnityEngine;

namespace IKhom.EventBusSystem.Runtime.helpers
{
    /// <summary>
    /// Utility class for automatic event bus initialization and cleanup.
    /// Handles registration of event types, initialization of all buses, and
    /// cleanup of buses when exiting play mode.
    /// </summary>
    internal static class EventBusUtilities
    {
        private static IReadOnlyList<Type> EventType { get; set; }
        private static IReadOnlyList<Type> EventBusTypes { get; set; }
        private static ILogger _logger;

#if UNITY_EDITOR
        /// <summary>
        /// Initializes the utility in the editor.
        /// Registers for play mode state changes to handle cleanup.
        /// </summary>
        [InitializeOnLoadMethod]
        public static void InitializeEditor()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
            EditorApplication.playModeStateChanged += OnPlayModeStateChange;
        }

        /// <summary>
        /// Handles play mode state changes.
        /// Clears all buses when exiting play mode to prevent memory leaks.
        /// </summary>
        /// <param name="state">The current play mode state.</param>
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
        /// Discovers all event types and initializes corresponding event buses.
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
        /// Creates an EventBus for each discovered event type.
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
        /// Invokes the Clear method on each event bus to release resources and remove bindings.
        /// </summary>
        private static void ClearAllBuses()
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