using UnityEngine;
using TMPro;

namespace BoschingMachine.Player.Modules
{
    [System.Serializable]
    public class PlayerPickerUpperUI
    {
        [SerializeField] TMP_Text text;
        [SerializeField] CanvasGroup group;
        [SerializeField] SeccondOrderDynamicsF spring;

        public void Update (PlayerPickerUpper pickerUpper)
        {
            float scaleTarget = 0.0f;

            if (!pickerUpper.HeldObject && pickerUpper.LookingAt)
            {
                text.text = $"Grab {pickerUpper.LookingAt.name}";
                scaleTarget = 1.0f;
            }

            spring.Loop(scaleTarget, null, Time.deltaTime);
            group.transform.localScale = Vector3.one * Mathf.Max(spring.Position, 0.0f);
        }
    }
}
