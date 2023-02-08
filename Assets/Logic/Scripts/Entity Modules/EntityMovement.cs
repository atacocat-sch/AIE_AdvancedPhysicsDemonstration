using System.Collections;
using Unity.Burst;
using UnityEngine;

[System.Serializable]
public sealed class EntityMovement
{
    [SerializeField] float moveSpeed = 10.0f;
    [SerializeField] float groundAcceleration = 80.0f;

    [Space]
    [SerializeField] float airMoveAcceleration = 8.0f;

    [Space]
    [SerializeField] float jumpHeight = 3.5f;
    [SerializeField] float upGravity = 2.0f;
    [SerializeField] float downGravity = 3.0f;
    [SerializeField] float jumpSpringPauseTime = 0.1f;

    [Space]
    [SerializeField] float springDistance = 1.2f;
    [SerializeField] float springForce = 250.0f;
    [SerializeField] float springDamper = 15.0f;
    [SerializeField] float groundCheckRadius = 0.4f;
    [SerializeField] LayerMask groundCheckMask = 0b1;

    bool previousJumpState;
    float lastJumpTime;

    public float MoveSpeed => moveSpeed;

    public float DistanceToGround { get; private set; }
    public bool IsGrounded => DistanceToGround < 0.0f;
    public GameObject Ground { get; private set; }
    public Rigidbody GroundRigidbody { get; private set; }

    public Vector3 MoveDirection { get; set; }
    public Vector2 FaceRotation { get; set; }
    public bool Jump { get; set; }
    public Vector2 PlanarSpeed(Rigidbody rigidbody) => new Vector2(rigidbody.velocity.x, rigidbody.velocity.z);
    public Vector2 LocalPlanarSpeed(Rigidbody rigidbody)
    {
        if (!IsGrounded) return PlanarSpeed(rigidbody);
        if (!GroundRigidbody) return PlanarSpeed(rigidbody);

        return PlanarSpeed(rigidbody) - new Vector2(GroundRigidbody.velocity.x, GroundRigidbody.velocity.z);
    }

    public void FixedProcess(MonoBehaviour caller, Rigidbody rigidbody)
    {
        DistanceToGround = GetDistanceToGround(rigidbody) - springDistance;

        MoveCharacter(rigidbody);

        if (Jump && !previousJumpState)
        {
            TryJump(caller, rigidbody);
        }
        previousJumpState = Jump;

        ApplySpring(rigidbody);
        ApplyGravity(rigidbody);
    }

    public void Process(Transform transform, Transform camera)
    {
        FaceRotation = new Vector2(FaceRotation.x, Mathf.Clamp(FaceRotation.y, -90.0f, 90.0f));
        transform.rotation = Quaternion.Euler(0.0f, FaceRotation.x, 0.0f);
        camera.rotation = Quaternion.Euler(-FaceRotation.y, FaceRotation.x, 0.0f);
    }

    private void ApplySpring(Rigidbody rigidbody)
    {
        if (IsGrounded && Time.time > lastJumpTime + jumpSpringPauseTime)
        {
            float force = 0.0f;
            float groundVelocity = GroundRigidbody ? GroundRigidbody.velocity.y : 0.0f;
            
            float contraction = 1.0f - ((DistanceToGround + springDistance) / springDistance);
            force += contraction * springForce;
            force += (groundVelocity - rigidbody.velocity.y) * springDamper;

            if (GroundRigidbody)
            {
                DistributeForce(rigidbody, GroundRigidbody, Vector3.up * force * Time.deltaTime);
            }
            else
            {
                rigidbody.velocity += Vector3.up * force * Time.deltaTime;
            }
        }
    }

    private void ApplyGravity(Rigidbody rigidbody)
    {
        rigidbody.useGravity = false;
        rigidbody.velocity += GetGravity(rigidbody) * Time.deltaTime;
    }

    private void MoveCharacter(Rigidbody rigidbody)
    {
        Vector3 direction = MoveDirection;

        if (IsGrounded)
        {
            Vector3 target = direction * moveSpeed;
            Vector3 current = rigidbody.velocity;

            Vector3 delta = Vector3.ClampMagnitude(target - current, moveSpeed);
            delta.y = 0.0f;

            Vector3 force = delta / moveSpeed * groundAcceleration;

            rigidbody.velocity += force * Time.deltaTime;
        }
        else
        {
            rigidbody.velocity += direction * airMoveAcceleration * Time.deltaTime;
        }
    }

    private void TryJump(MonoBehaviour caller, Rigidbody rigidbody)
    {
        caller.StartCoroutine(TryJumpRoutine(rigidbody));
    }
    private IEnumerator TryJumpRoutine(Rigidbody rigidbody)
    {
        if (IsGrounded)
        {
            lastJumpTime = Time.time;

            float gravity = Vector3.Dot(Vector3.down, GetGravity(rigidbody));
            float jumpForce = Mathf.Sqrt(2.0f * gravity * jumpHeight);
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0.0f, rigidbody.velocity.z);

            Vector3 force = Vector3.up * jumpForce;
            if (GroundRigidbody)
            {
                DistributeForce(rigidbody, GroundRigidbody, force);
                float f = GroundRigidbody.velocity.y;
                yield return null;

                float diff = GroundRigidbody.velocity.y - f;
                rigidbody.velocity += Vector3.up * diff;
                Debug.Log(diff);
            }
            else
            {
                rigidbody.velocity += force;
            }
        }
    }

    public void DistributeForce (Rigidbody a, Rigidbody b, Vector3 force)
    {
        if (a.mass > b.mass)
        {
            DistributeForce(b, a, -force);
            return;
        }

        float s1 = (b.mass - a.mass) / (b.mass + a.mass);
        float s2 = (2.0f * a.mass) / (b.mass + a.mass);

        a.velocity += force * s1;
        b.velocity -= force * s2;
    }

    private Vector3 GetGravity(Rigidbody rigidbody)
    {
        float scale = upGravity;
        if (!Jump)
        {
            scale = downGravity;
        }
        else if (rigidbody.velocity.y < 0.0f)
        {
            scale = downGravity;
        }

        return Physics.gravity * scale;
    }

    public float GetDistanceToGround(Rigidbody rigidbody)
    {
        if (Physics.SphereCast(rigidbody.position + Vector3.up * groundCheckRadius, groundCheckRadius, Vector3.down, out var hit, 1000.0f, groundCheckMask))
        {
            Ground = hit.transform.gameObject;
            GroundRigidbody = hit.rigidbody;
            return hit.distance;
        }
        else
        {
            Ground = null;
            GroundRigidbody = null;
            return float.PositiveInfinity;
        }
    }
}
