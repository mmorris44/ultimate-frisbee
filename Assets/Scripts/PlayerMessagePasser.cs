using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMessagePasser : NetworkBehaviour
{
    private DiscController discController;
    private GameObject disc;

    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer) return;

        disc = GameObject.Find("Disc");
        discController = disc.GetComponent<DiscController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command]
    public void CmdPickup(Transform heldDiscTransform)
    {
        Debug.Log("Executing pickup command on server");
        discController.pickup(heldDiscTransform);
    }
}
