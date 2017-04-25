using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class FailedCraft : MonoBehaviour {

	SpriteRenderer spriteRenderer;
	float fadeSpeed = 0.25f;

	void Awake () {

		spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
		spriteRenderer.color = Color.white;
	}

	void Update () {

		float newAlpha = spriteRenderer.color.a - fadeSpeed * Time.deltaTime;

		if(newAlpha > 0) {

			spriteRenderer.color = new Color(1, 1, 1, newAlpha);
		}
		else {

			Destroy(this.gameObject);
		}
	}
}
