using System.Threading.Tasks;
using IKhom.EventBusSystem.Runtime;
using UnityEngine;

namespace IKhom.EventBusSystem.Samples.EventBusExamples
{
    public class TestEventBusSender : MonoBehaviour
    {
        [SerializeField]
        private int _health;

        [SerializeField]
        private string _name;

        [ContextMenu("Raise Player Test Event")]
        public void RaisePlayerTestEvent()
        {
            EventBus<PlayerTestEvent>.Raise(new PlayerTestEvent
            {
                Health = _health,
                Name = _name
            });
        }

        [ContextMenu("Raise Test Event")]
        public void RaiseTestEvent()
        {
            EventBus<TestEvent>.Raise(new TestEvent());
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, (float)Screen.width / 2 - 20, Screen.height - 20), "Sender",
                GUI.skin.window);
            GUILayout.BeginVertical();

            GUILayout.Label("Health");
            _health = int.Parse(GUILayout.TextField(_health.ToString()));

            GUILayout.Label("Name");
            _name = GUILayout.TextField(_name);

            if (GUILayout.Button("Raise Player Test Event"))
            {
                RaisePlayerTestEvent();
            }

            if (GUILayout.Button("Raise Test Event"))
            {
                RaiseTestEvent();
            }
            
            // Add a section to demonstrate event caching functionality
            GUILayout.Space(20);
            GUILayout.Label("Event Caching Demo", GUI.skin.box);
            
            if (GUILayout.Button("Get Last Player Event"))
            {
                if (EventBus<PlayerTestEvent>.TryGetLastEvent(out var lastEvent))
                {
                    Debug.Log($"Last cached PlayerTestEvent: Name = {lastEvent.Name}, Health = {lastEvent.Health}");
                }
                else
                {
                    Debug.Log("No PlayerTestEvent has been cached yet");
                }
            }
            
            if (GUILayout.Button("Check Has Event"))
            {
                bool hasEvent = EventBus<PlayerTestEvent>.HasLastEvent();
                Debug.Log($"Has cached PlayerTestEvent: {hasEvent}");
            }
            
            if (GUILayout.Button("Clear Event Cache"))
            {
                EventBus<PlayerTestEvent>.ClearLastEvent();
                Debug.Log("PlayerTestEvent cache cleared");
            }

            if (GUILayout.Button("Trigger Async Event Sequence"))
            {
                TriggerEventSequenceAsync();
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        
        public async void TriggerEventSequenceAsync()
        {
            // Raise a test event
            RaiseTestEvent();
    
            // Wait 2 seconds
            await Task.Delay(2000);
    
            // Then raise a player event
            RaisePlayerTestEvent();
    
            Debug.Log("Async event sequence completed");
        }
    }
}