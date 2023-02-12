using BoschingMachine.Interactables;
using UnityEngine;

namespace BoschingMachine.Items
{
    [System.Serializable]
    public class Item
    {
        [SerializeField] string name;
        [SerializeField] Sprite icon;
        [SerializeField] Vector2Int size;
        [SerializeField] Loot worldItem;
        
        public string Name => name;
        public Sprite Icon => icon;
        public Vector2Int Size => Rotated ? new Vector2Int(size.y, size.x) : size;
        public Vector2Int SizeActual => size;
        public Inventory Inventory { get; private set; }
        public Vector2Int Position { get; private set; }
        public bool Rotated { get; set; }

        public void RegisterNewInventory(Inventory inventory, Vector2Int position)
        {
            Inventory = inventory;
            Position = position;
        }

        internal void RemoveFromInventory()
        {
            Inventory = null;
            Position = new Vector2Int(-1, -1);
        }

        public void DropInWorld (Vector3 position)
        {
            if (Inventory != null)
            {
                Inventory.RemoveItem(this);
            }

            var loot = UnityEngine.Object.Instantiate(worldItem, position, Quaternion.identity);
            loot.SetItem(this);
        }
    }
}
