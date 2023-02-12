using UnityEngine;
using BoschingMachine.Bipedal;

namespace BoschingMachine.Interactables
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] string interactName;
        [SerializeField] float interactTime;

        public virtual string InteractName => interactName;
        public float InteractTime => interactTime;
        public float GetInteractPercent(Biped biped) => biped == user ? interactPercent : 0.0f;

        Biped user;
        float interactPercent;
        int useFrame;

        public bool TryInteract(Biped biped, System.Action<float> partialCallback, System.Action finishCallback)
        {
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
                interactPercent += Time.deltaTime / interactTime;
                
                InteractTick(biped, interactPercent);
                partialCallback?.Invoke(interactPercent);
            }
            else
            {
                interactPercent = 0.0f;

                FinishInteract(biped);
                finishCallback?.Invoke();
            }

            useFrame = Time.frameCount;
            return true;
        }

        protected virtual void InteractTick(Biped biped, float t) { }
        protected abstract void FinishInteract(Biped biped);
    }
}
