using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D.Demo {
	
	public class DemoSlicer2DInspector : MonoBehaviour {
		Vector3 inspectorPosition = Vector3.zero;
		double originalSize = 0;
		double currentSize = 0;

		int sliced = 0;
		
		void OnGUI() {
			inspectorPosition = Vector3.zero;

			Vector2D pos = new Vector2D(Camera.main.ScreenToWorldPoint (UnityEngine.Input.mousePosition));
			
			foreach(Sliceable2D slicer in Sliceable2D.GetList()) {
				Polygon2D poly = slicer.shape.GetWorld();
				if (poly.PointInPoly(pos)) {
					Rect rect = poly.GetBounds();

					inspectorPosition = new Vector2(rect.center.x, rect.center.y + rect.height / 2);
					
					originalSize = slicer.GetComponent<DemoSlicer2DInspectorTracker>().originalSize;
					currentSize = poly.GetArea();
					sliced = slicer.limit.counter;
				}
			}

			if (Vector3.zero == inspectorPosition) {
				return;
			}

			Vector2 p = Camera.main.WorldToScreenPoint (inspectorPosition);
			TextWithShadow(p.x, p.y, "Original Size: " + (int)originalSize + " (100%)");

			inspectorPosition.y += 1;
			p = Camera.main.WorldToScreenPoint (inspectorPosition);
			
			float currentSizePercent = (float)System.Math.Round((currentSize / originalSize) * 100);
			TextWithShadow(p.x, p.y, "Current Size: " + (int)currentSize + " (" + currentSizePercent + "%)");

			inspectorPosition.y += 1;
			p = Camera.main.WorldToScreenPoint (inspectorPosition);
			TextWithShadow(p.x, p.y, "Sliced: " + sliced + " times");
		}

		public void TextWithShadow(float x, float y, string text) {
			GUIStyle textStyle2 = GUI.skin.GetStyle("Label");
			textStyle2.alignment = TextAnchor.UpperCenter;
			textStyle2.normal.textColor = Color.black;

			GUI.Label(new Rect(x - 99, Screen.height - y + 1, 200, 20), text, textStyle2);

			GUIStyle textStyle = GUI.skin.GetStyle("Label");
			textStyle.alignment = TextAnchor.UpperCenter;
			textStyle.normal.textColor = Color.white;

			GUI.Label(new Rect(x - 100, Screen.height - y, 200, 20), text, textStyle);
		}
	}
}