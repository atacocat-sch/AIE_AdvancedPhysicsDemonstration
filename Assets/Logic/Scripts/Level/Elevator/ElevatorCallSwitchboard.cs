using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine.Elevators
{
    public class ElevatorCallSwitchboard : MonoBehaviour
    {
        [SerializeField] ElevatorCallSwitch switchPrefab;
        [SerializeField] int rowMax = 1;
        [SerializeField] Vector2 pannelSize;
        [SerializeField] Vector3 offset;

        [Space]
        [Header("Gizmos")]
        [SerializeField] int floors = 2;

        List<ElevatorCallSwitch> instances;

        ElevatorGroup elevatorGroup;
        
        private void Start()
        {
            rowMax = Mathf.Max(1, rowMax);
            instances = new();

            elevatorGroup = GetComponentInParent<ElevatorGroup>();
            ReinstanceButtons();
        }

        private void ReinstanceButtons()
        {
            int floors = elevatorGroup.Floors.Length;
            while (floors > instances.Count)
            {
                var instance = Instantiate(switchPrefab, transform);
                instances.Add(instance);
            }
            while (instances.Count > floors)
            {
                var instance = instances[instances.Count - 1];
                instances.RemoveAt(instances.Count - 1);
                Destroy(instance);
            }

            int columns = (floors - 1) / rowMax;
            int rows = Mathf.Min(floors, rowMax);
            var buttonSpacing = pannelSize / new Vector2(columns + 1, rows + 1);

            for (int i = 0; i < floors; i++)
            {
                var instance = instances[i];
                var row = i % rows;
                var column = i / rows;

                var x = (columns / 2.0f) - column;
                var y = ((rows - 1.0f) / 2.0f) - row;
                var pos = (Vector3)(new Vector2(x, y) * buttonSpacing) + offset;

                instance.transform.localPosition = pos;
                instance.IndexOverride = i;
            }
        }

        private void OnDrawGizmosSelected()
        {
            elevatorGroup = GetComponentInParent<ElevatorGroup>();
            if (!elevatorGroup) return;

            Gizmos.matrix = transform.localToWorldMatrix;

            int columns = (floors - 1) / rowMax;
            int rows = Mathf.Min(floors, rowMax);
            var buttonSpacing = pannelSize / new Vector2(columns + 1, rows + 1);

            for (int i = 0; i < floors; i++)
            {
                int row = i % rows;
                int column = i / rows;

                var x = (columns / 2.0f) - column;
                var y = ((rows - 1.0f) / 2.0f) - row;
                var pos = (Vector3)(new Vector2(x, y) * buttonSpacing) + offset;

                Gizmos.DrawSphere(pos, 0.02f);
            }

            Gizmos.DrawWireCube(offset, pannelSize);
        }

        private void OnValidate()
        {
            rowMax = Mathf.Max(1, rowMax);
        }
    }
}
