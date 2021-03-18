using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Slicer2D.Demo {
	public class DemoDropdownController : MonoBehaviour {
		public enum DropDownTypes {SlicerType, LayerType};
		public DropDownTypes type;
		
		Slicer2DController controller;
		Dropdown dropdown;
		
		void Start () {
			dropdown = GetComponent<Dropdown>();
			controller = Slicer2DController.Get();
		}
		
		void Update () {
			switch (type) {
				case DropDownTypes.LayerType:
					controller.SetLayerType(dropdown.value);
					controller.SetSlicerColor(dropdown.value);
					break;

				case DropDownTypes.SlicerType:
					controller.SetSliceType(dropdown.value);
					break;
			}
		}
	}
}