using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine.Tags
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Tags/Tag")]
    public class Tag : ScriptableObject
    {

    }

    public static class Extensions
    {
        public static bool HasTag (this GameObject gameObject, Tag tag)
        {
            if (!tag) return true;

            var holder = gameObject.GetComponentInParent<TagHolder>();
            if (!holder) return false;

            return holder.HasTag(tag);
        }
    }
}
