using UnityEngine;
using BoschingMachine.Bipedal;
using BoschingMachine.Player.Modules;
using UnityEngine.InputSystem;
using BoschingMachine.Vitality;

namespace BoschingMachine.Player
{
    [SelectionBase]
    [DisallowMultipleComponent]
    public sealed class PlayerBiped : Biped
    {
        [Space]
        [SerializeField] InputActionAsset inputAsset;

        [Space]
        [SerializeField] PlayerPickerUpper pickerUpper;
        [SerializeField] Transform holdTarget;

        [Space]
        [SerializeField] float lookDeltaSensitivity;
        [SerializeField] float lookAdditiveSensitivity;

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
        InputAction lookDelta;
        InputAction lookAdditive;

        InputAction throwObject;
        InputAction interact;

        InputAction inventoryAction;

        public override Vector3 MoveDirection
        {
            get
            {
                if (Frozen) return Vector2.zero;

                Vector2 input = move.ReadValue<Vector2>();
                return transform.TransformDirection(input.x, 0.0f, input.y);
            }
        }

        public override bool Jump => ReadFlag(jump) && !Frozen;
        public override Vector2 LookRotation => lookRotation;
        public float FOV { get; set; }
        public float ViewmodelFOV { get; set; }

        protected override void Awake()
        {
            base.Awake();

            playerMap = inputAsset.FindActionMap("Player");
            playerMap.Enable();

            move = playerMap.FindAction("move");
            jump = playerMap.FindAction("jump");

            throwObject = playerMap.FindAction("throw");
            interact = playerMap.FindAction("pickupAndDrop");
            
            inventoryAction = playerMap.FindAction("inventory");

            persistantMap = inputAsset.FindActionMap("Player Persistant");
            persistantMap.Enable();

            lookDelta = persistantMap.FindAction("lookDelta");
            lookAdditive = persistantMap.FindAction("lookAdditive");
        }

        public bool ReadFlag(InputAction action) => action.ReadValue<float>() > 0.5f;

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

            winSignal.RaiseEvent += OnWin;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Cursor.lockState = CursorLockMode.None;

            interact.performed -= Interact;
            throwObject.performed -= Throw;

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
            interactor.Update(this, ReadFlag(interact));
            
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

            interactor.FixedUpdate();

            pickerUpper.FixedProcess(Rigidbody, holdTarget);
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
        }

        private Vector2 GetLookDelta()
        {
            Vector2 input = Vector2.zero;

            if (Cursor.lockState != CursorLockMode.Locked) return input;

            if (lookDelta != null) input += lookDelta.ReadValue<Vector2>() * lookDeltaSensitivity;
            if (lookAdditive != null) input += lookAdditive.ReadValue<Vector2>() * lookAdditiveSensitivity * Time.deltaTime;

            return input;
        }

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
