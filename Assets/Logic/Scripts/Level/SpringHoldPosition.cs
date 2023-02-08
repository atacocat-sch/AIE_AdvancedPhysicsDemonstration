using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[SelectionBase]
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public sealed class SpringHoldPosition : MonoBehaviour
{
    [SerializeField] float force;
    [SerializeField] float damping;

    Rigidbody rigidbody;
    Vector3 position;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        position = rigidbody.position;
    }

    private void FixedUpdate()
    {
        Vector3 force = (position - rigidbody.position) * this.force;
        force += -rigidbody.velocity * damping;

        rigidbody.velocity += force * Time.deltaTime;
    }
}
