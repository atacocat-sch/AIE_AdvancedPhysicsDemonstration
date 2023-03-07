using BoschingMachine.SignalGroups;
using UnityEngine;

namespace BoschingMachine
{
    public class FieldGenerator : MonoBehaviour
    {
        [SerializeField] float smoothTime;
        
        Transform fieldTransform;
        SignalGroup signalGroup;
        new Collider collider;

        float openPercent;
        float velocity;

        public bool State => signalGroup.StateB;

        private void Awake()
        {
            signalGroup = SignalGroup.GetOrCreate(gameObject);
            collider = GetComponentInChildren<Collider>();

            fieldTransform = transform.DeepFind("field");
        }

        private void OnEnable()
        {
            openPercent = State ? 1 : 0;
        }

        private void Update()
        {
            openPercent = Mathf.SmoothDamp(openPercent, State ? 1 : 0, ref velocity, smoothTime);
            collider.enabled = State;

            fieldTransform.localScale = new Vector3(1.0f, openPercent, 1.0f);
        }
    }
}
