using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState { FREE, DISC, MARKING, LAYOUT }

public class PlayerController : MonoBehaviour
{
    public float layoutSpeed = 10f;
    public float layoutDuration = 1f;
    public float layoutRecovery = 1f;

    public float turnSpeed = 100f;
    public float topSpeed = 1f;
    public float shuffleSpeed = 1f;

    public float pivotTurnSpeed = 200f;
    public float pivotHorizontalReach = 0.4f; // Max 1.2
    public float pivotVerticalReach = 0.3f; // Normal height = 1.5, min height = 1, max height 2. So max vertical reach is 0.5
    public float pivotSpeed = 1f;

    public float reach = 2f;

    public Transform heldDiscTransform;
    public CameraController cameraController;

    private Animator animator;
    private DiscController discController;
    private GameObject disc;
    private PlayerNetworkController playerNetworkController;

    private float v, h, sprint;
    private float layoutEnd;
    private Vector3 initialDiscPosition;

    public PlayerState playerState = PlayerState.FREE; // TODO: Make private
    public bool discCaught = false; // TODO: Make private

    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer()) return;

        animator = GetComponent<Animator>();
        disc = GameObject.Find("Disc");
        discController = disc.GetComponent<DiscController>();

        playerNetworkController = GetComponentInParent<PlayerNetworkController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer()) return;

        // Check sprinting
        if (Input.GetKey(KeyCode.LeftShift))
        {
            sprint = 0.2f;
        } else
        {
            sprint = 0f;
        }

        // Set disc alert to inactive by default
        discController.interactAlert.SetActive(false);

        // Check player state
        if (playerState == PlayerState.FREE)
        {
            checkFreeActions();
            checkForCatch();
        } else if (playerState == PlayerState.LAYOUT)
        {
            checkLayoutActions();
            checkForCatch();
        } else if (playerState == PlayerState.DISC)
        {
            checkDiscActions();
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer()) return;

        // Set animation variables
        animator.SetFloat("Walk", v);
        animator.SetFloat("Run", sprint);
        animator.SetFloat("Turn", h);

        if (playerState == PlayerState.DISC)
        {
            animator.SetFloat("Walk", 0);
            animator.SetFloat("Run", 0);
            animator.SetFloat("Turn", 0);
        }
    }

    void checkForCatch()
    {
        // Check is disc is catchable
        if (distanceToDisc() < reach && discController.getDiscState() == DiscState.FLIGHT)
        {
            discController.interactAlert.SetActive(true);
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
            {
                makeCatch();
            }
        }
    }

    void checkDiscActions()
    {
        // Vertical and horizontal input
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");

        // Enable rotating
        transform.Rotate(0, h * pivotTurnSpeed * Time.deltaTime, 0);

        // Do pivoting
        Vector3 newPosition = heldDiscTransform.localPosition;
        newPosition.x += pivotSpeed * Input.GetAxis("Mouse X");
        if (Mathf.Abs(newPosition.x - initialDiscPosition.x) > pivotHorizontalReach) newPosition.x = heldDiscTransform.localPosition.x;
        newPosition.y += pivotSpeed * Input.GetAxis("Mouse Y");
        if (Mathf.Abs(newPosition.y - initialDiscPosition.y) > pivotHorizontalReach) newPosition.y = heldDiscTransform.localPosition.y;
        heldDiscTransform.localPosition = newPosition;

        // Throw parameters
        ThrowDistance throwDistance = ThrowDistance.MEDIUM;
        ThrowCurve throwCurve = ThrowCurve.STRAIGHT;

        // Check input for throw distance
        if (Input.GetKey(KeyCode.Alpha1)) throwDistance = ThrowDistance.POP;
        if (Input.GetKey(KeyCode.Alpha2)) throwDistance = ThrowDistance.SHORT;
        if (Input.GetKey(KeyCode.Alpha3)) throwDistance = ThrowDistance.MEDIUM;
        if (Input.GetKey(KeyCode.Alpha4)) throwDistance = ThrowDistance.FAR;
        if (Input.GetKey(KeyCode.Alpha5)) throwDistance = ThrowDistance.HUCK;

        // Check input for throw curve
        if (Input.GetKey("z")) throwCurve = ThrowCurve.LEFT;
        if (Input.GetKey("x")) throwCurve = ThrowCurve.STRAIGHT;
        if (Input.GetKey("c")) throwCurve = ThrowCurve.RIGHT;

        // Check normal vs hammer throw
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
        {
            makeThrow(throwDistance, throwCurve, ThrowType.NORMAL);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            makeThrow((ThrowDistance) Random.Range(0, 4), (ThrowCurve) Random.Range(0, 2), ThrowType.NORMAL);
        }
    }

    void checkFreeActions()
    {
        // If disc caught, switch to DISC state and stop
        if (discCaught)
        {
            discCaught = false;
            playerState = PlayerState.DISC;
            cameraController.toggleFirstPersonCamera(); // Switch to FPS
            return;
        }

        // Vertical and horizontal input
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");

        
        if (distanceToDisc() < 2 && discController.getDiscState() == DiscState.GROUND)
        {
            discController.interactAlert.SetActive(true);

            // Pick up disc
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
            {
                pickup();
                return;
            }
        }

        // Try to layout
        if (Input.GetKeyDown("space"))
        {
            if (animator.GetFloat("Run") == 0.2f && !animator.GetBool("Jump"))
            {
                layout();
            }
        }

        // Move forward fast if running
        if (animator.GetFloat("Run") == 0.2f && v > 0)
        {
            transform.Translate(Vector3.forward * v * topSpeed * Time.deltaTime);
        }

        // Check for shuffling
        if (Input.GetKey("q"))
        {
            transform.Translate(-Vector3.right * shuffleSpeed * Time.deltaTime);
        }
        if (Input.GetKey("e"))
        {
            transform.Translate(Vector3.right * shuffleSpeed * Time.deltaTime);
        }
        if (Input.GetKey("s"))
        {
            transform.Translate(-Vector3.forward * shuffleSpeed * Time.deltaTime);
        }

        // Rotate fast if turning
        transform.Rotate(0, h * turnSpeed * Time.deltaTime, 0);
    }

    void checkLayoutActions()
    {
        v = v / 2;
        h = h / 2;
    }

    bool isLocalPlayer ()
    {
        return gameObject.transform.parent.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer;
    }

    void makeThrow (ThrowDistance throwDistance, ThrowCurve throwCurve, ThrowType throwType)
    {
        //Debug.Log("Throwing at distance " + throwDistance + ", curve " + throwCurve + ", and type " + throwType);
        heldDiscTransform.localPosition = initialDiscPosition; // Reset the held disc position
        playerState = PlayerState.FREE;
        discController.CmdMakeThrow(throwDistance, throwCurve, throwType);
        playerNetworkController.ReleaseDiscAuthority(); // Hand auth over disc back to server
        cameraController.toggleFirstPersonCamera();
    }

    void pickup ()
    {
        playerState = PlayerState.DISC;
        initialDiscPosition = heldDiscTransform.localPosition; // Save the initial position
        // Tell the disc that it is now held
        playerNetworkController.pickup(heldDiscTransform);
        cameraController.toggleFirstPersonCamera(); // Switch to FPS
    }

    void makeCatch ()
    {
        discCaught = true;
        initialDiscPosition = heldDiscTransform.localPosition; // Save the initial position
        // Tell the disc that it is now held
        playerNetworkController.pickup(heldDiscTransform);
    }

    float distanceToDisc ()
    {
        return (disc.transform.position - transform.position).magnitude;
    }

    void layout ()
    {
        animator.SetBool("Jump", true);
        layoutEnd = Time.time + layoutDuration;
        playerState = PlayerState.LAYOUT;
        StartCoroutine("LayoutRoutine");
    }

    IEnumerator LayoutRoutine()
    {
        // Build up
        while (Time.time < layoutEnd - layoutDuration * 0.7f)
        {
            transform.Translate(Vector3.forward * layoutSpeed / 2 * Time.deltaTime);
            yield return null;
        }

        // Lay
        while (Time.time < layoutEnd)
        {
            transform.Translate(Vector3.forward * layoutSpeed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(layoutRecovery);
        playerState = PlayerState.FREE;
    }
}
