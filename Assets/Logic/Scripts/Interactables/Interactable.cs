using UnityEngine;
using BoschingMachine.Bipedal;

namespace BoschingMachine.Interactables
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] float interactTime;

        public float InteractTime => interactTime;
        public float GetInteractPercent(Biped biped) => biped == user ? interactPercent : 0.0f;
        public virtual bool CanInteract => true;

        protected virtual string InoperableAppend => "Inoperable";

        Biped user;
        float interactPercent;
        int useFrame;

        public bool TryInteract(Biped biped, System.Action<float> partialCallback, System.Action finishCallback)
        {
            if (!CanInteract) return false;

            if (biped != user)
            {
                if (Time.frameCount <= useFrame + 1)
                {
                    return false;
                }
                else
                {
                    user = biped;
                    interactPercent = 0.0f;
                }
            }

            if (interactPercent < 1.0f)
            {
                if (interactTime < Time.deltaTime) interactPercent = 1.0f;
                else interactPercent += Time.deltaTime / interactTime;

                InteractTick(biped, interactPercent);
                partialCallback?.Invoke(interactPercent);

                if (interactPercent >= 1.0f)
                {
                    interactPercent = 1.0f;

                    FinishInteract(biped);
                    finishCallback?.Invoke();
                }
            }

            useFrame = Time.frameCount;
            return true;
        }

        public virtual void CancelInteract(Biped user)
        {
            if (user != this.user) return;

            interactPercent = 0.0f;
            this.user = null;
            useFrame = 0;
        }

        protected virtual void InteractTick(Biped biped, float t) { }
        protected abstract void FinishInteract(Biped biped);

        public virtual string BuildInteractString(string passthrough = "")
        {
            return CanInteract ? passthrough : TMPUtil.Color($"{passthrough} [{InoperableAppend}]", Color.gray);
        }
    }
}
