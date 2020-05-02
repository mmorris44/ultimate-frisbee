using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerControls : NetworkBehaviour
{
    private DiscController discController;
    private GameObject disc;

    void Start()
    {
        // Only allow if server
        if (!isServer) return;

        disc = GameObject.Find("Disc");
        discController = disc.GetComponent<DiscController>();
    }

    void Update()
    {
        // Only allow if server
        if (!isServer) return;

        if (Input.GetKeyDown("r"))
        {
            discController.ResetDisc();
        }
    }
}
