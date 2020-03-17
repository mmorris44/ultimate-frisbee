using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DiscState { GROUND, HELD, FLIGHT }
public enum ThrowDistance { POP, SHORT, MEDIUM, FAR, HUCK }
public enum ThrowCurve { RIGHT, STRAIGHT, LEFT }
public enum ThrowType { NORMAL, HAMMER }

public class DiscController : NetworkBehaviour
{
    public float rotationTorque = 1000f;

    public float[] curveValues; // Right force for each curve type
    public float[] durationValues; // Time to curve disc for each distance
    public float[] heightValues; // Up force for each distance
    public float[] distanceValues; // Forward force for each distance

    private Rigidbody discBody;
    private DiscState discState = DiscState.GROUND;

    private Transform heldDiscTransform;
    private float curveEndTime;
    private int curveIndex;
    private Vector3 curveDirection;

    // Start is called before the first frame update
    void Start()
    {
        discBody = GetComponent<Rigidbody>();
        discBody.maxAngularVelocity = 100;
    }

    void LateUpdate()
    {
        // If held
        if (discState == DiscState.HELD)
        {
            // Set disc position
            transform.position = heldDiscTransform.position;
            transform.rotation = heldDiscTransform.rotation;
        }
    }

    public DiscState getDiscState()
    {
        return discState;
    }

    public void pickup(Transform heldDiscTransform)
    {
        discState = DiscState.HELD;
        this.heldDiscTransform = heldDiscTransform;
        useDiscBody(false); // No physics
    }

    public void makeThrow(ThrowDistance throwDistance, ThrowCurve throwCurve, ThrowType throwType)
    {
        // Get indices for arrays from type of throw
        curveIndex = (int) throwCurve;
        int durationIndex = (int) throwDistance, heightIndex = (int) throwDistance, distanceIndex = (int) throwDistance;

        // Add forces
        useDiscBody(true);
        discBody.AddForce(transform.forward * distanceValues[distanceIndex]);
        discBody.AddForce(Vector3.up * heightValues[heightIndex]);

        // Set disk angle
        //transform.Rotate(0, 0, curveValues[curveIndex] * -10);

        // Spin the disc and make it fly
        discState = DiscState.FLIGHT;
        discBody.AddTorque(transform.up * rotationTorque);

        // Start routine for moving sideways
        curveEndTime = Time.time + durationValues[durationIndex];
        curveDirection = transform.right;
        StartCoroutine("CurveRoutine");
    }

    void useDiscBody(bool isActive)
    {
        if (!isActive)
        {
            discBody.detectCollisions = false;
            discBody.useGravity = false;
            discBody.velocity = Vector3.zero;
        } else
        {
            discBody.detectCollisions = true;
            discBody.useGravity = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Ground") discState = DiscState.GROUND;
    }

    IEnumerator CurveRoutine ()
    {
        while (Time.time < curveEndTime)
        {
            discBody.AddForce(curveDirection * curveValues[curveIndex]);
            yield return null;
        }
    }
}
