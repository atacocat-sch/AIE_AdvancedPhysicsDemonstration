using System.Collections;
using System.Collections.Generic;
using BoschingMachine.Bipedal;
using BoschingMachine.Interactables;
using UnityEngine;

namespace BoschingMachine
{
    public class ElevatorCallSwitch : Interactable
    {
        [SerializeField] GameObject elevatorObject;
        [SerializeField] int callIndex;

        IElevatorDriver elevator;

        public override bool CanInteract => true;

        private void Awake()
        {
            elevator = GetComponent<IElevatorDriver>();
        }

        protected override void FinishInteract(Biped biped)
        {
            
        }
    }
}
