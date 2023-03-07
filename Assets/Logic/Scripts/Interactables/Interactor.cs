using UnityEngine;
using BoschingMachine.Bipedal;
using BoschingMachine.Interactables;
using TMPro;
using UnityEngine.UI;

namespace BoschingMachine
{
    [System.Serializable]
    public class Interactor
    {
        [SerializeField] float interactRange;
        [SerializeField] InteractorUIHandler uiHandler;

        public Interactable CurrentInteractable { get; private set; }

        Biped biped;

        public void FixedUpdate ()
        {
            uiHandler.FixedUpdate();
        }

        public void Update(Biped biped, bool use)
        {
            uiHandler.UpdateUI(this, biped);
            if (use)
            {
                if (CurrentInteractable)
                {
                    TryInteract(biped, CurrentInteractable);
                }
                else if (TryGetLookingAt(biped, out Interactable interactable))
                {
                    TryInteract(biped, interactable);
                }
            }
            else
            {
                if (CurrentInteractable) CurrentInteractable.CancelInteract(biped);
                CurrentInteractable = null;
            }
        }

        public bool TryGetLookingAt(Biped biped, out Interactable interactable)
        {
            Ray ray = new Ray(biped.Head.position, biped.Head.forward);
            if (Physics.Raycast(ray, out var hit, interactRange))
            {
                interactable = hit.collider.GetComponentInParent<Interactable>();
                if (interactable)
                {
                    return true;
                }
            }

            interactable = null;
            return false;
        }

        private bool TryInteract(Biped biped, Interactable interactable)
        {
            this.biped = biped;

            if ((interactable.transform.position - biped.Head.position).sqrMagnitude > interactRange * interactRange)
            {
                CurrentInteractable = null;
                return false;
            }

            if (interactable.TryInteract(biped, null, FinishInteractCallback))
            {
                CurrentInteractable = interactable;
                return true;
            }

            return false;
        }

        private void FinishInteractCallback()
        {
            biped.Unfreeze();
            CurrentInteractable = null;
        }
    }

    [System.Serializable]
    public class InteractorUIHandler
    {
        [SerializeField] CanvasGroup hoverGroup;
        [SerializeField] TMP_Text label;
        [SerializeField] Image progressBar;
        [SerializeField] SeccondOrderDynamicsF spring;

        float springTarget;

        public void FixedUpdate ()
        {
            spring.Loop(springTarget, null, Time.deltaTime);
        }

        public void UpdateUI(Interactor interactor, Biped biped)
        {
            springTarget = 1.0f;

            Interactable interactable = interactor.CurrentInteractable;
            if (!interactable) interactor.TryGetLookingAt(biped, out interactable);

            if (interactable)
            {
                var text = interactable.BuildInteractString();
                if (!string.IsNullOrEmpty(text))
                {
                    label.text = interactable.BuildInteractString();
                    progressBar.transform.localScale = new Vector3(interactable.GetInteractPercent(biped), 1.0f, 1.0f);
                }
                else springTarget = 0.0f;
            }
            else springTarget = 0.0f;

            hoverGroup.transform.localScale = Vector3.one * spring.Position;

            if (spring.Position < 0.0f)
            {
                spring.Position = 0.0f;
                spring.Velocity = 0.0f;
            }
        }
    }
}
