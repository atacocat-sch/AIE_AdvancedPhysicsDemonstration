using BoschingMachine.Bipedal;
using BoschingMachine.Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine
{
    public class Elevator : Interactable
    {
        [SerializeField] Vector3[] floors;
        [SerializeField] float smoothTime;
        [SerializeField] float maxSpeed;
        [SerializeField] float threshold = 0.1f;

        int floor;
        int direction;
        new Rigidbody rigidbody;
        Vector3 velocity;
        bool stopped;

        public override bool CanInteract => stopped;

        protected override void FinishInteract(Biped biped)
        {
            if (floor + direction >= floors.Length) direction = -1;
            if (floor + direction < 0) direction = 1;

            floor += direction;
        }

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            direction = 1;
        }

        private void FixedUpdate()
        {
            Vector3 floorPoint = floors[floor];
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

        public override string BuildInteractString(string passthrough = "")
        {
            return "Move Elevator";
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
    }
}
