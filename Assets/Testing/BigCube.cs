using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigCube : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown("space")) transform.Translate(0, 0, 1);
    }
}
