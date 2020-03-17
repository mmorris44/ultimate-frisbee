using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject[] cameras;
    public GameObject firstPersonCamera;
    public int current = 0;

    private bool hasDisc = false;

    // Start is called before the first frame update
    void Start()
    {
        //if (!isLocalPlayer) return;
        //Cursor.visible = false;

        for (int i = 0; i < cameras.Length; ++i)
        {
            cameras[i].SetActive(false);
        }

        firstPersonCamera.SetActive(false);
        cameras[current].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        //if (!isLocalPlayer) return;

        if (Input.GetKeyDown("c") && !hasDisc)
        {
            cameras[current].SetActive(false);
            current = (current + 1) % cameras.Length;
            cameras[current].SetActive(true);
        }
    }

    public void toggleFirstPersonCamera()
    {
        if (hasDisc)
        {
            hasDisc = false;
            firstPersonCamera.SetActive(false);
            cameras[current].SetActive(true);
        } else
        {
            hasDisc = true;
            cameras[current].SetActive(false);
            firstPersonCamera.SetActive(true);
        }
    }
}
