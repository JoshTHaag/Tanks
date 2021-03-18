using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slicer2D.Demo {
	
	public class DemoSpinBlade : MonoBehaviour {
		void Update () {
			GetComponent<Rigidbody2D>().AddTorque(15);
		}
	}
}
