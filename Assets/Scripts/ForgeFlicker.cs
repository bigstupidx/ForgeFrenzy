using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ForgeFlicker : MonoBehaviour {

	[SerializeField] int materialIndex = 1;
	[SerializeField] Color[] colorArray;

	MeshRenderer meshRenderer;


	void Awake () {

		meshRenderer = this.GetComponent<MeshRenderer> ();
		StartCoroutine (ChangeColor ());
	}

	IEnumerator ChangeColor () {
		
		while (true) {

			Color newColor;

			do {

				newColor = colorArray [Random.Range (0, colorArray.Length)];

			} while (newColor == meshRenderer.materials [materialIndex].color);

			meshRenderer.materials [materialIndex].color = newColor;
			yield return new WaitForSeconds (0.1f);
		}
	}
}
