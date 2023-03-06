using BoschingMachine.Bipedal;
using UnityEngine;
using System.Linq;
using BoschingMachine.Tags;

namespace BoschingMachine.Player.Modules
{
    [System.Serializable]
    public sealed class PlayerPickerUpper
    {
        [SerializeField] float grabRange;
        [SerializeField] float throwSpeed;
        [SerializeField] float maxDistance;

        [Space]
        [SerializeField] Spring spring;

        [Space]
        [SerializeField] float rotationDamper;

        [Space]
        [SerializeField] LineRenderer lines;
        [SerializeField] float lineVolume;
        [SerializeField] PlayerPickerUpperUI ui;
        [SerializeField] Tag ignoreTag;

        public Rigidbody HeldObject { get; private set; }
        public Rigidbody LookingAt { get; private set; }

        public void FixedProcess(Biped biped, Transform holdTarget)
        {
            TryGetLookingAt(biped);

            if (!HeldObject) return;

            Vector3 vec = holdTarget.position - HeldObject.position;

            if (vec.sqrMagnitude > maxDistance * maxDistance)
            {
                HeldObject = null;
                return;
            }

            spring.Drive(HeldObject, biped.Rigidbody, holdTarget.position);

            HeldObject.AddTorque(-HeldObject.angularVelocity * rotationDamper);
        }

        public void Update(Transform holdTarget)
        {
            if (HeldObject)
            {
                lines.enabled = true;

                int segments = 16;
                lines.positionCount = segments;
                for (int i = 0; i < segments; i++)
                {
                    var p = i / (segments - 1.0f);
                    lines.SetPosition(i, Vector3.Lerp(HeldObject.position, holdTarget.position, p));
                }

                float distance = (HeldObject.position - holdTarget.position).magnitude;
                float factor = Mathf.Sqrt(lineVolume / distance);
                if (factor > 1.0f) factor = 1.0f;

                lines.widthCurve.MoveKey(1, new Keyframe(0.5f, factor));
            }
            else
            {
                lines.enabled = false;
            }

            ui.Update(this);
        }

        public bool TryGrabOrDrop(Biped biped)
        {
            if (HeldObject)
            {
                HeldObject = null;
                return true;
            }
            else if (LookingAt)
            {
                HeldObject = LookingAt;
                return true;
            }

            return false;
        }

        public bool Throw(Biped biped)
        {
            if (!HeldObject) return false;

            Vector3 throwForce = biped.Head.forward * throwSpeed * HeldObject.mass;
            throwForce = Vector3.ClampMagnitude(throwForce, spring.maxForce);

            HeldObject.AddForce(throwForce, ForceMode.Impulse);
            HeldObject.gameObject.AddComponent<MovingInterest>();
            HeldObject = null;

            return true;
        }

        public void TryGetLookingAt(Biped biped)
        {
            LookingAt = null;

            Ray ray = new Ray(biped.Head.position, biped.Head.forward);

            var results = Physics.RaycastAll(ray, grabRange).OrderBy(a => a.distance).Where(a => !a.transform.HasTag(ignoreTag));

            if (results.Count() == 0) return;

            var hit = results.First();

            if (!hit.rigidbody) return;
            if (hit.rigidbody.isKinematic) return;

            LookingAt = hit.rigidbody;
            return;
        }
    }
}
