using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D.Demo {
	
	public class DemoSlicer2DInspectorTracker : MonoBehaviour {
		public double originalSize = 0;
		
		void Start () {
			if (originalSize == 0) {
				originalSize = Polygon2DList.CreateFromGameObject(gameObject)[0].ToWorldSpace(transform).GetArea();
			}
		}

	}
}