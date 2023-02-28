using BoschingMachine.Animbites;
using BoschingMachine.Bipedal;
using UnityEngine;

namespace BoschingMachine.Interactables
{
    public class Dispenser : Interactable
    {
        [SerializeField] GameObject prefab;
        [SerializeField] Transform spawnpoint;
        [SerializeField] string itemName;
        [SerializeField] string itemPlural;

        [Space]
        [SerializeField] bool limitDeployment;
        [SerializeField] int capacity;
        [SerializeField] int usesLeft;
        [SerializeField] float rechargeTime;
        [SerializeField] int rechargeCount;

        [Space]
        [SerializeField] Bounds clogBounds;

        [Space]
        [SerializeField] SquashAnimbite squash;

        float timer;
        bool cloged;

        public override bool CanInteract
        {
            get
            {
                if (cloged) return false;
                if (usesLeft <= 0 && limitDeployment) return false;

                return true;
            }
        }

        protected override string InoperableAppend => $"Out of {Plural}";

        string Plural => string.IsNullOrEmpty(itemPlural) ? $"{itemName}s" : itemPlural;

        private void Update()
        {
            if (!limitDeployment) return;

            if (usesLeft < capacity)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0.0f;
            }

            usesLeft += rechargeCount * (int)(timer / rechargeTime);
            timer %= rechargeTime;

            if (usesLeft > capacity) usesLeft = capacity;
        }

        private void FixedUpdate()
        {
            cloged = CheckIfCloged();
        }

        private bool CheckIfCloged()
        {
            var center = transform.position + transform.rotation * clogBounds.center;
            var extents = clogBounds.extents;
            var rotation = transform.rotation;

            var results = Physics.OverlapBox(center, extents, rotation);
            foreach (var result in results)
            {
                if (!result.transform.IsChildOf(transform))
                {
                    return true;
                }
            }

            return false;
        }

        protected override void FinishInteract(Biped biped)
        {
            if (usesLeft > 0 || !limitDeployment)
            {
                var instance = Instantiate(prefab, spawnpoint.position, spawnpoint.rotation);
                instance.name = prefab.name;
                usesLeft--;
                cloged = true;
                timer = 0.0f;

                squash.Play(this, transform);
            }
        }

        public override string BuildInteractString(string passthrough = "")
        {
            if (cloged)
            {
                return TMPUtil.Color("Dispenser Jammed", Color.gray);
            }
            if (limitDeployment && usesLeft > 0)
            {
                return base.BuildInteractString($"Dispense {itemName} [{usesLeft}/{capacity} Left]");
            }
            else
            {
                return base.BuildInteractString($"Dispense {itemName}");
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(clogBounds.center, clogBounds.size);
            Gizmos.color *= new Color(1.0f, 1.0f, 1.0f, 0.1f);
            Gizmos.DrawCube(clogBounds.center, clogBounds.size);
        }
    }
}
