using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class TrebuchetProjectile : MonoBehaviour {
	
	void OnTriggerEnter (Collider other) {

		if (other.CompareTag ("Pickup")) {

			Destroy (other.gameObject);
		}

		if (other.CompareTag ("Player")) {

			PlayerController player = other.GetComponent<PlayerController> ();
			player.StartCoroutine (player.PlayerHit(4));
		}

		if (other.name == "Ground") {

			StartCoroutine (CreateExplosion ());
		}
	}

	IEnumerator CreateExplosion () {

		this.GetComponent<MeshRenderer> ().enabled = false;
		this.GetComponentInParent<TrebuchetProjectileController> ().StopAllCoroutines ();
		// TODO: Instantiate explosion
		Camera.main.GetComponent<CameraEffects> ().ShakeCamera (0.4f);
		yield return new WaitForSeconds (1);
		Destroy (this.transform.parent.gameObject);
	}
}
