using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TrebuchetProjectileController : MonoBehaviour {

	SpriteRenderer targetSpriteRenderer;
	GameObject trebuchetProjectile;

	void Awake () {

		targetSpriteRenderer = this.GetComponentInChildren<SpriteRenderer> ();
		trebuchetProjectile = this.transform.GetChild (1).gameObject;

		StartCoroutine (FlickerTargetSprite ());
		StartCoroutine (DropProjectile ());
	}

	IEnumerator FlickerTargetSprite () {

		float flickerLength = 0.25f;

		while(true) {

			targetSpriteRenderer.enabled = !targetSpriteRenderer.enabled;
			yield return new WaitForSeconds (flickerLength);
		}
	}

	IEnumerator DropProjectile () {

		yield return new WaitForSeconds (2);
		trebuchetProjectile.SetActive (true);
	}
}
