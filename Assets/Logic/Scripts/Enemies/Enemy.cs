using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoschingMachine.Bipedal;

namespace BoschingMachine.Enemies
{
    public class Enemy : Biped
    {
        [SerializeField] float rotationSmoothTime;

        Vector2 lookRotation;
        Vector2 lookVelocity;

        Vector3 moveDirection;
        Vector2 targetLookRotation;

        public override Vector3 MoveDirection => moveDirection;
        public override Vector2 LookRotation => lookRotation;

        public void SetMoveDirection(Vector3 dir) => moveDirection = dir;

        protected override void Update()
        {
            base.Update();

            lookRotation.x = Mathf.SmoothDampAngle(lookRotation.x, targetLookRotation.x, ref lookVelocity.x, rotationSmoothTime);
            lookRotation.y = Mathf.SmoothDamp(lookRotation.y, targetLookRotation.y, ref lookVelocity.y, rotationSmoothTime);
        }

        public void SetLookDirection (Vector3 direction)
        {
            if (direction.sqrMagnitude < 0.01f) return;

            direction.Normalize();
            float yaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg; 
            float pitch = Mathf.Asin(direction.y) * Mathf.Rad2Deg;
            targetLookRotation = new Vector2(yaw, pitch);
        }
    }
}
