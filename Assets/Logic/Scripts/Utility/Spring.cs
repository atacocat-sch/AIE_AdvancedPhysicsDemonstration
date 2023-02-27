using UnityEngine;

namespace BoschingMachine
{
    [System.Serializable]
    public class Spring
    {
        [SerializeField] float constant = 1000.0f;
        [SerializeField] float damper = 100.0f;
        [SerializeField] float maxForce = 600.0f;

        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public float Mass { get; set; } = 1.0f;

        public void Update (Vector3 target, float dt)
        {
            var diff = target - Position;
            Vector3 force = GetForce(diff);
            TryUseDiffForce(ref force, target, dt);
            Integrate(force, dt);
        }

        private Vector3 GetForce(Vector3 diff)
        {
            Vector3 force = diff * constant - Velocity * damper;
            force = Vector3.ClampMagnitude(force, maxForce);
            return force;
        }

        private void Integrate (Vector3 force, float dt)
        {
            Position += Velocity * dt;
            Velocity += force * (dt / Mass);
        }

        private Vector3 CalculateDifferenceForce (Vector3 target, float dt)
        {
            var diff = target - Position;
            Vector3 force = diff / dt * 0.5f -Velocity;
            return force * Mass / dt;
        }

        private void TryUseDiffForce(ref Vector3 force, Vector3 target, float dt)
        {
            var diffForce = CalculateDifferenceForce(target, dt);
            if (diffForce.sqrMagnitude < force.sqrMagnitude) force = diffForce;
        }

        public void Drive(Rigidbody self, Vector3 target) => Drive(self, null, target);
        public void Drive (Rigidbody self, Rigidbody other, Vector3 target)
        {
            var oldPos = Position;
            var oldVel = Velocity;
            var oldMass = Mass;

            Position = self.position;
            Velocity = self.velocity;
            Mass = self.mass;

            var diff = target - Position;
            var force = GetForce(diff);

            TryUseDiffForce(ref force, target, Time.deltaTime);

            self.AddForce(force);
            if (other) other.AddForce(-force);

            Position = oldPos;
            Velocity = oldVel;
            Mass = oldMass;
        }
    }
}
