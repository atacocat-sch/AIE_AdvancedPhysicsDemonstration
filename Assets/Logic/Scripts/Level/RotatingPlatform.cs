using UnityEngine;

namespace BoschingMachine
{
    [RequireComponent(typeof(Rigidbody))]
    public class RotatingPlatform : MonoBehaviour
    {
        [SerializeField] Vector3 axis;
        [SerializeField] float revolutionTime;

        new Rigidbody rigidbody;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            float degPerSeccond = 360.0f / revolutionTime;
            rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(axis * degPerSeccond * Time.deltaTime));
        }
    }
}
