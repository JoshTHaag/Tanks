using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class Terrain : MonoBehaviour
{
    public string sortingLayerName;
    public int sortingOrder;

    new PolygonCollider2D collider;

    void Start()
    {
        collider = gameObject.GetComponent<PolygonCollider2D>();

        GenerateMesh();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if(!EditorApplication.isPlaying && !EditorApplication.isPaused)
        {
            UpdateTerrain();
        }
    }
#endif

    public void UpdateTerrain()
    {
        GenerateMesh();
    }

    void GenerateMesh()
    {
        MeshFilter mf = GetComponent<MeshFilter>();

        //int pointCount = collider.GetTotalPointCount();

        //Mesh mesh = new Mesh();
        //mesh.name = "Generated Mesh - " + gameObject.name;

        //Vector2[] points = collider.points;
        //Vector3[] vertices = new Vector3[pointCount];
        //Vector2[] uv = new Vector2[pointCount];

        //for (int j = 0; j < pointCount; j++)
        //{
        //    Vector2 actual = points[j];
        //    vertices[j] = new Vector3(actual.x, actual.y, 0);
        //    uv[j] = actual;
        //}

        //Triangulator tr = new Triangulator(points);
        //int[] triangles = tr.Triangulate();
        //mesh.vertices = vertices;
        //mesh.triangles = triangles;
        //mesh.uv = uv;

        Mesh mesh = collider.CreateMesh(false, false);

        Vector3[] verts = mesh.vertices;
        Vector2[] uv = new Vector2[verts.Length];

        for (int x = 0; x < mesh.vertices.Length; ++x)
        {
            verts[x] -= transform.position;
            verts[x].x /= transform.lossyScale.x;
            verts[x].y /= transform.lossyScale.y;
            uv[x] = verts[x];
        }

        mesh.vertices = verts;
        mesh.uv = uv;
        mesh.RecalculateBounds();

        mesh.name = "Generated Mesh - " + gameObject.name;

        mf.mesh = mesh;
    }

    private void OnValidate()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.sortingLayerName = sortingLayerName;
        mr.sortingOrder = sortingOrder;
    }
}