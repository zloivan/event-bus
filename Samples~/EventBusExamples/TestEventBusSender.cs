using System;
using UnityEngine;

namespace Utilities.EventBuss.Samples
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

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}