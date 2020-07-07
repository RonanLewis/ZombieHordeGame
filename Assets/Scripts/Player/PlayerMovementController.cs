using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovementController : MonoBehaviour
{

    public float speed = 10.0f;
    public float sprintModifier = 1.4f;
    public float maxVelocityChange = 10.0f;
    public bool canJump = true;
    public static bool isLocked = false;
    public float jumpHeight = 2.0f;
    public float airMoveModifier = 0.8f;
    public RaycastHit[] moveHits;
    private Rigidbody rb;
    private float distToGround;
    private CapsuleCollider capCol;
    private bool sprinting = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
        capCol = GetComponent<CapsuleCollider>();

    }
    void FixedUpdate()
    {
            CalculateMovement();
    }
    void Update()
    {
        if (GameHandler.isPaused)
        {
            return;
        }
        if (!isLocked)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                sprinting = true;
            }
            else
            {
                sprinting = false;
            }
            // Jump
            if (IsGrounded() && canJump && Input.GetKeyDown(KeyCode.Space))
            {
                Vector3 velocity = rb.velocity;
                Jump(velocity);
            }
        }
        else
        {
            rb.freezeRotation = true;
        }
    }
    public bool IsGrounded()
    {
        RaycastHit hit;
        int layerMask = ~(1 << 8);
        return Physics.SphereCast(transform.position, transform.GetComponent<CapsuleCollider>().radius, -Vector3.up, out hit, (distToGround / 2) + 0.01f, layerMask);
    }
    private void CalculateMovement()
    {
        // Calculate how fast we should be moving
        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (isLocked)
        {
            targetVelocity = Vector3.zero;
        }
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;


        //Various modifiers to move speed.
        float moveMultiplier = 1f;
        if (!IsGrounded())
        {
            moveMultiplier *= (airMoveModifier);
        }
        else
        {
            if (sprinting)
            {
                moveMultiplier *= sprintModifier;
            }
            targetVelocity *= moveMultiplier;
        }

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange * (moveMultiplier), maxVelocityChange * (moveMultiplier));
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange * (moveMultiplier), maxVelocityChange * (moveMultiplier));
        velocityChange.y = 0;
        if (!IsGrounded())
        {
            velocityChange *= moveMultiplier;
        }
        moveHits = DetectMoveHits(velocityChange);
        if (moveHits.Length == 0)
        {
            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
        else
        {
            rb.AddForce(Vector3.ProjectOnPlane(velocityChange, moveHits[0].normal), ForceMode.VelocityChange);
        }


    }
    private void Jump(Vector3 velocity)
    {
        rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
    }
    private float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity we deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * Math.Abs(Physics.gravity.y));
    }
    private RaycastHit[] DetectMoveHits(Vector3 dir)
    {
        //if (dir == Vector3.zero) return true;
        float distanceToPoints = capCol.height / 2 - capCol.radius;

        Vector3 point1 = transform.position + capCol.center + Vector3.up * distanceToPoints;
        Vector3 point2 = transform.position;

        float radius = capCol.radius * 0.95f;
        float castDistance = 0.1f;
        int layerMask = ~(1 << 8);
        RaycastHit[] hits = Physics.CapsuleCastAll(point1, point2, radius, dir, castDistance, layerMask);

        return hits;
    }
    public static void LockMovement()
    {
        isLocked = true;
    }

    public static void UnlockMovement()
    {
        isLocked = false;
    }

}
