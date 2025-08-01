using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class CurveGen : MonoBehaviour
{
    [Header("Control Points")]
    public List<Transform> controlPoints = new List<Transform>();

    [Header("Curve Settings")]
    [Range(2, 100)]
    public int segmentCount = 24;

    [Header("Perpendicular Line Settings")]
    public float perpendicularLength = 1.0f;
    public float heightMultiplier = 0.5f;
    public List<float> rotationAngles = new List<float>();

    [Header("Color Settings")]
    public Color curveColorDebug;
    public Color LeftPointsColor;
    public Color RightPointsColor;

    //lists
    public List<Vector3> mainPoints = new List<Vector3>();
    public List<Vector3> leftPoints = new List<Vector3>();
    public List<Vector3> rightPoints = new List<Vector3>();

    private void Update()
    {
        mainPoints.Clear();
        leftPoints.Clear();
        rightPoints.Clear();

        rotationAngles = Enumerable.Repeat(0f, controlPoints.Count).ToList();

        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            //Get the segment points
            Transform p0 = i == 0 ? controlPoints[i] : controlPoints[i - 1];
            Transform p1 = controlPoints[i];
            Transform p2 = controlPoints[i + 1];
            Transform p3 = (i + 2 < controlPoints.Count) ? controlPoints[i + 2] : controlPoints[i + 1];

            //Interpolate rotation between control points
            float angleStart = rotationAngles[i];
            float angleEnd = rotationAngles[i + 1];

            //Draw Catmull-Rom curve for this segment
            for (int j = 0; j < segmentCount; j++)
            {
                float t0 = (float)j / segmentCount;
                float t1 = (float)(j + 1) / segmentCount;

                Vector3 pointA = GetCatMullRomPosition(t0, p0.position, p1.position, p2.position, p3.position);
                Vector3 pointB = GetCatMullRomPosition(t1, p0.position, p1.position, p2.position, p3.position);

                mainPoints.Add(pointA);

                float interpolatedAngle = Mathf.LerpAngle(angleStart, angleEnd, t0);

                //Calculations
                Vector3 tangent = (pointB - pointA).normalized;
                Vector3 perpendicular = Vector3.Cross(tangent, Vector3.up).normalized * perpendicularLength;
                Quaternion rotation = Quaternion.AngleAxis(interpolatedAngle, tangent);
                Vector3 rotatedPerpendicular = rotation * perpendicular;
                Vector3 upDirection = Vector3.Cross(tangent, rotatedPerpendicular).normalized;
                Vector3 heightAdjustment = upDirection * heightMultiplier;
                Vector3 leftPoint = pointA - rotatedPerpendicular + heightAdjustment;
                Vector3 rightPoint = pointA + rotatedPerpendicular + heightAdjustment;

                //Move outter bars up
                float curveHeightAtPoint = Vector3.Dot(heightAdjustment, upDirection);
                leftPoint += upDirection * curveHeightAtPoint;
                rightPoint += upDirection * curveHeightAtPoint;

                //Add endpoints lists
                leftPoints.Add(leftPoint);
                rightPoints.Add(rightPoint);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = curveColorDebug;
        for (int i = 0; i < mainPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(mainPoints[i], mainPoints[i + 1]);
        }

        Gizmos.color = LeftPointsColor;
        for (int i = 0; i < mainPoints.Count; i++)
        {
            Gizmos.DrawLine(mainPoints[i], leftPoints[i]);
        }

        Gizmos.color = RightPointsColor;
        for (int i = 0; i < mainPoints.Count; i++)
        {
            Gizmos.DrawLine(mainPoints[i], rightPoints[i]);
        }
    }

    private Vector3 GetCatMullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        float f0 = -0.5f * t3 + t2 - 0.5f * t;
        float f1 = 1.5f * t3 - 2.5f * t2 + 1.0f;
        float f2 = -1.5f * t3 + 2.0f * t2 + 0.5f * t;
        float f3 = 0.5f * t3 - 0.5f * t2;

        return f0 * p0 + f1 * p1 + f2 * p2 + f3 * p3;
    }


    private class PiecewiseCubic
    {
        Vector3 a, b, c, d;
        float length;
        //string type = "normal";

        public PiecewiseCubic(Vector3 xi, Vector3 xf, Vector3 dxi, Vector3 dxf)
        {
            a = dxf + dxi + 2 * (xi - xf);
            b = 3 * (xf - xi) - 2 * dxi - dxf; 
            c = dxi;
            d = xi;
        }
    }
}