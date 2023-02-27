using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine.Bipedal
{
    [System.Serializable]
    public sealed class BipedalMovement
    {
        [SerializeField] float moveSpeed = 10.0f;
        [SerializeField] float groundAcceleration = 80.0f;

        [Space]
        [SerializeField] float airMoveAcceleration = 8.0f;

        [Space]
        [SerializeField] float jumpHeight = 3.5f;
        [SerializeField] float upGravity = 2.0f;
        [SerializeField] float downGravity = 3.0f;
        [SerializeField] float jumpSpringPauseTime = 0.1f;

        [Space]
        [SerializeField] float springDistance = 1.2f;
        [SerializeField] float springForce = 250.0f;
        [SerializeField] float springDamper = 15.0f;
        [SerializeField] float groundCheckRadius = 0.4f;
        [SerializeField] float groundMaxSlope = 46.0f;
        [SerializeField] LayerMask groundCheckMask = 0b1;

        bool previousJumpState;
        float lastJumpTime;

        public float MoveSpeed => moveSpeed;

        public float DistanceToGround { get; private set; }
        public Vector3 GroundNormal { get; set; }
        public bool IsGrounded => DistanceToGround < 0.0f && (Mathf.Acos(Mathf.Clamp01(Vector3.Dot(Vector3.up, GroundNormal))) * Mathf.Rad2Deg) < groundMaxSlope;
        public GameObject Ground { get; private set; }
        public Rigidbody GroundRigidbody { get; private set; }

        public Vector2 PlanarSpeed(Rigidbody rigidbody) => new Vector2(rigidbody.velocity.x, rigidbody.velocity.z);
        public Vector2 LocalPlanarSpeed(Rigidbody rigidbody)
        {
            if (!IsGrounded) return PlanarSpeed(rigidbody);
            if (!GroundRigidbody) return PlanarSpeed(rigidbody);

            return PlanarSpeed(rigidbody) - new Vector2(GroundRigidbody.velocity.x, GroundRigidbody.velocity.z);
        }

        public void Move(Rigidbody rigidbody, Vector3 moveDirection, bool jump)
        {
            DistanceToGround = GetDistanceToGround(rigidbody) - springDistance;

            MoveCharacter(rigidbody, moveDirection);

            if (jump && !previousJumpState)
            {
                TryJump(rigidbody);
            }
            previousJumpState = jump;

            ApplySpring(rigidbody);
            ApplyGravity(rigidbody, jump);
        }

        private void ApplySpring(Rigidbody rigidbody)
        {
            if (IsGrounded && Time.time > lastJumpTime + jumpSpringPauseTime)
            {
                float contraction = 1.0f - (DistanceToGround + springDistance) / springDistance;
                Vector3 moment = Vector3.up * contraction * springForce;
                moment -= Vector3.up * rigidbody.velocity.y * springDamper;

                AddMomentToSelfAndGround(rigidbody, moment);
            }
        }

        private void ApplyGravity(Rigidbody rigidbody, bool jump)
        {
            rigidbody.useGravity = false;
            rigidbody.AddForce(GetGravity(rigidbody, jump), ForceMode.Acceleration);
        }

        private void MoveCharacter(Rigidbody rigidbody, Vector3 moveDirection)
        {
            moveDirection = Vector3.ClampMagnitude(moveDirection, 1.0f);

            if (IsGrounded)
            {
                Vector3 target = moveDirection * moveSpeed;
                Vector3 current = rigidbody.velocity;

                Vector3 delta = Vector3.ClampMagnitude(target - current, moveSpeed);
                delta.y = 0.0f;

                Vector3 moment = delta / moveSpeed * groundAcceleration;

                AddMomentToSelfAndGround(rigidbody, moment);
            }
            else
            {
                rigidbody.AddForce(moveDirection * airMoveAcceleration, ForceMode.Acceleration);
            }
        }

        private void TryJump(Rigidbody rigidbody)
        {
            if (IsGrounded)
            {
                float gravity = Vector3.Dot(Vector3.down, GetGravity(rigidbody, true));
                float jumpForce = Mathf.Sqrt(2.0f * gravity * jumpHeight);

                if (rigidbody.velocity.y < 0.0f) rigidbody.AddForce(Vector3.up * -rigidbody.velocity.y, ForceMode.VelocityChange);

                AddMomentToSelfAndGround(rigidbody, Vector3.up * jumpForce, ForceMode.Impulse);

                lastJumpTime = Time.time;
            }
        }

        private Vector3 GetGravity(Rigidbody rigidbody, bool jump)
        {
            float scale = upGravity;
            if (!jump)
            {
                scale = downGravity;
            }
            else if (rigidbody.velocity.y < 0.0f)
            {
                scale = downGravity;
            }

            return Physics.gravity * scale;
        }

        public void AddMomentToSelfAndGround(Rigidbody self, Vector3 moment, ForceMode forceMode = ForceMode.Force)
        {
            Vector3 force = moment * self.mass;
            self.AddForce(force, forceMode);
            if (GroundRigidbody && IsGrounded)
            {
                GroundRigidbody.AddForce(-force, forceMode);
            }
        }

        public float GetDistanceToGround(Rigidbody rigidbody)
        {
            List<RaycastHit> hits = new List<RaycastHit>(Physics.SphereCastAll(rigidbody.position + Vector3.up * groundCheckRadius, groundCheckRadius, Vector3.down, springDistance, groundCheckMask));
            
            RaycastHit? hit = null;
            foreach (var other in hits)
            {
                if (other.rigidbody == rigidbody) continue;

                if (hit.HasValue) hit = GetBetterHit(hit.Value, other);
                else hit = other;
            }

            if (hit.HasValue)
            {
                Ground = hit.Value.transform.gameObject;
                GroundRigidbody = hit.Value.rigidbody;
                GroundNormal = hit.Value.normal;
                return hit.Value.distance;
            }
            else
            {
                Ground = null;
                GroundRigidbody = null;
                GroundNormal = Vector3.zero;
                return float.PositiveInfinity;
            }
        }

        public RaycastHit GetBetterHit (RaycastHit a, RaycastHit b)
        {
            if (!a.rigidbody) return a;
            if (!b.rigidbody) return b;

            if (a.rigidbody.isKinematic) return a;
            if (b.rigidbody.isKinematic) return b;

            if (a.rigidbody.mass == b.rigidbody.mass) return a.distance < b.distance ? a : b;
            else return a.rigidbody.mass > b.rigidbody.mass ? a : b;
        }

        public void Look(Rigidbody rigidbody, Transform head, Vector2 lookRotation)
        {
            rigidbody.rotation = Quaternion.Euler(0.0f, lookRotation.x, 0.0f);
            head.rotation = Quaternion.Euler(lookRotation.y, lookRotation.x, 0.0f);
        }
    }
}
