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

        private void OnEnable()
        {
            _testEventBinding = new EventBinding<TestEvent>(HandleTestEvent);
            _playerTestEventBinding = new EventBinding<PlayerTestEvent>(HandlePlayerTestEvent);

            EventBus<TestEvent>.Register(_testEventBinding);
            EventBus<PlayerTestEvent>.Register(_playerTestEventBinding);
            
            _testEventBinding.Add(AdditionalTestEventHandler);
        }

        private void AdditionalTestEventHandler(TestEvent obj)
        {
            Debug.Log("new handlers can be added after the registration of binding");
        }

        private void OnDisable()
        {
            EventBus<TestEvent>.Deregister(_testEventBinding);
            EventBus<PlayerTestEvent>.Deregister(_playerTestEventBinding);
        }

        private void HandleTestEvent()
        {
            Debug.Log($"Received TestEvent!");
        }

        private void HandlePlayerTestEvent(PlayerTestEvent playerTestEvent)
        {
            _receivedPlayerName = playerTestEvent.Name;
            _receivedPlayerHealth = playerTestEvent.Health;
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

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}