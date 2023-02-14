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

        float lookSpeed;

        public override Vector3 MoveDirection => moveDirection;
        public override Vector2 LookRotation => lookRotation;

        public void SetMoveDirection(Vector3 dir) => moveDirection = new Vector3(dir.x, 0.0f, dir.z);
        public void MoveToPoint(Vector3 pos, float holdDistance = 1.0f, float speed = 1.0f)
        {
            Vector3 vec = pos - transform.position;
            vec.y = 0.0f;
            if (vec.sqrMagnitude < holdDistance * holdDistance)
            {
                moveDirection = Vector3.zero;
            }
            else
            {
                moveDirection = vec.normalized * speed;
            }
        }

        protected override void Update()
        {
            base.Update();

            lookRotation.x = Mathf.SmoothDampAngle(lookRotation.x, targetLookRotation.x, ref lookVelocity.x, rotationSmoothTime / lookSpeed);
            lookRotation.y = Mathf.SmoothDamp(lookRotation.y, targetLookRotation.y, ref lookVelocity.y, rotationSmoothTime / lookSpeed);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            moveDirection = Vector3.zero;
        }

        public void SetLookDirection (Vector3 direction, float speed)
        {
            lookSpeed = speed;

            if (direction.sqrMagnitude < 0.01f) return;

            direction.Normalize();
            float yaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg; 
            float pitch = -Mathf.Asin(direction.y) * Mathf.Rad2Deg;
            targetLookRotation = new Vector2(yaw, pitch);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, MoveDirection * 3.0f);
            Gizmos.color = Color.white;
        }
    }
}
