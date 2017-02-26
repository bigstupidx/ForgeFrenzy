using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class InvisibilityTrigger : MonoBehaviour {

	[SerializeField] Material transparentMaterial;

	MeshRenderer meshRenderer;
	Material originalMaterial;
	int playerCount = 0;

	void Awake () {

		meshRenderer = this.GetComponentInParent<MeshRenderer>();
		originalMaterial = meshRenderer.material;
	}

	void OnTriggerEnter (Collider other) {

		if(other.CompareTag("Player")) {

			playerCount++;
			UpdateMaterial();
		}
	}

	void OnTriggerExit (Collider other) {
		
		if (other.CompareTag ("Player")) {

			playerCount--;
			UpdateMaterial();
		}
	}

	void UpdateMaterial () {

		meshRenderer.material = (playerCount > 0) ? transparentMaterial : originalMaterial;
	}
}
