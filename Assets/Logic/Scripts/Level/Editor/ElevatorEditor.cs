using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace BoschingMachine
{
    [CustomEditor(typeof(Elevator))]
    public class ElevatorEditor : UnityEditor.Editor
    {
        new Elevator target => base.target as Elevator;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            int floor = -1;
            for (int i = 0; i < target.Floors.Length; i++)
            {
                if (GUILayout.Button($"Goto Floor {i + 1}"))
                {
                    floor = i;
                }
            }
            if (EditorGUI.EndChangeCheck() && floor != -1)
            {
                Undo.RecordObject(target.transform, "Elevator Warp");
                target.transform.position = target.Floors[floor];
            }
        }

        private void OnSceneGUI()
        {
            EditorGUI.BeginChangeCheck();
            var floors = new Vector3[target.Floors.Length];
            target.Floors.CopyTo(floors, 0);

            for (int i = 0; i < floors.Length; i++)
            {
                floors[i] = Handles.PositionHandle(floors[i], Quaternion.identity);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Floor Position");
                floors.CopyTo(target.Floors, 0);
            }
        }
    }
}
