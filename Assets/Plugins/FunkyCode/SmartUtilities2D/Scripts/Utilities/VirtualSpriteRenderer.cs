﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities2D {
	public class VirtualSpriteRenderer {
		public Sprite sprite;

		public int sortingLayerID = 0;
		public string sortingLayerName = "";
		public int sortingOrder = 0;

		public Color color;
		public Material material;

		public bool flipX = false;
		public bool flipY = false;

		public VirtualSpriteRenderer(SpriteRenderer spriteRenderer) {
			if (spriteRenderer != null) {
				sprite = spriteRenderer.sprite;
			}
			else
            {
				return;
            }
			
			sortingLayerID = spriteRenderer.sortingLayerID;
			sortingLayerName = spriteRenderer.sortingLayerName;
			sortingOrder = spriteRenderer.sortingOrder;

			flipX = spriteRenderer.flipX;
			flipY = spriteRenderer.flipY;

			material = new Material(spriteRenderer.sharedMaterial);

			color = spriteRenderer.color;
		}
	}
}