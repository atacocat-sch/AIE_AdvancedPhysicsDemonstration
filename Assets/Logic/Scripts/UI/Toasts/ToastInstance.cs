using System.Collections;
using BoschingMachine.Utility;
using TMPro;
using UnityEngine;

namespace BoschingMachine.UI.Toasts
{
    public sealed class ToastInstance
    {
        const float duration = 5.0f;
        const float animateTime = 0.4f;
        
        public string message;
        public float time;

        public TMP_Text textObject;

        public ToastInstance(Toast toast, string message, TMP_Text prefab, System.Action<ToastInstance> finishCallback)
        {
            this.message = message;
            time = Time.time;

            textObject = Object.Instantiate(prefab, toast.transform);
            textObject.name = "Toast Instance";
            textObject.gameObject.SetActive(true);

            toast.StartCoroutine(Loop(finishCallback));
        }

        public IEnumerator Loop(System.Action<ToastInstance> finishCallback)
        {
            float percent = 0.0f;
            while (percent < 1.0f)
            {
                AnimateIn(percent);
                percent += Time.deltaTime / animateTime;
                yield return null;
            }

            textObject.text = message;

            yield return new WaitForSeconds(duration);

            percent = 0.0f;
            while (percent < 1.0f)
            {
                AnimateOut(percent);
                percent += Time.deltaTime / animateTime;
                yield return null;
            }

            textObject.text = string.Empty;

            finishCallback(this);
        }

        public void AnimateIn(float p)
        {
            textObject.text = AnimHelper.TypewriterReversed(message, p);
        }

        public void AnimateOut(float p)
        {
            textObject.text = AnimHelper.TypewriterReversed(message, 1.0f - p);
        }

        public ToastInstance Destroy()
        {
            Object.Destroy(textObject);
            return this;
        }
    }
}
