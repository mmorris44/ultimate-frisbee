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
    public float discLiftForce = 1f;

    public float[] curveValues; // Right force for each curve type
    public float[] durationValues; // Time to curve disc for each distance
    public float[] heightValues; // Up force for each distance
    public float[] distanceValues; // Forward force for each distance

    public GameObject interactAlert;

    private Rigidbody discBody;
    private float curveEndTime;
    private int curveIndex;
    private Vector3 curveDirection;
    public Transform heldDiscTransform; // TODO: Make private

    [SyncVar]
    public DiscState discState = DiscState.GROUND; // TODO: Make private

    // Start is called before the first frame update
    void Start()
    {
        discBody = GetComponent<Rigidbody>();
        discBody.maxAngularVelocity = 100;

        heldDiscTransform = transform;
    }

    private void Update()
    {
        // Only server use physics
        if (!isServer) useDiscBody(false);
        else {
            if (discState == DiscState.HELD) useDiscBody(false);
            else useDiscBody(true);
        }
    }

    void FixedUpdate()
    {
        // Only execute on server
        if (!isServer) return;

        // If in flight, apply upward force based on current speed
        if (discState == DiscState.FLIGHT)
        {
            Vector3 velocity = discBody.velocity;
            velocity.y = 0;
            discBody.AddForce(Vector3.up * velocity.magnitude * discLiftForce);
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
        //Debug.Log("[SERVER] Request to update disc state received. Current state: " + discState);
        discState = DiscState.HELD;
        //Debug.Log("[SERVER] Disc state updated. Changed to: " + discState);
    }

    // Called when client receives authority over the disc
    public override void OnStartAuthority ()
    {
        //Debug.Log("[CLIENT] Auth has been granted, sending request for held disc state update");
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

        // Spin the disc and make it fly
        discState = DiscState.FLIGHT;
        discBody.AddTorque(transform.up * rotationTorque);

        // Start routine for moving sideways
        curveEndTime = Time.time + durationValues[durationIndex];
        curveDirection = transform.right;
        StartCoroutine("CurveRoutine");

        // Start routine for detecting collisions
        StartCoroutine("CollisionsOnRoutine");
    }

    void useDiscBody(bool isActive)
    {
        if (!isActive)
        {
            discBody.velocity = Vector3.zero;
            discBody.isKinematic = true;
            discBody.detectCollisions = false;
        } else
        {
            discBody.isKinematic = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Only check for collisions as the server
        if (!isServer) return;

        // Don't worry about collision if disc is held
        if (discState == DiscState.HELD) return;

        if (collision.gameObject.name == "Ground")
        {
            Debug.Log("Hit ground at " + collision.GetContact(0).point + ", updating disc state to " + DiscState.GROUND);
            discState = DiscState.GROUND;
        }
    }

    IEnumerator CollisionsOnRoutine ()
    {
        yield return new WaitForSeconds(0.1f);
        discBody.detectCollisions = true;
    }

    IEnumerator CurveRoutine ()
    {
        while (Time.time < curveEndTime)
        {
            discBody.AddForce(curveDirection * curveValues[curveIndex]);
            yield return new WaitForFixedUpdate();
        }
    }
}
