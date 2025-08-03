using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteAlways]
public class CoasterGen : MonoBehaviour
{
    [Header("Curve Generator")]
    public CurveGen curveGenorator;

    [Header("Mesh Settings")]
    [Range(0, 1)] public float radiusTubes = 0.2f;
    [Range(0, 1)] public float radiusSmaller = 0.1f;
    [Range(0, 1)] public float radiusInners = 0.05f;
    //[Range(0, 1)] public float radiusPosts = 0.4f;
    public int radialSegments = 16;

    [Header("Mesh GameObjects")]
    public GameObject RightBar;
    public GameObject LeftBar;
    public GameObject InnerBars;
    //public GameObject PostBar;

    [Header("Materials")]
    public Material MainTrackColor;
    public Material InnerTrackColor;
    public Material BarTrackColor;
    //public Material PostColor;

    //Points Lists
    private List<Vector3> pointsMain;
    private List<Vector3> pointsLeft;
    private List<Vector3> pointsRight;

    //public List<Vector3> pointsPosts;
    //public List<Vector3> pointsPostsGround;

    //Meshes
    private Mesh meshMain;
    private Mesh meshLeft;
    private Mesh meshRight;
    private Mesh meshInners;
    //private Mesh meshPosts;

    private void Update()
    {
        //set points lists (If you want to use your own points you can remove this and set them manually)
        pointsMain = curveGenorator.mainPoints;
        pointsLeft = curveGenorator.leftPoints;
        pointsRight = curveGenorator.rightPoints;

 
        

        //Generate Main Tube
        GenerateTube(meshMain, pointsMain, radiusTubes, GetComponent<MeshFilter>());
        GenerateTube(meshLeft, pointsLeft, radiusSmaller, RightBar.GetComponent<MeshFilter>());
        GenerateTube(meshRight, pointsRight, radiusSmaller, LeftBar.GetComponent<MeshFilter>());
        

        //Generate Inner Connection Bars
        GenerateCylinders(curveGenorator.mainPoints, curveGenorator.leftPoints, curveGenorator.mainPoints, curveGenorator.rightPoints, meshInners, InnerBars.GetComponent<MeshFilter>(), radiusInners);


        //Generate Support Posts
        /*pointsPosts.Clear();
        pointsPostsGround.Clear();
        for (int i = 0; i < curveGenorator.controlPoints.Count; i++)
        {
            pointsPosts.Add(curveGenorator.controlPoints[i].position);
            pointsPostsGround.Add(new Vector3(curveGenorator.controlPoints[i].position.x, 0, curveGenorator.controlPoints[i].position.z));

        }*/

        //GenerateCylinders(pointsPostsGround, pointsPosts, new List<Vector3>(), new List<Vector3>(), meshPosts, PostBar.GetComponent<MeshFilter>(), radiusPosts);
        //Debug.Log(new Vector2(pointsPosts[0][0], curveGenorator.controlPoints[0].position[0]));
        //Debug.Log(pointsPostsGround);

        //Update Track Colors
        if (MainTrackColor != null) { GetComponent<MeshRenderer>().material = MainTrackColor; };
        if (InnerTrackColor != null) { InnerBars.GetComponent<MeshRenderer>().material = InnerTrackColor; };
        if (BarTrackColor != null) { RightBar.GetComponent<MeshRenderer>().material = BarTrackColor; };
        if (BarTrackColor != null) { LeftBar.GetComponent<MeshRenderer>().material = BarTrackColor; };
    }

    void GenerateCylinders(List<Vector3> list1, List<Vector3> list2, List<Vector3> list3, List<Vector3> list4, Mesh targetMesh, MeshFilter targetMeshFilter, float radius)
    {
        //Make sure everything is correct input
        if (list1 == null || list2 == null || list3 == null || list4 == null)
        {
            Debug.LogError("All four lists must be initialized.");
            return;
        }

        if (list1.Count != list2.Count || list3.Count != list4.Count)
        {
            Debug.LogError("List1 and List2 must have the same number of points. List3 and List4 must also have the same number of points.");
            return;
        }

        //Create the mesh
        targetMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        //Generate cylinders for List 1 > List 2
        for (int i = 0; i < list1.Count; i++)
        {
            GenerateCylinder(list1[i], list2[i], vertices, triangles, uv, normals, radius);
        }

        //Generate cylinders for List 3 > List 4
        for (int i = 0; i < list3.Count; i++)
        {
            GenerateCylinder(list3[i], list4[i], vertices, triangles, uv, normals, radius);
        }

        //Update to the mesh
        targetMesh.vertices = vertices.ToArray();
        targetMesh.triangles = triangles.ToArray();
        targetMesh.uv = uv.ToArray();
        targetMesh.normals = normals.ToArray();

        targetMeshFilter.mesh = targetMesh;
    }

