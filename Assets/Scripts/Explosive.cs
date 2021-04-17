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
                Slice2D slice = Slicer2D.API.PolygonSlice(id.shape.GetLocal().ToWorldSpace(id.transform), slicePolygon);
                if (slice.GetPolygons().Count > 0)
                {
                    List<Polygon2D> results = slice.GetPolygons();

                    for (int i = 0; i < results.Count; ++i)
                    {
                        if (slicePolygonDestroy.PolyInPoly(results[i]) == true)
                        {
                            results.RemoveAt(i);
                        }
                    }

                    int indexOfLargest = 0;
                    if(results.Count > 1)
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

                    var origin = collision.collider.GetComponent<TanksTerrain>();
                    for (int i = 0; i < results.Count; ++i)
                    {
                        results[i] = results[i].ToLocalSpace(origin.transform);
                        Vector2[][] paths = results[i].GetPaths();
                        TanksTerrain terrain;
                        if (i == indexOfLargest)
                        {
                            terrain = origin;

                            // Apply the collider changes to the original terrain.
                            origin.HostTerrainDeformed(paths);                 
                        }
                        else
                        {
                            terrain = Instantiate(GameManager.Instance.prefabNetTerrain);
                            terrain.transform.parent = origin.transform.parent;
                            terrain.transform.position = origin.transform.position;
                            terrain.transform.rotation = origin.transform.rotation;

                            terrain.Sliceable.limit = new Limit();
                            terrain.Sliceable.limit.counter = origin.Sliceable.limit.counter + 1;
                            terrain.Sliceable.limit.maxSlices = origin.Sliceable.limit.maxSlices;
                            terrain.Sliceable.limit.enabled = origin.Sliceable.limit.enabled;

                            // Initialize the new networked terrain gameobject.
                            terrain.HostInit(paths);
                        }

                        // Update the host sliceable's shape.
                        terrain.Sliceable.shape = new Shape();
                        terrain.Sliceable.shape.SetShape(results[i]);
                        terrain.Sliceable.shape.ForceUpdate();
                    }
                }
            }

            Destroy(gameObject);

            Polygon2D.defaultCircleVerticesCount = 25;
        }
    }
}
