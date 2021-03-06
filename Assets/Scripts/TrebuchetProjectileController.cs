﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TrebuchetProjectileController : MonoBehaviour {

	[SerializeField] float minZ;

	SpriteRenderer targetSpriteRenderer;
	GameObject trebuchetProjectile;
	GameObject playerTargeting;
	float trackingTime = 2;
	float instantiationTime;

	void Awake () {

		instantiationTime = Time.time;
		targetSpriteRenderer = this.GetComponentInChildren<SpriteRenderer> ();
		trebuchetProjectile = this.transform.GetChild (1).gameObject;

		StartCoroutine (FlickerTargetSprite ());
		StartCoroutine (DropProjectile ());
	}

	void Update () {

		if(playerTargeting && Time.time < (instantiationTime + trackingTime)) {

			this.transform.position = new Vector3(playerTargeting.transform.position.x, this.transform.position.y, playerTargeting.transform.position.z);

			float playerX = playerTargeting.transform.position.x;
			float objectY = this.transform.position.y;
			float clampedZ = Mathf.Clamp(this.transform.position.z, minZ, this.transform.position.z);
			this.transform.position = new Vector3(playerX, objectY, clampedZ);
		}
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

	public void SetTarget(GameObject target) {

		playerTargeting = target;
	}
}
