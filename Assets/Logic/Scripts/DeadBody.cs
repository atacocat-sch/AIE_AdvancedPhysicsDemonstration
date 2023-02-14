using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class DeadBody : MonoBehaviour
    {
        [SerializeField] Transform bodyRoot;

        List<Rigidbody> segments;

        private void Awake()
        {
            segments = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        }

        private void FixedUpdate()
        {
            Vector3 translation = Vector3.zero;

            foreach (var segment in segments)
            {
                Vector3 vector = segment.transform.position - transform.position;
                translation += vector;
            }

            translation /= segments.Count;

            transform.position += translation;
            bodyRoot.position -= translation;
        }
    }
}
