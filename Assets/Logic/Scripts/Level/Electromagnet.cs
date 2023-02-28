using BoschingMachine.Bipedal;
using BoschingMachine.Interactables;
using BoschingMachine.Tags;
using UnityEngine;

namespace BoschingMachine
{
    public class Electromagnet : Interactable
    {
        [SerializeField] Vector3 poleMinOffset;
        [SerializeField] Vector3 poleMaxOffset;
        [SerializeField] float constant;
        [SerializeField] float minForce;
        [SerializeField] float maxForce;
        [SerializeField] bool active;
        [SerializeField] Tag magneticTag;

        public override bool CanInteract => true;

        float Radius => Mathf.Sqrt(constant / minForce);
        Vector3 PoleMin => transform.TransformPoint(poleMinOffset);
        Vector3 PoleMax => transform.TransformPoint(poleMaxOffset);

        protected override void FinishInteract(Biped biped)
        {
            active = !active;
        }

        public override string BuildInteractString(string passthrough = "")
        {
            return active ? "Deactivate Electromagnet" : "Activate Electromagnet";
        }

        private void FixedUpdate()
        {
            if (!active) return;

            var bodies = Tag.GetHoldersForTag(magneticTag);
            foreach (var body in bodies)
            {
                if (!body.TryGetComponent(out Rigidbody rigidbody)) return;

                var point = GetPoint(rigidbody);
                var vec = point - rigidbody.position;
                var sqrDist = vec.sqrMagnitude;
                var dir = vec.normalized;

                rigidbody.AddForce(dir * (constant / sqrDist));
            }
        }

        private Vector3 GetPoint(Rigidbody rigidbody)
        {
            var dist = (PoleMax - PoleMin).magnitude;
            var dir = (PoleMax - PoleMin).normalized;
            var dot = Vector3.Dot(rigidbody.position - PoleMin, dir);
            dot = Mathf.Clamp(dot, 0.0f, dist);

            return PoleMin + dir * dot;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(PoleMax, 0.1f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(PoleMin, 0.1f);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere((PoleMax + PoleMin) * 0.5f, Radius);
        }

        private void OnValidate()
        {
            minForce = Mathf.Max(minForce, 0.001f);
        }
    }
}
