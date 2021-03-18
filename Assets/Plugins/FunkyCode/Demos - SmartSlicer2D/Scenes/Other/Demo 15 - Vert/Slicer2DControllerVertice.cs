using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities2D.Extensions;
using Utilities2D;

namespace Slicer2D {

	public class Slicer2DControllerVertice : MonoBehaviour {
		// Controller Visuals
		public Visuals visuals = new Visuals();

		public Sliceable2D target = null;
		public int verticeID = 0;

		// Physics Force
		public bool addForce = true;
		public float addForceAmount = 5f;

		// Mouse Events
		private Pair2 linearPair = Pair2.zero;

		// Input
		public InputController input = new InputController();

		public void Start() {
			visuals.Initialize(gameObject);
		}

		public void Update() {
			input.Update();
		
			if (visuals.drawSlicer == false) {
				return;
			}

			if (linearPair.a == Vector2.zero && linearPair.b == Vector2.zero) {
				return;
			}
			
			visuals.Clear();
			visuals.GenerateLinearMesh(linearPair);
			visuals.Draw();

		
			if (target != null) {
				Polygon2D poly = target.shape.GetWorld();

				int pointIDA = ((verticeID - 1) + poly.pointsList.Count) % poly.pointsList.Count;
				int pointIDB = verticeID;
				int pointIDC = (verticeID + 1) % poly.pointsList.Count;

				Vector2 pointA = poly.pointsList[pointIDA].ToVector2();
				Vector2 pointB = poly.pointsList[pointIDB].ToVector2();
				Vector2 pointC = poly.pointsList[pointIDC].ToVector2();

				Vector2 offset = pointB;
				float angle = Math2D.FindAngle(pointA, pointB, pointC);
				float angleZero = pointA.Atan2( pointB );

				offset = offset.Push(-angle / 2 + angleZero, 0.5f);

				linearPair.a = offset;
			}
		
			if (UnityEngine.Input.GetMouseButtonDown(1)) {
				Vector2D point = new Vector2D(input.GetInputPosition(0));

				if (target != null) {
					Polygon2D poly = target.shape.GetWorld();
					if (poly.PointInPoly(point) == false) {
						target = null;

						linearPair.a = Vector2.zero;
						linearPair.b = Vector2.zero;
					}
				}

				foreach(Sliceable2D slicer in Sliceable2D.GetList()) {
					Polygon2D poly = slicer.shape.GetWorld();
					if (poly.PointInPoly(point)) {
						
						int id = 0;
						double distance = 1000000;

						foreach(Vector2D p in poly.pointsList) {
							double newDistance = Vector2D.Distance(p, point); 
							if (newDistance < distance) {
								distance = newDistance;
								id = poly.pointsList.IndexOf(p);
							}
						}		

						verticeID = id;
						target = slicer;

						break;
					}
				}
			}
		}

		// Checking mouse press and release events to get linear slices based on input
		public void LateUpdate()  {
			Vector2 pos = input.GetInputPosition(0);
			
			if (input.GetInputPressed(0)) {
				linearPair.b = pos;
			}

			if (input.GetInputReleased(0)) {
				LinearSlice (linearPair.ToPair2D());
			}

			if (input.GetInputPressed(0) == false) {

				linearPair.b = pos;
			}
		}

		private void LinearSlice(Pair2D slice) {
			List<Slice2D> results = Slicing.LinearSliceAll (slice, null);

			if (addForce == false) {
				return;
			}

			// Adding Physics Forces
			float sliceRotation = (float)Vector2D.Atan2 (slice.B, slice.A);
			foreach (Slice2D id in results) {
				foreach (GameObject gameObject in id.GetGameObjects()) {
					Rigidbody2D rigidBody2D = gameObject.GetComponent<Rigidbody2D> ();
					if (rigidBody2D == null) {
						continue;
					}
					foreach (Vector2D p in id.GetCollisions()) {
						rigidBody2D.AddForceAtPosition(new Vector2 (Mathf.Cos (sliceRotation) * addForceAmount, Mathf.Sin (sliceRotation) * addForceAmount), p.ToVector2());
					}
				}
			}
		}
	}
}