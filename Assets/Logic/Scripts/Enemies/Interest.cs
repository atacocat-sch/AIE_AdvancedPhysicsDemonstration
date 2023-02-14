using UnityEngine;

namespace BoschingMachine
{
    public abstract class Interest : MonoBehaviour
    {
        public bool Expired { get; protected set; }

        private void Update()
        {
            if (Expired)
            {
                Destroy(this);
            }
        }
    }
}
