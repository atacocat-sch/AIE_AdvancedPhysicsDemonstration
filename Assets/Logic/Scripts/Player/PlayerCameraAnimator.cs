using BoschingMachine.Player;
using BoschingMachine.Utility;
using Cinemachine;
using UnityEngine;

namespace BoschingMachine
{
    [System.Serializable]
    public class PlayerCameraAnimator
    {
        [SerializeField] Transform animationContainer;

        [Space]
        [SerializeField] float defaultFov;

        [Space]
        [SerializeField] float moveTilt;

        [Space]
        [SerializeField] float smoothTime;

        [Space]
        [SerializeField] float crouchTime;
        [SerializeField] float crouchCamOffset;
        [SerializeField] float crouchZoom;

        PlayerBiped biped;

        Vector3 offset;
        Vector2 target;
        Vector2 position;
        Vector2 velocity;

        float rotationTarget;
        float rotation;
        float rotationalVelocity;

        Vector3 relativeVelocity;

        float crouchPercent;

        public bool Crouched { get; set; }
        public float Zoom { get; set; }

        public void Update(PlayerBiped biped)
        {
            Setup(biped);

            CameraMoveTilt(biped);
            CrouchCam();

            Apply(biped);
        }

        private void CrouchCam()
        {
            crouchPercent = Mathf.MoveTowards(crouchPercent, Crouched ? 1.0f : 0.0f, Time.deltaTime / crouchTime);
            var smoothed = Curves.Smootherdamp(crouchPercent);

            offset += Vector3.up * crouchCamOffset * smoothed;
            Zoom *= Mathf.Lerp(1.0f, crouchZoom, smoothed);
        }

        private void CameraMoveTilt(PlayerBiped biped)
        {
            float dot = Vector3.Dot(-biped.Head.transform.right, relativeVelocity);
            rotationTarget += dot * moveTilt;
        }

        private void Setup(PlayerBiped biped)
        {
            this.biped = biped;
            var movement = biped.Movement;
            target = Vector2.zero;
            rotationTarget = 0.0f;
            offset = Vector3.zero;
            relativeVelocity = movement.RelativeVelocity(biped.Rigidbody);
        }

        private void Apply(PlayerBiped biped)
        {
            position = Vector2.SmoothDamp(position, target, ref velocity, smoothTime);
            animationContainer.localPosition = position;
            animationContainer.position += biped.transform.rotation * offset;

            rotation = Mathf.SmoothDamp(rotation, rotationTarget, ref rotationalVelocity, smoothTime);
            animationContainer.localRotation = Quaternion.Euler(Vector3.forward * rotation);

            var vcam = biped.GetComponentInChildren<CinemachineVirtualCamera>();
            if (vcam)
            {
                var fovRad = defaultFov * Mathf.Deg2Rad;
                vcam.m_Lens.FieldOfView = 2.0f * Mathf.Atan(Mathf.Tan(0.5f * fovRad) / Zoom) * Mathf.Rad2Deg;
            }
            Zoom = 1.0f;
        }
    }
}
