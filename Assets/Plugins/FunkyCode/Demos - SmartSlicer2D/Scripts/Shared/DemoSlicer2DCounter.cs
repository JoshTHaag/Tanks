using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slicer2D.Demo {
	
	public class DemoSlicer2DCounter : MonoBehaviour {
		public static int sliceCount = 0;

		void Start () {
			Sliceable2D slicer = GetComponent<Sliceable2D>();
			slicer.AddResultEvent(SliceEvent);
		}
		
		void SliceEvent (Slice2D slice) {
			sliceCount ++;
			Debug.Log("Slice Count: " + sliceCount);
		}
	}
}