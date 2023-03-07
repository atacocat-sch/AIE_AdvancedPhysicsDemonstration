using UnityEngine;
using UnityEngine.InputSystem;
using BoschingMachine.Bipedal;
using BoschingMachine.Player.Modules;
using BoschingMachine.Vitality;
using BoschingMachine.Editor;

namespace BoschingMachine.Player
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class PlayerBiped : Biped
    {
        [Space]
        [SerializeField] InputActionAsset inputAsset;

        [Space]
        [SerializeField][Percent] float crouchSpeedPenalty;

        [Space]
        [SerializeField] PlayerPickerUpper pickerUpper;
        [SerializeField] Transform holdTarget;

        [Space]
        [SerializeField] float lookDeltaSensitivity;
        [SerializeField] float lookAdditiveSensitivity;
        [SerializeField] PlayerCameraAnimator camAnimator;

        [Space]
        [SerializeField] Interactor interactor;

        [Space]
        [SerializeField] Rigidbody deadCamera;
        [SerializeField] float deadCamForce;
        [SerializeField] float deadCamTorque;

        [Space]
        [SerializeField] Signal winSignal;

        public Health health;

        Vector2 lookRotation;

        InputActionMap playerMap;
        InputActionMap persistantMap;

        InputAction move;
        InputAction jump;
        InputAction crouch;
        InputAction lookDelta;
        InputAction lookAdditive;

        InputAction throwObject;
        InputAction interact;

        GameObject normalCollision;
        GameObject crouchCollision;

        bool crouched;

        public override Vector3 MoveDirection
        {
            get
            {
                if (Frozen) return Vector2.zero;

                Vector2 input = move.ReadValue<Vector2>();
                return transform.TransformDirection(input.x, 0.0f, input.y) * SpeedPenalty;
            }
        }

        public override bool Jump => Switch(jump) && !Frozen;
        public override Vector2 LookRotation => lookRotation;
        public float FOV { get; set; }
        public float ViewmodelFOV { get; set; }

        public float SpeedPenalty
        {
            get
            {
                float s = 1.0f;
                if (crouched) s *= crouchSpeedPenalty;
                return s;
            }
        }


        protected override void Awake()
        {
            base.Awake();

            playerMap = inputAsset.FindActionMap("Player");
            playerMap.Enable();

            move = playerMap.FindAction("move");
            jump = playerMap.FindAction("jump");
            crouch = playerMap.FindAction("crouch");

            throwObject = playerMap.FindAction("throw");
            interact = playerMap.FindAction("pickupAndDrop");
            
            persistantMap = inputAsset.FindActionMap("Player Persistant");
            persistantMap.Enable();

            lookDelta = persistantMap.FindAction("lookDelta");
            lookAdditive = persistantMap.FindAction("lookAdditive");

            var collisionGroups = transform.Find("Collision Groups");
            normalCollision = collisionGroups.GetChild(0).gameObject;
            crouchCollision = collisionGroups.GetChild(1).gameObject;
        }

        public bool Switch(InputAction action) => action.ReadValue<float>() > 0.5f;

        protected override void OnEnable()
        {
            base.OnEnable();

            Cursor.lockState = CursorLockMode.Locked;

            interact.performed += Interact;
            throwObject.performed += Throw;

            if (TryGetComponent(out health))
            {
                health.DamageEvent += OnDamage;
                health.DeathEvent += OnDie;
            }

            crouch.performed += Crouch;

            winSignal.RaiseEvent += OnWin;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Cursor.lockState = CursorLockMode.None;

            interact.performed -= Interact;
            throwObject.performed -= Throw;

            crouch.performed -= Crouch;

            winSignal.RaiseEvent -= OnWin;
        }

        private void OnDestroy()
        {
            winSignal.RaiseEvent -= OnWin;
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            interactor.Update(this, Switch(interact));
            
            lookRotation += GetLookDelta();
            lookRotation.y = Mathf.Clamp(lookRotation.y, -90.0f, 90.0f);
            base.Update();

            pickerUpper.Update(holdTarget);

#if !UNITY_EDITOR
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
                else Cursor.lockState = CursorLockMode.Locked;
            }
#endif
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            CrouchAction();

            interactor.FixedUpdate();

            pickerUpper.FixedProcess(this, holdTarget);
        }

        private void CrouchAction()
        {
            if (Jump) crouched = false;

            normalCollision.SetActive(!crouched);
            crouchCollision.SetActive(crouched);
            camAnimator.Crouched = crouched;
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            camAnimator.Update(this);
        }

        private Vector2 GetLookDelta()
        {
            Vector2 input = Vector2.zero;

            if (Cursor.lockState != CursorLockMode.Locked) return input;

            if (lookDelta != null) input += lookDelta.ReadValue<Vector2>() * lookDeltaSensitivity;
            if (lookAdditive != null) input += lookAdditive.ReadValue<Vector2>() * lookAdditiveSensitivity * Time.deltaTime;

            return input;
        }

        private void Crouch(InputAction.CallbackContext c) => crouched = !crouched;

        public void Interact (InputAction.CallbackContext _)
        {
            if (interactor.TryGetLookingAt(this, out var _)) return;
            if (pickerUpper.TryGrabOrDrop(this)) return;
        }

        public void Throw(InputAction.CallbackContext _)
        {
            pickerUpper.Throw(this);
        }

        public override void Freeze()
        {
            base.Freeze();
            playerMap.Disable();
        }

        public override void Unfreeze()
        {
            base.Unfreeze();
            playerMap.Enable();
        }

        private void OnWin(object sender, System.EventArgs e)
        {
            Cursor.lockState = CursorLockMode.None;
            playerMap.Disable();
            persistantMap.Disable();

            health.Invulnerable = true;
        }

        private void OnDamage(Health.DamageArgs obj)
        {

        }

        private void OnDie(Health.DamageArgs obj)
        {
            var dcam = Instantiate(deadCamera, Head.position, Head.rotation);
            dcam.velocity = Rigidbody.GetPointVelocity(Head.position);

            dcam.velocity += Random.insideUnitSphere * deadCamForce;
            dcam.angularVelocity += Random.insideUnitSphere * deadCamTorque;
        }
    }
}
