using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[DisallowMultipleComponent]
public class Anvil : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float maxHammeringTime = 4;

	[Header("References")]
	[SerializeField] GameObject hammeringBar;
	[SerializeField] Image hammeringBarFill;
	[SerializeField] GameObject hammeredWeapon;
	[SerializeField] GameObject aButtonGameobject;
	[SerializeField] GameObject xButtonGameobject;

	GameObject playerHammering;
	GameObject placedObject;
	float hammeringTimer;


	void Awake () {

		ResetHammeringBar();
	}

	void OnTriggerEnter(Collider other) {

		if(other.CompareTag("Player")) {

			if(!placedObject) {

				if(other.GetComponent<PlayerController>().HasObject()) {

					aButtonGameobject.SetActive(true);
				}
			}
			else if(placedObject.name.Contains("Ingot")) {
				
				xButtonGameobject.SetActive(true);
			}
		}
	}

	void OnTriggerExit(Collider other) {

		if(other.CompareTag("Player")) {

			xButtonGameobject.SetActive(false);
			aButtonGameobject.SetActive(false);
		}
	}

	public void SetPlayerHammering (GameObject player) {

		if (placedObject && placedObject.name.Contains ("Ingot")) {
			
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
		aButtonGameobject.SetActive(false);
		xButtonGameobject.SetActive(true);
	}

	public void RemoveObject(PlayerController player) {

		if (placedObject != null) {

			player.ReceiveItem (placedObject);
			placedObject = null;
		}
	}

	public void Hammer () {

		if (placedObject != null && placedObject.name.Contains("Ingot")) {

			if(hammeringTimer == 0) {

				xButtonGameobject.SetActive(false);
				hammeringBar.SetActive(true);
				hammeringBarFill.fillAmount = 0;
			}

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

			if(this.GetComponent<AudioSource>().isPlaying == false) {

				this.GetComponent<AudioSource>().Play();
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
		aButtonGameobject.SetActive(false);
		xButtonGameobject.SetActive(false);
	}
}
