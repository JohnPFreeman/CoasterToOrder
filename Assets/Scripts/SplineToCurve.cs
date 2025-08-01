using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteAlways]
public class SplineToCurve : MonoBehaviour
{

    [Header("Curve Generator")]
    public CurveGen curveGen;

    public SplineContainer splineContainer;


    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        Spline trackSpline = splineContainer.Spline;
        trackSpline.Clear();

        List<Transform> knotPoints = curveGen.controlPoints;

        for (int i = 0; i < knotPoints.Count; i++) 
        { 
           trackSpline.Add(new BezierKnot(knotPoints[i].position), TangentMode.AutoSmooth);
        }

    }
}
