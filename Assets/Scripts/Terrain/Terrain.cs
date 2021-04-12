using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class Terrain : NetworkBehaviour
{
    public string sortingLayerName;
    public int sortingOrder;

    new PolygonCollider2D collider;

    void Start()
    {
        collider = gameObject.GetComponent<PolygonCollider2D>();

        GenerateMesh();
    }

    [ClientRpc]
    public void TerrainSliced_ClientRpc(Vector2[][] newPaths)
    {
        Debug.LogError("TerrainSliced_ServerRpc");

        //int maxPaths = Mathf.Max(collider.pathCount, newPaths.Length);

        //for(int i = 0; i < maxPaths; ++i)
        //{
        //    if(i < newPaths.Length)
        //    {
        //        collider.SetPath(i, newPaths[i]);
        //    }
        //    else
        //    {
        //        collider.SetPath(i, new Vector2[0]);
        //    }
        //}
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