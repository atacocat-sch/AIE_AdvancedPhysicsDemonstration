using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine.UI.Toasts
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class ToastOnEnable : MonoBehaviour
    {
        [SerializeField][TextArea] string message;
        [SerializeField] Toast.ToastLocation location;

        private void OnEnable()
        {
            StartCoroutine(DeferToast());
        }

        public IEnumerator DeferToast ()
        {
            yield return null;
            Toast.RaiseToast(message, location);
        }
    }
}
