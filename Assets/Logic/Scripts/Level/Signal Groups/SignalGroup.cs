using UnityEngine;

namespace BoschingMachine.SignalGroups
{
    public class SignalGroup : MonoBehaviour
    {
        public int State { get; set; }
        public bool StateB => State > 0;
        
        public event System.Action<int> OnCall;

        public void Call() => Call(0);
        public void Call(int state)
        {
            State = state;
            OnCall?.Invoke(state);
        }

        public static SignalGroup GetOrCreate(GameObject gameObject)
        {
            if (!gameObject) return null;
            if (!Application.isPlaying) return null;

            var group = gameObject.GetComponentInParent<SignalGroup>();
            if (group) return group;

            group = new GameObject("Procedual Signal Group").AddComponent<SignalGroup>();
            group.transform.parent = gameObject.transform.parent;
            group.transform.localPosition = Vector3.zero;
            group.transform.localRotation = Quaternion.identity;
            gameObject.transform.parent = group.transform;

            return group;
        }
    }
}
