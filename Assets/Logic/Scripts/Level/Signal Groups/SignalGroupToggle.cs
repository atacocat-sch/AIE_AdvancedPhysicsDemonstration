using BoschingMachine.Bipedal;
using BoschingMachine.SignalGroups;
using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine.Interactables
{
    public class SignalGroupToggle : Interactable
    {
        [Space]
        [SerializeField] string displayText;

        SignalGroup signalGroup;

        private void Awake()
        {
            signalGroup = SignalGroup.GetOrCreate(gameObject);
        }

        protected override void FinishInteract(Biped biped)
        {
            signalGroup.Call(!signalGroup.StateB);
        }

        public override string BuildInteractString(string passthrough = "")
        {
            return base.BuildInteractString(displayText);
        }
    }
}
