using BoschingMachine.Bipedal;
using UnityEngine;

namespace BoschingMachine.Player.Modules
{
    [System.Serializable]
    public sealed class PlayerPickerUpper
    {
        [SerializeField] float grabRange;
        [SerializeField] float throwForce;
        [SerializeField] float maxForce;
        [SerializeField] float maxDistance;

        [Space]
        [SerializeField] float springForce;
        [SerializeField] float springDamper;

        [Space]
        [SerializeField] float rotationDamper;

        [Space]
        [SerializeField] LineRenderer lines;

        Rigidbody heldObject;

        public void FixedProcess(Rigidbody rigidbody, Transform holdTarget)
        {
            if (!heldObject) return;

            Vector3 vec = holdTarget.position - heldObject.position;
            Vector3 force = vec * springForce;
            force += -heldObject.velocity * springDamper;

            force = Vector3.ClampMagnitude(force, maxForce);

            if (vec.sqrMagnitude > maxDistance * maxDistance)
            {
                heldObject = null;
                return;
            }

            heldObject.AddForce(force);
            rigidbody.AddForce(-force);

            heldObject.AddTorque(-heldObject.angularVelocity * rotationDamper);
        }

        public void Update(Transform holdTarget)
        {
            if (heldObject)
            {
                lines.enabled = true;
                lines.SetPosition(0, heldObject.position);
                lines.SetPosition(1, holdTarget.position);
            }
            else
            {
                lines.enabled = false;
            }
        }

        public bool TryGrabOrDrop(Biped biped)
        {
            if (heldObject)
            {
                heldObject = null;
                return true;
            }
            else
            {
                Ray ray = new Ray(biped.Head.position, biped.Head.forward);
                if (Physics.Raycast(ray, out var hit, grabRange))
                {
                    if (hit.rigidbody)
                    {
                        heldObject = hit.rigidbody;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Throw(Biped biped)
        {
            if (!heldObject) return false;

            heldObject.AddForce(biped.Head.forward * throwForce, ForceMode.Impulse);
            heldObject.gameObject.AddComponent<MovingInterest>();
            heldObject = null;

            return true;
        }
    }
}
