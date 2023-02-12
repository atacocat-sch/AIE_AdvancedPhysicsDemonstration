using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace BoschingMachine.UI.Toasts
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class Toast : MonoBehaviour
    {
        [SerializeField] TMP_Text template;
        [SerializeField] ToastLocation location;
        [SerializeField] int maxToasts;

        List<ToastInstance> toasts;

        private static event System.Action<string, ToastLocation> ToastRaisedEvent;

        private void Awake()
        {
            toasts = new List<ToastInstance>();
            template.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            ToastRaisedEvent += OnToastRaised;
        }

        private void OnDisable()
        {
            ToastRaisedEvent -= OnToastRaised;
        }

        private void OnDestroy()
        {
            ToastRaisedEvent -= OnToastRaised;
        }

        private void OnToastRaised(string message, ToastLocation location)
        {
            if (this.location != location) return;
            toasts.Add(new ToastInstance(this, message, template, ToastFinishedCallback));

            if (maxToasts <= 0) return;

            if (toasts.Count > maxToasts)
            {
                toasts[0].Destroy();
                toasts.RemoveAt(0);
            }
        }

        private void ToastFinishedCallback(ToastInstance toast)
        {
            toasts.Remove(toast.Destroy());
        }

        public static void RaiseToast (string message, ToastLocation location)
        {
            ToastRaisedEvent?.Invoke(message, location);
        }

        public enum ToastLocation
        {
            List,
            Title
        }
    }
}
