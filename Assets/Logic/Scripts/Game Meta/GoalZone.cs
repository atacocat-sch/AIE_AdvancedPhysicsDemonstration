using UnityEngine;

namespace BoschingMachine
{
    public class GoalZone : MonoBehaviour
    {
        [SerializeField] Signal winSignal;
        
        static bool done;

        private void Awake()
        {
            done = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (done) return;

            GoalObject goalObject = other.GetComponentInParent<GoalObject>();
            if (goalObject)
            {
                goalObject.SetCaptured(true);

                if (GoalObject.GoalsLeft == 0)
                {
                    winSignal.Raise(this, null);
                    done = true;
                }
            }
        }
    }
}
