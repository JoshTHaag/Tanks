using System.Collections.Generic;
using UnityEngine;

namespace Slicer2D.Demo {
	public class DemoSceneDestroyer : MonoBehaviour {
		public void DestroyScene() {
			Destroy(transform.parent.gameObject);
		}
	}
}

