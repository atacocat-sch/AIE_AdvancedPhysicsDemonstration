using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BoschingMachine.SignalGroups
{
    public class SignalGroupUIButton : Button
    {
        public bool toggle;
        public int callData;

        SignalGroup signalGroup;

        protected override void Awake()
        {
            base.Awake();

            signalGroup = SignalGroup.GetOrCreate(gameObject);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            signalGroup.Call(GetCallData());
        }

        public int GetCallData()
        {
            if (toggle) return signalGroup.StateB ? 0 : 1;
            else return callData;
        }
    }
}
