using UnityEngine;

namespace BoschingMachine
{
    [RequireComponent(typeof(Rigidbody))]
    public class MovingInterest : Interest
    {
        const float expiredTime = 5.0f;

        new Rigidbody rigidbody;

        float stillTime;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (rigidbody.IsSleeping())
            {
                stillTime += Time.deltaTime;
            }

            if (stillTime > expiredTime)
            {
                Expired = true;
            }
        }
    }
}
