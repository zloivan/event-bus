using System;
using System.Threading;
using IKhom.EventBusSystem.Runtime;
using UnityEngine;

namespace IKhom.EventBusSystem.Samples.EventBusExamples
{
    public class TestEventBussReceiver : MonoBehaviour
    {
        private EventBinding<TestEvent> _testEventBinding;
        private EventBinding<PlayerTestEvent> _playerTestEventBinding;

        private string _receivedPlayerName;
        private int _receivedPlayerHealth;
        private bool _usingCachedEvent;
        private string _asyncStatusMessage = "";

        private void OnEnable()
        {
            _testEventBinding = new EventBinding<TestEvent>(HandleTestEvent);
            _playerTestEventBinding = new EventBinding<PlayerTestEvent>(HandlePlayerTestEvent);

            //Action handler;
            //HandlePlayerTestEvent.RegisterHandler<PlayerTestEvent>();
            //EventBus<TestEvent>.Register();
            EventBus<TestEvent>.Register(_testEventBinding);
            EventBus<PlayerTestEvent>.Register(_playerTestEventBinding);

            _testEventBinding.Add(AdditionalTestEventHandler);

            // Check if there's already a cached PlayerTestEvent (useful for late subscribers)
            if (EventBus<PlayerTestEvent>.TryGetLastEvent(out var cachedEvent))
            {
                _receivedPlayerName = cachedEvent.Name;
                _receivedPlayerHealth = cachedEvent.Health;
                _usingCachedEvent = true;

                Debug.Log(
                    $"Using cached PlayerTestEvent: Name = {_receivedPlayerName}, Health = {_receivedPlayerHealth}");
            }
        }

        private void OnDisable()
        {
            EventBus<TestEvent>.Deregister(_testEventBinding);
            EventBus<PlayerTestEvent>.Deregister(_playerTestEventBinding);
        }

        private void AdditionalTestEventHandler(TestEvent obj)
        {
            Debug.Log("new handlers can be added after the registration of binding");
        }

        private void HandleTestEvent()
        {
            Debug.Log($"Received TestEvent!");
        }

        private void HandlePlayerTestEvent(PlayerTestEvent playerTestEvent)
        {
            _receivedPlayerName = playerTestEvent.Name;
            _receivedPlayerHealth = playerTestEvent.Health;
            _usingCachedEvent = false;

            Debug.Log($"Received PlayerTestEvent: Name = {_receivedPlayerName}, Health = {_receivedPlayerHealth}");
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(
                new Rect((float)Screen.width / 2 + 10, 10, (float)Screen.width / 2 - 20, Screen.height - 20),
                "Receiver", GUI.skin.window);
            GUILayout.BeginVertical();

            GUILayout.Label("Received Player Name: " + _receivedPlayerName);
            GUILayout.Label("Received Player Health: " + _receivedPlayerHealth);

            if (_usingCachedEvent)
            {
                GUILayout.Label("Using cached event data", GUI.skin.box);
            }

            if (GUILayout.Button("Check Last Event"))
            {
                CheckLastPlayerEvent();
            }

            if (GUILayout.Button("Clear Event Cache"))
            {
                EventBus<PlayerTestEvent>.ClearLastEvent();
                Debug.Log("Cleared PlayerTestEvent cache");
            }

            // Add async buttons
            if (GUILayout.Button("Wait for PlayerTestEvent Async"))
            {
                WaitForPlayerEventAsync(CancellationToken.None);
            }

            if (GUILayout.Button("Wait for TestEvent Async"))
            {
                WaitForTestEventAsync(CancellationToken.None);
            }

            GUILayout.Label(_asyncStatusMessage, GUI.skin.box);

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private async void WaitForPlayerEventAsync(CancellationToken cancellationToken)
        {
            try
            {
                _asyncStatusMessage = "Waiting for PlayerTestEvent...";

                // Wait for PlayerTestEvent with a 5-second timeout
                var playerEvent = await EventBus<PlayerTestEvent>.WaitForEventAsync(
                    TimeSpan.FromSeconds(5),
                    cancellationToken);

                _asyncStatusMessage = $"Async: Received {playerEvent.Name}, Health={playerEvent.Health}";
                Debug.Log($"Async received PlayerTestEvent: Name={playerEvent.Name}, Health={playerEvent.Health}");
            }
            catch (TimeoutException)
            {
                _asyncStatusMessage = "Async: Timed out waiting for event";
                Debug.Log("Timed out waiting for PlayerTestEvent");
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation
            }
        }

        private async void WaitForTestEventAsync(CancellationToken cancellationToken)
        {
            try
            {
                _asyncStatusMessage = "Waiting for TestEvent...";

                // Wait for TestEvent without a timeout
                await EventBus<TestEvent>.WaitForEventAsync(cancellationToken);

                _asyncStatusMessage = "Async: Received TestEvent";
                Debug.Log("Async received TestEvent");
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation
            }
        }

        private void CheckLastPlayerEvent()
        {
            // Different ways to access the cached event

            // Method 1: Using TryGetLastEvent with out parameter
            if (EventBus<PlayerTestEvent>.TryGetLastEvent(out var lastEvent))
            {
                Debug.Log($"Last PlayerTestEvent: Name = {lastEvent.Name}, Health = {lastEvent.Health}");
            }
            else
            {
                Debug.Log("No cached PlayerTestEvent available");
            }

            // Method 2: Using GetLastEvent directly (will return default if no event cached)
            var playerEvent = EventBus<PlayerTestEvent>.GetLastEvent();
            Debug.Log(
                $"GetLastEvent result: {(string.IsNullOrEmpty(playerEvent.Name) ? "No event" : playerEvent.Name)}");

            // Method 3: Check if event exists first
            if (EventBus<PlayerTestEvent>.HasLastEvent())
            {
                Debug.Log("PlayerTestEvent cache exists");
            }
            else
            {
                Debug.Log("No PlayerTestEvent in cache");
            }
        }
    }
}