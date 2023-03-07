using BoschingMachine.Bipedal;
using BoschingMachine.Interactables;
using UnityEngine;

namespace BoschingMachine.SignalGroups
{
    public class SignalGroupButton : Interactable
    {
        [SerializeField] protected int index;
        [SerializeField] protected string text = "Press Button";

        SignalGroup signalGroup;

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

        public override string BuildInteractString(string passthrough = "")
        {
            return base.BuildInteractString(text);
        }
    }
}
