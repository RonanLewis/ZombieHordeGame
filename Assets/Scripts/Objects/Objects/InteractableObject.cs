using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractableObject : MonoBehaviour
{

    public Transform grabbedBy = null;
    public float rotateSpeed = 50f;
    public float minHoldDistance = 2f;
    private Transform grabbedByCam = null;
    private Rigidbody rb;

    public float maxSpeed = 10f;
    public float gain = 5f;
    public float acceleration = 10f;
    public float deceleration = 3f;

    public float breakPoint = 1.5f;
    private bool isColliding = false;
    private Collision collisionWith;
    private float distanceModifier = 0;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Use()
    {

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (grabbedByCam)
        {
            Vector3 moveToLocation = grabbedByCam.position + grabbedByCam.forward * (minHoldDistance + distanceModifier);
            Vector3 dist = moveToLocation - transform.position;
                        // calc a target vel proportional to distance (clamped to maxVel)
            Vector3 tgtVel = Vector3.ClampMagnitude(deceleration * dist, maxSpeed);
            // calculate the velocity error
            Vector3 error = tgtVel - rb.velocity;
            // calc a force proportional to the error (clamped to maxForce)
            Vector3 force = Vector3.ClampMagnitude(gain * error, acceleration);
            rb.AddForce(force);
        }
    }

    public void Rotate(Vector3 rotateBy)
    {
        if (grabbedByCam)
        {
            float horizontalRot;
            float verticalRot;

            Vector3 vertRotAxis = transform.InverseTransformDirection(grabbedByCam.TransformDirection(Vector3.right)).normalized;
            horizontalRot = rotateBy.y * -rotateSpeed;
            verticalRot = rotateBy.x * rotateSpeed;
            transform.Rotate(vertRotAxis, verticalRot);
            transform.Rotate(new Vector3(0, horizontalRot), Space.World);
        }
    }
    public void Drop()
    {
        if (grabbedBy)
        {
            Debug.Log("Dropping");
            grabbedBy = null;
            grabbedByCam = null;
            rb.useGravity = true;
            distanceModifier = 0;
            rb.freezeRotation = false;
        }
    }
    private void Update()
    {
        if (grabbedByCam)
        {
            Debug.Log(1);
            if (isColliding
                && Vector3.Distance(transform.position, grabbedByCam.position + grabbedByCam.forward * minHoldDistance) > minHoldDistance / breakPoint
                || Vector3.Distance(transform.position, grabbedByCam.position) < minHoldDistance / breakPoint)
            {
                Drop();
            }
            CheckGrasp();
        }
    }
    public void Throw()
    {
        rb.AddForce(grabbedByCam.forward * 10f, ForceMode.Impulse);
        Drop();
    }
    public void Hold(Transform by)
    {
        OnPickUp(by);
    }

    private void OnPickUp(Transform by)
    {

        if (!grabbedBy)
        {
            grabbedBy = by;
            // Get the camera
            grabbedByCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().transform;

            minHoldDistance = grabbedBy.GetComponent<PlayerInteractController>().holdDistance;
            rb.velocity = Vector3.zero;
            rb.freezeRotation = true;
            rb.useGravity = false;
            Debug.Log(transform.name + " picked up by: " + by.name);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (grabbedBy)
        {
            if (collision.transform.GetInstanceID() != grabbedBy.GetInstanceID())
            {
                isColliding = true;
                collisionWith = collision;
            }
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (grabbedBy)
        {
            if (collision.transform.GetInstanceID() != grabbedBy.GetInstanceID())
            {
                isColliding = true;
                collisionWith = collision;
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (grabbedBy)
        {
            if (collision.transform.GetInstanceID() != grabbedBy.GetInstanceID())
            {
                isColliding = false;
                collisionWith = null;
            }
        }
    }

    private void CheckGrasp()
    {
        if (grabbedBy)
        {
            //Drops the held item if something gets inbetween the player and the item.
            Debug.DrawRay(transform.position, transform.position - grabbedBy.transform.position, Color.green);
            RaycastHit hit;
            if (grabbedBy && Physics.Raycast(transform.position, grabbedBy.transform.position - transform.position, out hit, Mathf.Infinity))
            {
                if (hit.transform.GetInstanceID() != grabbedBy.GetInstanceID())
                {
                    Drop();
                }
            }
        }
    }

}
