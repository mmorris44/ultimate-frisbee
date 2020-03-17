using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer()) return;

        if (Input.GetKeyDown("a")) transform.Translate(1, 0, 0);
        if (Input.GetKeyDown("d")) transform.Translate(-1, 0, 0);
    }

    bool isLocalPlayer()
    {
        return gameObject.transform.parent.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer;
    }
}
