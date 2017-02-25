using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Anvil : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float fillSpeed;

	[Header("References")]
	[SerializeField] Canvas anvilCanvas;
	[SerializeField] Image barFill;

	GameObject placedObject;
	float completionProgress;


	void Awake () {

		placedObject = null;
		anvilCanvas.gameObject.SetActive (false);
	}

	public void PlaceObject (GameObject newObject) {

		placedObject = newObject;
		placedObject.transform.SetParent (this.transform);
		placedObject.transform.localPosition = new Vector3(0, this.transform.GetComponent<Collider>().bounds.max.y / 2f, 0);
		placedObject.transform.localRotation = Quaternion.Euler(90, 0, 90);
		placedObject.GetComponent<Collider> ().enabled = true;
		completionProgress = 0;
	}

	public void Hammer () {

		if (completionProgress < 1) {
			
			anvilCanvas.gameObject.SetActive (true);
			completionProgress += 0.01f;
			barFill.fillAmount = completionProgress;
		}
		else {

			anvilCanvas.gameObject.SetActive (false);
			placedObject.GetComponentInChildren<SpriteRenderer> ().color = Color.yellow;
			Debug.Log ("Hammering finished!");
		}
	}

	public GameObject GetPlacedObject () {

		return placedObject;
	}
}
