using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine
{
    [System.Serializable]
    public class ElevatorDoor
    {
        [SerializeField] Transform[] doors;
        [SerializeField] float doorOpenDistance;
        [SerializeField] float doorOpenTime;

        float doorPos;
        float doorVel;

        public bool IsDoorOpen { get; set; }
        public bool DoorsClosed => doorPos < 0.01f;

        public void FixedUpdate()
        {
            doorPos = Mathf.SmoothDamp(doorPos, IsDoorOpen ? 1.0f : 0.0f, ref doorVel, doorOpenTime);
            foreach (var door in doors)
            {
                door.localPosition = door.transform.localRotation * Vector3.right * doorPos * doorOpenDistance;
            }
        }
    }
}
