using BoschingMachine.Bipedal;
using BoschingMachine.Interactables;
using UnityEngine;

namespace BoschingMachine.SignalGroups
{
    public class SignalGroupIndexButton : Interactable
    {
        [SerializeField] protected int index;

        SignalGroup signalGroup;

        public override bool CanInteract => true;

        private void Awake()
        {
            signalGroup = SignalGroup.GetOrCreate(gameObject);
        }

        protected override void FinishInteract(Biped biped)
        {
            Call();
        }

        public virtual void Call()
        {
            signalGroup.Call(index);
        }
    }
}
