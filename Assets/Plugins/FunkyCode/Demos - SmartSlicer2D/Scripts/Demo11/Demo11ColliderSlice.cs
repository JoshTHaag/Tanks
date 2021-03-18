using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D.Demo {
	
	public class Demo11ColliderSlice : MonoBehaviour {

		void Update () {
			if (UnityEngine.Input.GetKeyDown(KeyCode.Space)) {
				Slice();
			}
		}

		public void Slice() {
			float timer = Time.realtimeSinceStartup;
				
			foreach(Transform t in transform) {
				Polygon2D poly = Polygon2DList.CreateFromGameObject(t.gameObject)[0].ToWorldSpace(t);

				Slicing.ComplexSliceAll(poly.pointsList, Layer.Create());
			}

			Destroy(gameObject);

			Debug.Log(name + " in " + (Time.realtimeSinceStartup - timer) * 1000 + "ms");
		}
	}
}