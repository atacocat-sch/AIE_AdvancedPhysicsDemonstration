using BoschingMachine.Bipedal;
using BoschingMachine.Items;
using BoschingMachine.UI.Toasts;
using UnityEngine;

namespace BoschingMachine.Interactables
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class Loot : Interactable
    {
        [SerializeField] int value;
        [SerializeField] Item item;

        public override string InteractName => $"Collect {item.Name}";

        protected override void FinishInteract(Biped biped)
        {
            if (biped is IHasInventory)
            {
                Inventory inventory = ((IHasInventory)biped).Inventory;
                if (inventory.TryAddItem(item))
                {
                    Destroy(gameObject);
                }
                else
                {
                    Toast.RaiseToast($"Cannot add {item.Name} to Inventory", Toast.ToastLocation.List);
                }
            }
        }
        
        public void SetItem (Item item)
        {
            this.item = item;
        }
    }
}
