using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMaker : MonoBehaviour {

    public int numberOfPoints = 100;
    public float scaleOuterSphere = 50f;
    public float scaleMultipleSpheres = 10f;
    public Material SphereMaterial;
    private bool done = false;

    // Use this for initialization
    void CreateSphere () {
        GameObject innerSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        innerSphere.transform.localScale = innerSphere.transform.localScale * (scaleOuterSphere * 2);
        innerSphere.transform.name = "Inner Sphere";
        innerSphere.SetActive(false);

        Vector3[] myPoints = GetPointsOnSphere(numberOfPoints);

        foreach (Vector3 point in myPoints)
        {
            GameObject outerSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            outerSphere.transform.localScale = outerSphere.transform.localScale * (scaleMultipleSpheres * 2);
            outerSphere.transform.GetComponent<Renderer>().sharedMaterial = SphereMaterial;
            outerSphere.transform.position = point * scaleOuterSphere;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!done)
        {
            CreateSphere();
            done = true;
        }
    }

    Vector3[] GetPointsOnSphere(int nPoints)
    {
        float fPoints = (float)nPoints;

        Vector3[] points = new Vector3[nPoints];

        float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
        float off = 2 / fPoints;

        for (int k = 0; k < nPoints; k++)
        {
            float y = k * off - 1 + (off / 2);
            float r = Mathf.Sqrt(1 - y * y);
            float phi = k * inc;

            points[k] = new Vector3(Mathf.Cos(phi) * r, y, Mathf.Sin(phi) * r);
        }

        return points;
    }
}
