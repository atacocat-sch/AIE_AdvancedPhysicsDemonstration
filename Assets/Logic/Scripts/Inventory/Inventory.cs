using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine.Items
{
    [System.Serializable]
    public class Inventory : IEnumerable<Item>
    {
        [SerializeField] Vector2Int size;

        Item[,] slots;

        public event System.Action ChangeEvent;

        public Vector2Int Size => size;

        public void Setup ()
        {
            slots = new Item[size.x, size.y];
        }
        
        public bool TryAddItem (Item item)
        {
            bool Pass()
            {
                for (int x = 0; x < size.x - item.Size.x + 1; x++)
                {
                    for (int y = 0; y < size.y - item.Size.y + 1; y++)
                    {
                        if (TryInsertItem(item, new Vector2Int(x, y))) return true;
                    }
                }
                return false;
            }

            bool rotationState = item.Rotated;

            item.Rotated = false;
            if (Pass()) return true;
            item.Rotated = true;
            if (Pass()) return true;

            item.Rotated = rotationState;
            return false;
        }

        public bool TryInsertItem (Item item, Vector2Int position)
        {
            if (position.x < 0 || position.y < 0) return false;
            if (position.x + item.Size.x > size.x || position.y + item.Size.y > size.y) return false;

            for (int x = 0; x < item.Size.x; x++)
            {
                for (int y = 0; y < item.Size.y; y++)
                {
                    if (slots[position.x + x, position.y + y] != null) return false;
                }
            }

            if (item.Inventory != null)
            {
                item.Inventory.RemoveItem(item);
            }

            for (int x = 0; x < item.Size.x; x++)
            {
                for (int y = 0; y < item.Size.y; y++)
                {
                    slots[position.x + x, position.y + y] = item;
                }
            }

            item.RegisterNewInventory(this, position);
            ChangeEvent?.Invoke();
            return true;
        }

        public void RemoveItem(Item item)
        {
            if (item.Inventory != this) return;

            for (int x = 0; x < item.Size.x; x++)
            {
                for (int y = 0; y < item.Size.y; y++)
                {
                    Vector2Int pos = item.Position + new Vector2Int(x, y);
                    slots[pos.x, pos.y] = null;
                }
            }

            item.RemoveFromInventory();
            ChangeEvent?.Invoke();
        }

        public IEnumerator<Item> GetEnumerator()
        {
            HashSet<Item> items = new HashSet<Item>();
            foreach (var slot in slots)
            {
                if (slot == null) continue;
                if (!items.Contains(slot)) items.Add(slot);
            }

            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void RaiseChange()
        {
            ChangeEvent?.Invoke();
        }
    }

    public interface IHasInventory
    {
        public Inventory Inventory { get; }
    }
}
