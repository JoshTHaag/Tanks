using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {
    
    public class BombScript : MonoBehaviour {
        public Collider2D sliceArea;
        public int particleLayer;

        void Update() {
           Slice();
        }

        void Slice() {
            Polygon2D.defaultCircleVerticesCount = 15;

            Polygon2D bombPoly = Polygon2DList.CreateFromGameObject(sliceArea.gameObject)[0];
    
            Polygon2D slicePolygon = bombPoly.ToWorldSpace(sliceArea.gameObject.transform);
            Polygon2D slicePolygonDestroy = bombPoly.ToScale(new Vector2(1.05f, 1.05f)).ToWorldSpace(sliceArea.gameObject.transform);

            foreach (Sliceable2D id in Sliceable2D.GetListCopy()) {
               
                
                Slice2D result = Slicer2D.API.PolygonSlice (id.shape.GetWorld(), slicePolygon);

   
                if (result.GetPolygons().Count > 0) {

                    foreach (Polygon2D p in new List<Polygon2D>(result.GetPolygons())) {
                        if (slicePolygonDestroy.PolyInPoly (p) == true) {
                           //result.GetPolygons().Remove (p);
                        }
                    }
        
                    if (result.GetPolygons().Count > 0) {
                        List<GameObject> gList = id.PerformResult (result.GetPolygons(), result);

                        SliceParticles(gList, result, slicePolygonDestroy);

                    } else {
                        // Polygon is Destroyed!!!
                        Destroy (id.gameObject);
                    }
                }
            }

            
            Polygon2D.defaultCircleVerticesCount = 25;

           // Destroy(this);
        }

    public void SliceParticles(List<GameObject> gList, Slice2D result, Polygon2D destroyPoly) {
         foreach(GameObject g in gList) {
            if (destroyPoly.PolyInPoly (result.GetPolygons()[gList.IndexOf(g)]) == true) {
                
                Slice2D slice = g.GetComponent<Sliceable2D>().Explode();

                foreach(GameObject p in slice.GetGameObjects()) {
                    p.AddComponent<Rigidbody2D>();
                    p.layer = particleLayer;
                    Destroy(p.GetComponent<Sliceable2D>());
                }

                Slicer2D.AddForce.ExplodeByPoint(slice, 500, new Vector2D(transform.position));
                
            }
        }
    }
    }


}