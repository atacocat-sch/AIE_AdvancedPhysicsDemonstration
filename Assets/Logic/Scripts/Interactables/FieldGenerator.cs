using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoschingMachine
{
    public class FieldGenerator : MonoBehaviour, IStateDrivable
    {
        [SerializeField] Transform fieldTransform;
        [SerializeField] new Collider collider;
        [SerializeField] float smoothTime;
        [SerializeField] bool state;

        float openPercent;
        float velocity;

        public bool State { get => state; set => state = value; }

        private void OnEnable()
        {
            openPercent = State ? 1 : 0;
        }

        private void Update()
        {
            openPercent = Mathf.SmoothDamp(openPercent, State ? 1 : 0, ref velocity, smoothTime);
            collider.enabled = state;

            fieldTransform.localScale = new Vector3(1.0f, openPercent, 1.0f);
        }
    }
}
