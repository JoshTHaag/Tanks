using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;
using Slicer2D;

[RequireComponent(typeof(Rigidbody2D))]
public class Explosive : MonoBehaviour
{
    public float explosiveness = 1f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!TanksNetworkManager.Singleton.IsHost)
            return;

        if (collision.collider.name.Contains("Terrain"))
        {
            Vector2D pos = new Vector2D(transform.position);

            Polygon2D.defaultCircleVerticesCount = Mathf.Clamp((int)(24 * explosiveness), 8, 64);

            Polygon2D slicePolygon = Polygon2D.Create(Polygon2D.PolygonType.Circle, explosiveness);
            Polygon2D slicePolygonDestroy = Polygon2D.Create(Polygon2D.PolygonType.Circle, explosiveness + 0.5f);

            slicePolygon = slicePolygon.ToOffset(pos);
            slicePolygonDestroy = slicePolygonDestroy.ToOffset(pos);

            foreach (Sliceable2D id in Sliceable2D.GetListCopy())
            {
                id.applySliceToOrigin = true;
                Slice2D result = Slicer2D.API.PolygonSlice(id.shape.GetLocal().ToWorldSpace(id.transform), slicePolygon);
                if (result.GetPolygons().Count > 0)
                {
                    foreach (Polygon2D p in new List<Polygon2D>(result.GetPolygons()))
                    {
                        if (slicePolygonDestroy.PolyInPoly(p) == true)
                        {
                            result.GetPolygons().Remove(p);
                        }
                    }

                    if (result.GetPolygons().Count > 0)
                    {
                        var newTerrains = id.PerformResult(result.GetPolygons(), new Slice2D());

                        foreach(var newTerrain in newTerrains)
                        {
                            var tanksTerrain = newTerrain.GetComponent<TanksTerrain>();
                            if(tanksTerrain)
                            {
                                PolygonCollider2D collider = collision.collider as PolygonCollider2D;
                                Vector2[][] paths = new Vector2[collider.pathCount][];
                                for (int i = 0; i < collider.pathCount; ++i)
                                {
                                    paths[i] = collider.GetPath(i);
                                }

                                if (tanksTerrain.NetworkObject.IsSpawned)
                                    tanksTerrain.HostTerrainDeformed(paths);
                                else
                                    tanksTerrain.HostInit(paths);
                            }
                        }
                    }
                    else
                    {
                        // Polygon is Destroyed.
                        Destroy(id.gameObject);
                    }
                }
            }

            Destroy(gameObject);

            Polygon2D.defaultCircleVerticesCount = 25;
        }
    }
}
