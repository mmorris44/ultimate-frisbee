using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float speed = 0.9f;
    public float minCamDistance = 5f;
    public float maxCamDistance = 10f;
    public float rotationSpeed = 10f;
    public float verticalOffset = 2f;

    // Update is called once per frame
    void LateUpdate()
    {
        if (!isLocalPlayer()) return;

        // Go to nice spot
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        if (direction.magnitude < minCamDistance || direction.magnitude > maxCamDistance)
        {
            Vector3 target = player.position;
            target.y = transform.position.y;
            target += Vector3.ClampMagnitude(-direction, 1) * minCamDistance;
            direction = target - transform.position;
            float step = direction.magnitude * speed * Time.deltaTime;
            //transform.position = Vector3.Lerp(transform.position, target, step);
            transform.position -= (transform.position - target) * step;
        }

        // Rotate on mouse X
        Vector3 playerCenter = player.position;
        playerCenter.y = 0;
        transform.RotateAround(playerCenter, Vector3.up, Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime);

        transform.LookAt(new Vector3(player.position.x, player.position.y + verticalOffset, player.position.z));
    }

    bool isLocalPlayer()
    {
        return gameObject.transform.parent.parent.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer;
    }
}
