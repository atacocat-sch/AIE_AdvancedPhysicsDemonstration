using UnityEngine;

namespace BoschingMachine
{
    public class GoalObject : MonoBehaviour
    {
        bool captured;

        public static int GoalsLeft { get; private set; } = 0;

        private void Start()
        {
            captured = true;
            SetCaptured(false);
        }

        public void SetCaptured(bool v)
        {
            if (captured == v) return;
            
            captured = v;
            if (v) GoalsLeft--;
            else GoalsLeft++;
        }
    }
}
