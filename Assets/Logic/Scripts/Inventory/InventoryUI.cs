using System.Collections.Generic;
using BoschingMachine.Player;
using UnityEngine;

namespace BoschingMachine.Items.UI
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class InventoryUI : MonoBehaviour
    {
        [SerializeField] float fadeSpeed, cellSize;
        
        [Space]
        [SerializeField] RectTransform itemContainer;
        [SerializeField] ItemUI itemUIPrefab;
        [SerializeField] RectTransform outline;

        
        CanvasGroup inventoryGroup;
        PlayerBiped player;
        Inventory inventory;
        Dictionary<Item, ItemUI> uiInstances = new Dictionary<Item, ItemUI>();

        bool open;
        float fade;

        new public RectTransform transform => (RectTransform)base.transform;

        private void Awake()
        {
            inventoryGroup = GetComponent<CanvasGroup>();
            player = GetComponentInParent<PlayerBiped>();
            inventory = player.Inventory;

            inventory.ChangeEvent += Redraw;
        }

        private void Start()
        {
            fade = 0.0f;
            SetOpenState(false);

            outline.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, inventory.Size.x * cellSize);
            outline.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventory.Size.y * cellSize);
        }

        public void Redraw()
        {
            foreach (var item in inventory)
            {
                if (!uiInstances.ContainsKey(item))
                {
                    uiInstances.Add(item, Instantiate(itemUIPrefab, itemContainer).SetItem(item, cellSize));
                }
            }

            HashSet<Item> oldItems = new HashSet<Item>();
            foreach (var item in uiInstances)
            {
                if (item.Key.Inventory != inventory)
                    oldItems.Add(item.Key);
            }

            foreach (var oldItem in oldItems)
            {
                Destroy(uiInstances[oldItem]);
                uiInstances.Remove(oldItem);
            }

            foreach (var item in uiInstances)
            {
                item.Value.UpdateVisuals();
            }
        }

        private void Update()
        {
            fade += ((open ? 1.0f : 0.0f) - fade) * fadeSpeed * Time.deltaTime;
            fade = Mathf.Clamp01(fade);
            inventoryGroup.alpha = fade;
        }

        public void ToggleOpenState ()
        {
            SetOpenState(!open);
        }

        public void SetOpenState (bool open)
        {
            this.open = open;
            inventoryGroup.interactable = open;
            
            if (open)
            {
                player.TakeCursor(this);
            }
            else
            {
                player.ReleaseCursor(this);
            }
        }
    }
}
