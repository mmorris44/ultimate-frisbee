using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerControls : NetworkBehaviour
{
    public DiscController discController;

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
