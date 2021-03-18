using System.Collections;
using System.Collections.Generic;
using Utilities2D;
using UnityEngine;

namespace Slicer2D.Demo {
	public class DemoSlicer2DAI : MonoBehaviour {
		public GameObject AIZone = null;
		private Polygon2D AIZonePolygon = null;

		InputController controller;

		public Polygon2D GetAIZone() {
			if (AIZonePolygon == null) {
				AIZonePolygon = Polygon2DList.CreateFromGameObject(AIZone)[0].ToWorldSpace(AIZone.transform);
			}
			return(AIZonePolygon);
		}

		void Start () {
			// Getting slicer input controller
			controller = GetComponent<Slicer2DLinearController>().input;
			
			// Enabling input events programming
			controller.SetRawInput(false, 0);
		}
		
		void Update () {

			UpdateAI(0);
		}

		Sliceable2D GetSlicerInZone() {
			foreach(Sliceable2D slicer in Sliceable2D.GetList()) {
				if (slicer.limit.counter > 0) {
					continue;
				}

				if (Math2D.PolyCollidePoly(slicer.shape.GetWorld(), GetAIZone()) == false) {
					continue;
				}

				return(slicer);
			}

			return(null);
		} 

		void UpdateAI(int id) {
			if (controller.Playing(id)) {
				return;
			}

			// Slicing the biggest object in the scene only
			Sliceable2D biggestObject = GetSlicerInZone();

			if (biggestObject != null) {
				
				controller.ClearActions(id);

				Polygon2D poly = biggestObject.shape.GetWorld();

				// Predict rigidbody movement
				Rigidbody2D rb = biggestObject.GetComponent<Rigidbody2D>();

				Vector2 pos = rb.position;
				Vector2 gravity = Physics2D.gravity;
				Vector2 force = rb.velocity;

				// Get center of the object
				Vector2 centerOffset = poly.GetBounds().center - pos;

				float timer = 0;
				while(timer < 0.25f) {
					float delta = 0.1f;

					pos += force * delta;
					force += gravity * delta;

					timer += delta;
				}

				centerOffset += pos;

				// Random slice rotation
				double sliceRotation = Random.Range(0f, Mathf.PI * 2);

				// Get bounds of an object to know the slice size
				Rect bounds = poly.GetBounds();
				float sliceSize = Mathf.Sqrt(bounds.width * bounds.width + bounds.height * bounds.height) * 1f;

				Vector2D firstPoint = new Vector2D(centerOffset);
				firstPoint.Push(sliceRotation, sliceSize);

				Vector2D secondPoint = new Vector2D(centerOffset);
				secondPoint.Push(sliceRotation, -sliceSize);

				// How fast to perform actions?
				float actionTime = 0.125f * 0.6f;

				controller.SetMouse(firstPoint.ToVector2(), actionTime, id);
				controller.PressMouse(actionTime, id);
				controller.MoveMouse(secondPoint.ToVector2(), actionTime, id);
				controller.ReleaseMouse(actionTime, id);
				controller.SetMouse(Vector2.zero, id);

				controller.Play(id);
			}
		}

		// Getting the biggest object in the scene
		/*
		Slicer2D GetBiggestObject() {
			Slicer2D obj = null;
			double area = -1e+10f;

			foreach(Slicer2D slicer in Slicer2D.GetList()) {
				Polygon2D poly = slicer.shape.GetLocal();
				if (poly.GetArea() > area) {
					obj = slicer;
					area = poly.GetArea();
				}
			}

			return(obj);
		} */
	}
}