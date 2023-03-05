using BoschingMachine.Bipedal;
using BoschingMachine.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine
{
    public class Elevator : MonoBehaviour, IElevatorDriver
    {
        [SerializeField] Vector3[] floors;
        [SerializeField] float smoothTime;
        [SerializeField] float maxSpeed;
        [SerializeField] float threshold = 0.1f;
        [SerializeField] int startingFloor;
 
        new Rigidbody rigidbody;
        Vector3 velocity;
        bool stopped;

        public int Floor { get; private set; }
        public Vector3[] Floors => floors;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Vector3 floorPoint = floors[Floor];
            stopped = (floorPoint - rigidbody.position).sqrMagnitude < threshold * threshold;
            
            if (stopped)
            {
                rigidbody.MovePosition(floorPoint);
            }
            else
            {
                rigidbody.MovePosition(Vector3.SmoothDamp(rigidbody.position, floorPoint, ref velocity, smoothTime, maxSpeed));
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < floors.Length - 1; i++)
            {
                Vector3 a = floors[i];
                Vector3 b = floors[i + 1];

                Gizmos.DrawLine(a, b);
            }
        }

        public void Call(int floor)
        {
            Floor = floor;
        }
    }

    public interface IElevatorDriver
    {
        int Floor { get; }
        void Call(int floor);
    }
}
