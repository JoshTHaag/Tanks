using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slicer2D {
	public class Demo16Tutorial : MonoBehaviour {
		public Transform[] points = new Transform[1];

		InputController controller;

		public int currentAction = 0;

		void Start () {
			// Getting slicer input controller
			controller = GetComponent<Slicer2DController>().input;
			
			// Enabling input events programming
			controller.SetRawInput(false);

			controller.AddCompletedEvents(EventsCompleted);

			SetNewEvent();
		}

		void EventsCompleted() {
			SetNewEvent();

			Debug.Log("Events Completed");

			controller.SetRawInput(true);
		}

		void SetNewEvent(int id = 0) {
			controller.ClearActions(id);

			// How fast to perform actions?
			float actionTime = Random.Range(1, 2);

			if (currentAction < points.Length - 1) {
					
				controller.SetMouse(points[currentAction].position, actionTime, id);
				controller.PressMouse(actionTime, id);

				currentAction ++;

				controller.MoveMouse(points[currentAction].position, actionTime, id);
				controller.ReleaseMouse(actionTime, id);

				controller.Play(id);
			}
		}
	}
}