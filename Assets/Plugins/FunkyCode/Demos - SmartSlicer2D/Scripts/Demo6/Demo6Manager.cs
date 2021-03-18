using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D.Demo {
		
	public class Demo6Manager : MonoBehaviour {
		public GameObject bombPrefab;
		public GameObject bouncerPrefab;
		public Transform parent;

		void Update () {
			Vector2D pos = GetMousePosition ();

			if (UnityEngine.Input.GetMouseButtonDown (0)) {
				GameObject g = Instantiate (bombPrefab) as GameObject;
				g.transform.position = new Vector3 ((float)pos.x, (float)pos.y, -4.75f);
				g.transform.parent = transform;
			}

			if (UnityEngine.Input.GetMouseButtonDown (1)) {
				GameObject g = Instantiate (bouncerPrefab) as GameObject;
				g.transform.position = new Vector3 ((float)pos.x, (float)pos.y, -4.75f);
				g.transform.parent = transform;
			}
		}

		public static Vector2D GetMousePosition() {
			return(new Vector2D (Camera.main.ScreenToWorldPoint (UnityEngine.Input.mousePosition)));
		}
	}
}