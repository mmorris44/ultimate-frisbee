using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFree : MonoBehaviour
{
    public float speedH = 2.0f;
    public float speedV = 2.0f;
    public float cameraSpeed = 15f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    void Update()
    {
        if (!isLocalPlayer()) return;
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    bool isLocalPlayer()
    {
        return gameObject.transform.parent.parent.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer;
    }
}
