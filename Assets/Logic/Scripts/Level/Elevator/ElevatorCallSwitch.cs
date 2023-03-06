using BoschingMachine.Bipedal;
using BoschingMachine.Interactables;
using UnityEngine;
using UnityEngine.Events;

namespace BoschingMachine.Elevators
{
    public class ElevatorCallSwitch : Interactable
    {
        [SerializeField] UnityEvent<string> setupCallback;

        ElevatorGroup elevatorGroup;
        Elevator elevator;
        int index;

        public override bool CanInteract => true;
        public int? IndexOverride { get; set; } = null;

        private void Start()
        {
            index = GetButtonCallIndex();
            setupCallback.Invoke(Elevator.IndexToFloorName(index));
        }

        public int GetButtonCallIndex()
        {
            LazyInit();

            if (IndexOverride.HasValue) return IndexOverride.Value;

            if (!elevatorGroup) return 0;

            var best = 0;
            for (int i = 1; i < elevatorGroup.Floors.Length; i++)
            {
                var d1 = Mathf.Abs(elevatorGroup.Floors[i] - transform.position.y);
                var d2 = Mathf.Abs(elevatorGroup.Floors[best] - transform.position.y);

                if (d1 < d2) best = i;
            }

            return best;
        }

        private void LazyInit()
        {
            if (!elevatorGroup) elevatorGroup = GetComponentInParent<ElevatorGroup>();
            if (!elevator) elevator = GetComponentInParent<Elevator>();
        }

        protected override string InoperableAppend => "En Route";
        public override string BuildInteractString(string passthrough = "")
        {
            return base.BuildInteractString("Call Elevator");
        }

        protected override void FinishInteract(Biped biped)
        {
            if (elevator) elevator.Request(index);
            else elevatorGroup.Request(index);
        }
    }
}
