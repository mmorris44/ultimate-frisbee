using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPlotter : MonoBehaviour
{
    public float plotGap = 0.1f;
    public float distance = 100f;
    public float highPoint = 20f;

    public GameObject pathMarkerPrefab;

    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            PlotPath();
        }
    }

    public void PlotPath()
    {
        float x = plotGap;
        float[] quadParams = GetABCForQuadratic(highPoint, distance);

        while (x < distance)
        {
            GameObject pathMarker = Instantiate(pathMarkerPrefab);
            pathMarker.transform.parent = this.transform;
            pathMarker.transform.localPosition = new Vector3(0, CalculateQuadratic(quadParams, x), x);
            x += plotGap;
        }
    }

    // Get params for given quadratic
    float[] GetABCForQuadratic(float highPoint, float distance)
    {
        float a = -4 * highPoint / (distance * distance);
        float b = 4 * highPoint / distance;
        return new float[3] { a, b, 0 };
    }

    // Calculate quadratic at point
    float CalculateQuadratic(float[] abcParams, float x)
    {
        return abcParams[0] * x * x + abcParams[1] * x + abcParams[2];
    }
}
