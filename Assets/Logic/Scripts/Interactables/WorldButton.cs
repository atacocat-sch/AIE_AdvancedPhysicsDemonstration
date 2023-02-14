using BoschingMachine.Bipedal;
using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine.Interactables
{
    public class WorldButton : Interactable, IStateDrivable
    {
        [Space]
        [SerializeField] List<GameObject> connections;

        public bool State { get; set; }

        protected override void FinishInteract(Biped biped)
        {
            State = !State;

            foreach (var connection in connections)
            {
                if (connection.TryGetComponent(out IStateDrivable stateDrivable))
                {
                    stateDrivable.State = State;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            foreach (var connection in connections)
            {
                Gizmos.DrawLine(transform.position, connection.transform.position);
            }
            Gizmos.color = Color.white;
        }
    }
}
