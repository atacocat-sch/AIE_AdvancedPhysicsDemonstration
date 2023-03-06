using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine.Elevators
{
    public class ElevatorGroup : MonoBehaviour
    {
        [SerializeField] float[] floors;

        public float[] Floors => floors;
        public bool[] Requests { get; private set; }

        public List<Elevator> Elevators { get; } = new();

        private void Awake()
        {
            Requests = new bool[floors.Length];
        }

        private void Update()
        {
            if (Requests.Length != floors.Length)
            {
                var old = Requests;
                Requests = new bool[floors.Length];
                for (int i = 0; i < old.Length && i < Requests.Length; i++)
                {
                    Requests[i] = old[i];
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (floors == null) return;
            foreach (var floor in floors)
            {
                Gizmos.DrawCube(HeightToPoint(floor), new Vector3(1, 0, 1));
            }
        }

        public void Request(int i)
        {
            Requests[i] = true;   
        }

        public Vector3 HeightToPoint(float h) => new Vector3(transform.position.x, h, transform.position.z);

        public enum ElevatorDirection
        {
            Up,
            Down,
            Waiting,
        }
    }
}
