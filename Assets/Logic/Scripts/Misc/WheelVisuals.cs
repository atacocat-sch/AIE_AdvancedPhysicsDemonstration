using UnityEngine;

namespace BoschingMachine
{
    public class WheelVisuals : MonoBehaviour
    {
        WheelCollider wheel;

        private void Awake()
        {
            wheel = GetComponentInParent<WheelCollider>();
        }

        private void LateUpdate()
        {
            wheel.GetWorldPose(out var position, out var rotation);
            transform.position = position;
            transform.rotation = rotation;
        }
    }
}
