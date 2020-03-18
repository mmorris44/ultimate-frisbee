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
    private float curveEndTime;
    private int curveIndex;
    private Vector3 curveDirection;
    private Transform heldDiscTransform;

    [SyncVar]
    public DiscState discState = DiscState.GROUND; // TODO: make private later

    // Start is called before the first frame update
    void Start()
    {
        discBody = GetComponent<Rigidbody>();
        discBody.maxAngularVelocity = 100;

        heldDiscTransform = transform; // TODO: remove later
    }

    private void Update()
    {
        if (discState == DiscState.HELD)
        {
            useDiscBody(false); // No physics
        } else
        {
            useDiscBody(true); // Use physics
        }
    }

    void LateUpdate()
    {
        // Only execute with authority
        if (!hasAuthority) return;

        // If held
        if (discState == DiscState.HELD)
        {
            SetDiscTransform();
        }
    }

    void SetDiscTransform()
    {
        transform.position = heldDiscTransform.position;
        transform.rotation = heldDiscTransform.rotation;
    }

    public DiscState getDiscState()
    {
        return discState;
    }

    public void pickup(Transform heldDiscTransform)
    {
        this.heldDiscTransform = heldDiscTransform;
    }

    // Command to set the value of the state in the server
    [Command]
    public void CmdPickup ()
    {
        discState = DiscState.HELD;
    }

    // Called when client receives authority over the disc
    public override void OnStartAuthority ()
    {
        Debug.Log("Auth has been granted, sending request for held disc state update");
        CmdPickup();
    }

    [Command]
    public void CmdMakeThrow(ThrowDistance throwDistance, ThrowCurve throwCurve, ThrowType throwType)
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
            //discBody.detectCollisions = false;
            //discBody.useGravity = false;
            discBody.velocity = Vector3.zero;
            discBody.isKinematic = true;
        } else
        {
            //discBody.detectCollisions = true;
            //discBody.useGravity = true;
            discBody.isKinematic = false;
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
