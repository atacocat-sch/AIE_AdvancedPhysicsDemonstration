using System.Collections;
using UnityEngine;

namespace BoschingMachine.Animbites
{
    [System.Serializable]
    public class SquashAnimbite
    {
        [SerializeField] AnimationCurve curve;
        [SerializeField][Range(0.0f, 1.0f)] float strength;
        [SerializeField] float duration;
        [SerializeField] Vector3 axis;

        public void Play (MonoBehaviour behaviour, Transform target)
        {
            behaviour.StartCoroutine(Play(target));
        }

        public IEnumerator Play (Transform target)
        {
            Vector3 originalScale = target.localScale;
            Vector3 axis = this.axis.normalized;

            float percent = 0.0f;
            while (percent < 1.0f)
            {
                float vScale = 1.0f + curve.Evaluate(percent) * strength;
                float hScale = Mathf.Sqrt(1 / vScale);

                Vector3 scale = Vector3.one * hScale;
                scale += axis * (vScale - Vector3.Dot(axis, scale));

                target.localScale = Vector3.Scale(originalScale, scale);

                percent += Time.deltaTime / duration;
                yield return null;
            }

            target.localScale = originalScale;
        }
    }
}
