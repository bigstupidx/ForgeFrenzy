using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Anvil : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float maxHammeringTime = 4;

	[Header("References")]
	[SerializeField] GameObject hammeringBar;
	[SerializeField] Image hammeringBarFill;
	[SerializeField] GameObject hammeredWeapon;

	GameObject playerHammering;
	GameObject placedObject;
	float hammeringTimer;


	void Awake () {

		ResetHammeringBar();
	}

	public void SetPlayerHammering (GameObject player) {

		if (placedObject.name.Contains ("Ingot")) {
			
			playerHammering = player;
		}
	}

	public void RemovePlayerHammering () {

		playerHammering = null;
	}

	public void PlaceObject (GameObject newObject) {

		placedObject = newObject;
		placedObject.transform.SetParent (this.transform);
		placedObject.transform.localPosition = new Vector3(0.5f, this.transform.GetComponent<Collider>().bounds.max.y / 2f + 0.1f, -0.5f);
		placedObject.transform.localRotation = Quaternion.Euler(90, 0, 90);
		placedObject.GetComponent<Collider> ().enabled = false;
		hammeringTimer = 0;
	}

	public void RemoveObject(PlayerController player) {

		if (placedObject != null) {

			player.ReceiveItem (placedObject);
			placedObject = null;
		}
	}

	public void Hammer () {

		if (placedObject != null && placedObject.name.Contains("Ingot")) {

			if (hammeringTimer < maxHammeringTime) {
			
				hammeringBar.SetActive (true);
				hammeringTimer += Time.deltaTime;
				hammeringBarFill.fillAmount = hammeringTimer / maxHammeringTime;
			}
			else if (hammeringTimer >= maxHammeringTime) {
			
				GameObject newHammeredWeapon = Instantiate (hammeredWeapon, Vector3.zero, Quaternion.identity) as GameObject;
				playerHammering.GetComponent<PlayerController> ().ReceiveItem (newHammeredWeapon);
				Destroy (placedObject.gameObject);
				ResetHammeringBar ();
			}
		}
	}

	public GameObject GetPlacedObject () {

		return placedObject;
	}

	public void ResetHammeringBar () {

		hammeringTimer = 0;
		hammeringBarFill.fillAmount = 0;
		hammeringBar.SetActive(false);
	}
}
