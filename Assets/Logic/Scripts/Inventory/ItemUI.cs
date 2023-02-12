using UnityEngine;
using UnityEngine.UI;

namespace BoschingMachine.Items.UI
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class ItemUI : MonoBehaviour
    {
        [SerializeField] Image icon;

        Item item;
        float cellSize;
        bool dirty;

        new public RectTransform transform => (RectTransform)base.transform;

        public ItemUI SetItem(Item item, float cellSize)
        {
            this.item = item;
            this.cellSize = cellSize;
            UpdateVisuals();
            return this;
        }

        public void UpdateVisuals ()
        {
            dirty = true;
        }

        private void LateUpdate()
        {
            if (dirty)
            {
                icon.sprite = item.Icon;
                transform.sizeDelta = (Vector2)item.SizeActual * cellSize;
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, item.Rotated ? 90.0f : 0.0f);

                Vector2 pos = ((Vector2)item.Position + (Vector2)item.Size * 0.5f) * cellSize;
                pos.y = -pos.y;
                transform.localPosition = pos;

                dirty = false;
            }
        }
    }
}
