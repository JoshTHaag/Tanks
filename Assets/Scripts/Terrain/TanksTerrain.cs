using UnityEngine;
using Slicer2D;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;
using MLAPI.Transports;
using UnityEngine.Assertions;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class TanksTerrain : NetworkBehaviour
{
    public string sortingLayerName;
    public int sortingOrder;

    NetworkVariable<Vector2[][]> colliderPaths = new NetworkVariable<Vector2[][]>(new NetworkVariableSettings() { 
        //SendTickrate = -1f
        SendNetworkChannel = (NetworkChannel)Tanks.Networking.TanksNetworkChannel.TerrainUpdate
    });

    new PolygonCollider2D collider;

    Sliceable2D sliceable;

    void Start()
    {
        collider = GetComponent<PolygonCollider2D>();
        sliceable = GetComponent<Sliceable2D>();

        // The host already has the correct paths at this point.
        if (!IsHost)
        {
            colliderPaths.OnValueChanged += OnColliderPathsChanged;
            SetColliderPaths(colliderPaths.Value);
            GenerateMesh();
        }
    }

    private void OnDestroy()
    {
        UnityEngine.Debug.Log("Destroyed terrain");
    }

    public void HostInit(Vector2[][] colliderPaths)
    {
        UnityEngine.Debug.Log("Spawned new terrain");

        Assert.IsFalse(NetworkObject.IsSpawned, "HostInit called for TanksTerrain that is already spawned!");

        collider = GetComponent<PolygonCollider2D>();
        sliceable = GetComponent<Sliceable2D>();

        this.colliderPaths.Value = colliderPaths;

        GenerateMesh();

        NetworkObject.Spawn(null, true);
    }

    void OnColliderPathsChanged(Vector2[][] oldPaths, Vector2[][] newPaths)
    {
        SetColliderPaths(newPaths);

        GenerateMesh();
    }

    void SetColliderPaths(Vector2[][] newPaths)
    {
        int maxPaths = Mathf.Max(collider.pathCount, newPaths.Length);

        for (int i = 0; i < maxPaths; ++i)
        {
            if (i < newPaths.Length)
            {
                collider.SetPath(i, newPaths[i]);
            }
            else
            {
                collider.SetPath(i, new Vector2[0]);
            }
        }
    }

    public void HostTerrainDeformed(Vector2[][] newPaths)
    {
        if (!IsHost)
            return;

        colliderPaths.Value = newPaths;

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

    public void GenerateMesh()
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