    void GenerateCylinder(Vector3 start, Vector3 end, List<Vector3> vertices, List<int> triangles, List<Vector2> uv, List<Vector3> normals, float radius)
    {
        Vector3 direction = (end - start).normalized;
        float length = Vector3.Distance(start, end);

        //Create a basis for the cylinder
        Vector3 up = Vector3.up;
        if (Vector3.Dot(up, direction) > 0.9f)
        {
            up = Vector3.right;
        }
        Vector3 right = Vector3.Cross(direction, up).normalized;
        up = Vector3.Cross(right, direction).normalized;

        int vertexStartIndex = vertices.Count;

        //Generate vertices and normals
        for (int i = 0; i <= 1; i++) // Two circles: start and end
        {
            Vector3 center = i == 0 ? start : end;
            for (int j = 0; j < radialSegments; j++)
            {
                float angle = j * Mathf.PI * 2f / radialSegments;
                Vector3 radialPoint = center + right * Mathf.Cos(angle) * radius+ up * Mathf.Sin(angle) * radius;
                vertices.Add(radialPoint);
                normals.Add((radialPoint - center).normalized);
                uv.Add(new Vector2(j / (float)radialSegments, i));
            }
        }

        //Generate triangles
        for (int j = 0; j < radialSegments; j++)
        {
            int nextJ = (j + 1) % radialSegments;
            int baseIndexStart = vertexStartIndex;
            int baseIndexEnd = vertexStartIndex + radialSegments;

            triangles.Add(baseIndexStart + j);
            triangles.Add(baseIndexEnd + j);
            triangles.Add(baseIndexEnd + nextJ);

            triangles.Add(baseIndexStart + j);
            triangles.Add(baseIndexEnd + nextJ);
            triangles.Add(baseIndexStart + nextJ);
        }
    }

    public void GenerateTube(Mesh targetMesh, List<Vector3> points, float radius_, MeshFilter meshFilter)
    {
        if (points == null || points.Count < 2)
        {
            Debug.LogError("At least two points are required to generate a tube.");
            return;
        }

        targetMesh = new Mesh();
        Vector3[] vertices = new Vector3[points.Count * radialSegments];
        int[] triangles = new int[(points.Count - 1) * radialSegments * 6];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector3[] normals = new Vector3[vertices.Length];

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 center = points[i];
            Vector3 forward = i < points.Count - 1 ? (points[i + 1] - center).normalized : (center - points[i - 1]).normalized;
            Vector3 up = Vector3.up;
            if (Vector3.Dot(forward, up) > 0.9f)
            {
                up = Vector3.right;
            }
            Vector3 right = Vector3.Cross(forward, up).normalized;
            up = Vector3.Cross(right, forward).normalized;

            for (int j = 0; j < radialSegments; j++)
            {
                float angle = j * Mathf.PI * 2f / radialSegments;
                Vector3 radialPoint = center + right * Mathf.Cos(angle) * radius_ + up * Mathf.Sin(angle) * radius_;
                vertices[i * radialSegments + j] = radialPoint;
                uv[i * radialSegments + j] = new Vector2(j / (float)radialSegments, i / (float)(points.Count - 1));
                normals[i * radialSegments + j] = (radialPoint - center).normalized;
            }
        }

        int triIndex = 0;
        for (int i = 0; i < points.Count - 1; i++)
        {
            for (int j = 0; j < radialSegments; j++)
            {
                int nextJ = (j + 1) % radialSegments;
                int baseIndex = i * radialSegments;
                int nextBaseIndex = (i + 1) * radialSegments;

                triangles[triIndex++] = baseIndex + j;
                triangles[triIndex++] = nextBaseIndex + j;
                triangles[triIndex++] = nextBaseIndex + nextJ;

                triangles[triIndex++] = baseIndex + j;
                triangles[triIndex++] = nextBaseIndex + nextJ;
                triangles[triIndex++] = baseIndex + nextJ;
            }
        }

        //Setting Meshes
        targetMesh.vertices = vertices;
        targetMesh.triangles = triangles;
        targetMesh.uv = uv;
        targetMesh.normals = normals;

        meshFilter.mesh = targetMesh;
    }
}