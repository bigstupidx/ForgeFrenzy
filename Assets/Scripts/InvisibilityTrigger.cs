using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class InvisibilityTrigger : MonoBehaviour {

	[SerializeField] Material transparentMaterial;

	MeshRenderer meshRenderer;
	Material[] originalMaterialsArray;
	Material[] transparentMateralArray;
	int playerCount = 0;

	void Awake () {

		meshRenderer = this.GetComponentInParent<MeshRenderer>();

		originalMaterialsArray = new Material[meshRenderer.materials.Length];
		transparentMateralArray = new Material[meshRenderer.materials.Length];

		for(int i = 0; i < meshRenderer.materials.Length; i++) {

			originalMaterialsArray[i] = meshRenderer.materials[i];
			transparentMateralArray[i] = transparentMaterial;
		}
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
		
		if(playerCount > 0) {
			
			meshRenderer.materials = transparentMateralArray;
		}
		else {

			meshRenderer.materials = originalMaterialsArray;
		}
	}
}
