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

        var hitTerrain = collision.transform.GetComponent<TanksTerrain>();
        if (hitTerrain)
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
                Slice2D slice = Slicer2D.API.PolygonSlice(id.shape.GetLocal().ToWorldSpace(id.transform), slicePolygon);
                if (slice.GetPolygons().Count > 0)
                {
                    List<Polygon2D> results = slice.GetPolygons();

                    for (int i = 0; i < results.Count; ++i)
                    {
                        if (slicePolygonDestroy.PolyInPoly(results[i]) == true)
                        {
                            results.RemoveAt(i);
                            i--;
                        }
                    }

                    UnityEngine.Debug.Log("slice results: " + results.Count);

                    if(results.Count > 0)
                    {
                        int indexOfLargest = 0;
                        if (results.Count > 1)
                        {
                            double prevLargestArea = double.MinValue;
                            for (int i = 0; i < results.Count; ++i)
                            {
                                double area = results[i].GetArea();
                                if (area > prevLargestArea)
                                {
                                    indexOfLargest = i;
                                    prevLargestArea = area;
                                }
                            }
                        }

                        for (int i = 0; i < results.Count; ++i)
                        {
                            results[i] = results[i].ToLocalSpace(hitTerrain.transform);
                            Vector2[][] paths = results[i].GetPaths();
                            TanksTerrain terrain;
                            if (i == indexOfLargest)
                            {
                                terrain = hitTerrain;

                                // Apply the collider changes to the original terrain.
                                hitTerrain.HostTerrainDeformed(paths);
                            }
                            else
                            {
                                terrain = Instantiate(hitTerrain.prefabNetTerrain);
                                terrain.transform.parent = hitTerrain.transform.parent;
                                terrain.transform.position = hitTerrain.transform.position;
                                terrain.transform.rotation = hitTerrain.transform.rotation;

                                terrain.Sliceable.limit = new Limit();
                                terrain.Sliceable.limit.counter = hitTerrain.Sliceable.limit.counter + 1;
                                terrain.Sliceable.limit.maxSlices = hitTerrain.Sliceable.limit.maxSlices;
                                terrain.Sliceable.limit.enabled = hitTerrain.Sliceable.limit.enabled;

                                // Initialize the new networked terrain gameobject.
                                terrain.HostInit(paths);
                            }

                            // Update the host sliceable's shape.
                            terrain.Sliceable.shape = new Shape();
                            terrain.Sliceable.shape.SetShape(results[i]);
                            terrain.Sliceable.shape.ForceUpdate();
                        }

                    }
                    else
                    {
                        if(id.GetComponent<TanksTerrain>())
                            Destroy(id.gameObject);
                    }

                }
            }

            Destroy(gameObject);

            Polygon2D.defaultCircleVerticesCount = 25;
        }
    }
}
