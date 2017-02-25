using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibilityTrigger : MonoBehaviour {

	[SerializeField] Material transparentMaterial;

	Material originalMaterial;

	void Awake () {

		originalMaterial = this.GetComponentInParent<MeshRenderer> ().material;
	}

	void OnTriggerEnter(Collider other){

		if (other.CompareTag ("Player")) {

			this.GetComponentInParent<MeshRenderer> ().material = transparentMaterial;
		}
	}

	void OnTriggerExit(Collider other) {

		if (other.CompareTag ("Player")) {

			this.GetComponentInParent<MeshRenderer> ().material = originalMaterial;
		}
	}
}
