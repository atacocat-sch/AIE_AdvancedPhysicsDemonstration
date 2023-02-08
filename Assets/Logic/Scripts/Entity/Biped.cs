using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class Biped : MonoBehaviour
{
    [SerializeField] EntityMovement movement;

    [Space]
    [SerializeField] Transform cameraRotor;

    new Rigidbody rigidbody;

    public EntityMovement Movement => movement;

    protected virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {
        movement.Process(transform, cameraRotor);
    }

    protected virtual void FixedUpdate()
    {
        movement.FixedProcess(this, rigidbody);
    }
}
