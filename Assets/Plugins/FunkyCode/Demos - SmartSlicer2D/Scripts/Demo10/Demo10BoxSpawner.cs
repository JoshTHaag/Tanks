using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D.Demo {

	public class Demo10BoxSpawner : MonoBehaviour {
		public GameObject spawnObject;
		public TimerHelper time;
		private int id = 0;
		
		void SpawnBox() {
			GameObject box = Instantiate(spawnObject, transform) as GameObject;

			box.name = "Box " + id;
			id++;

			box.transform.parent = transform;
			box.transform.localPosition = new Vector3(0, 10, -5);
		}

		void Start () {
			SpawnBox();
			time = TimerHelper.Create();
		}
		
		void Update () {
			if (time.GetMillisecs() > 3750) {
				SpawnBox();
				time = TimerHelper.Create();
			}
		}
	}
}