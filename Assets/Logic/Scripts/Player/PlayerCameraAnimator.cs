using BoschingMachine.Player;
using UnityEngine;

namespace BoschingMachine
{
    [System.Serializable]
    public class PlayerCameraAnimator
    {
        [SerializeField] Transform animationContainer;

        [Space]
        [SerializeField] float runSwayFrequency;
        [SerializeField] float runSwayAmplitude;

        [Space]
        [SerializeField] float moveTilt;

        [Space]
        [SerializeField] float smoothTime;

        Vector2 target;
        Vector2 position;
        Vector2 velocity;

        float rotationTarget;
        float rotation;
        float rotationalVelocity;

        float runDistance;

        public void Update(PlayerBiped biped)
        {
            var movement = biped.Movement;
            target = Vector3.zero;
            rotationTarget = 0.0f;
            Vector3 relativeVelocity = movement.RelativeVelocity(biped.Rigidbody);

            if (movement.IsGrounded)
            {
                float runSpeed = relativeVelocity.magnitude;
                float normalizedRunSpeed = runSpeed / movement.MaxMoveSpeed;

                rotationTarget += Mathf.Sin(runDistance * runSwayFrequency) * runSwayAmplitude * normalizedRunSpeed;

                runDistance += runSpeed * Time.deltaTime;
            }

            float dot = Vector3.Dot(-biped.Head.transform.right, relativeVelocity);
            rotationTarget += dot * moveTilt;
            Integrate(biped);
        }

        private void Integrate(PlayerBiped biped)
        {
            position = Vector2.SmoothDamp(position, target, ref velocity, smoothTime);
            animationContainer.localPosition = position;

            rotation = Mathf.SmoothDamp(rotation, rotationTarget, ref rotationalVelocity, smoothTime);
            animationContainer.localRotation = Quaternion.Euler(Vector3.forward * rotation);
        }
    }
}
