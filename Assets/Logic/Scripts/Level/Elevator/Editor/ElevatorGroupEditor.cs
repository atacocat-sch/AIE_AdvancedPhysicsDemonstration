using UnityEngine;
using UnityEditor;
using BoschingMachine.Elevators;

namespace BoschingMachine.Editor
{
    [CustomEditor(typeof(ElevatorGroup))]
    public class ElevatorGroupEditor : UnityEditor.Editor
    {
        private void OnEnable()
        {
            EditorApplication.update += Repaint;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Repaint;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying) return;

            ElevatorGroup target = this.target as ElevatorGroup;
            if (!target) return;
            if (target.Floors == null) return;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                for (int i = 0; i < target.Floors.Length; i++)
                {
                    if (GUILayout.Button($"Goto {Elevator.IndexToFloorName(i)}"))
                    {
                        target.Request(i);
                    }
                }
            }

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                bool none = true;
                for (int i = 0; i < target.Floors.Length; i++)
                {
                    if (target.Requests[i])
                    {
                        GUILayout.Label($"Request for Floor {Elevator.IndexToFloorName(i)}");
                        none = false;
                    }
                }

                if (none)
                {
                    GUILayout.Label($"Requests Empty");
                }
            }
        }
    }
}
