using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkController : NetworkBehaviour
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

    public void pickup (Transform heldDiscTransform)
    {
        discController.pickup(heldDiscTransform); // Pickup locally
        RequestDiscAuthority(); // Propogate to authoritative server
    }

    public void RequestDiscAuthority ()
    {
        // Request authority for disc
        NetworkIdentity id = disc.GetComponent<NetworkIdentity>();
        //Debug.Log("[CLIENT] Requesting auth for " + id + " with current auth status " + id.hasAuthority);
        CmdRequestAuthority(id);
    }

    public void ReleaseDiscAuthority ()
    {
        // Release authority for disc
        NetworkIdentity id = disc.GetComponent<NetworkIdentity>();
        //Debug.Log("[CLIENT] Releasing auth for " + id + " with current auth status " + id.hasAuthority);
        CmdReleaseAuthority(id);
    }

    [Command]
    void CmdRequestAuthority(NetworkIdentity otherId)
    {
        //Debug.Log("[SERVER] Assigning auth for " + otherId + " to " + connectionToClient.identity);
        otherId.AssignClientAuthority(connectionToClient);
    }

    [Command]
    void CmdReleaseAuthority(NetworkIdentity otherId)
    {
        //Debug.Log("[SERVER] Releasing auth for " + otherId);
        otherId.RemoveClientAuthority();
    }
}
