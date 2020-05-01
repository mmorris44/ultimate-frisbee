using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Drops path along disc flight
public class PathDropper : MonoBehaviour
{
    public float dropInterval = 0.5f;

    public GameObject dropMarkerPrefab;
    public List<GameObject> dropMarkers = new List<GameObject>();

    DiscController discController;

    float lastDrop = 0;

    void Start()
    {
        discController = GetComponent<DiscController>();
    }

    void Update()
    {
        if (discController.getDiscState() == DiscState.FLIGHT && lastDrop + dropInterval < Time.time)
        {
            lastDrop = Time.time;
            GameObject gameObject = Instantiate(dropMarkerPrefab);
            gameObject.transform.position = transform.position;
            dropMarkers.Add(gameObject);
        } else if (discController.getDiscState() != DiscState.FLIGHT)
        {
            foreach(GameObject dropMarker in dropMarkers)
            {
                Destroy(dropMarker);
            }
            dropMarkers.Clear();
        }
    }
}