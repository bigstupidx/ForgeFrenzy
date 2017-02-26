using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Anvil : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float maxHammeringTime = 4;

	[Header("References")]
	[SerializeField] GameObject hammeringBar;
	[SerializeField] Image hammeringBarFill;
	[SerializeField] GameObject hammeredAxe;
	[SerializeField] GameObject hammeredSword;

	GameObject playerHammering;
	GameObject placedObject;
	float hammeringTimer;


	void Awake () {

		ResetHammeringBar();
	}

	public void SetPlayerHammering (GameObject player) {

		playerHammering = player;
	}

	public void RemovePlayerHammering () {

		playerHammering = null;
	}

	public void PlaceObject (GameObject newObject) {

		placedObject = newObject;
		placedObject.transform.SetParent (this.transform);
		placedObject.transform.localPosition = new Vector3(0, this.transform.GetComponent<Collider>().bounds.max.y / 2f, 0);
		placedObject.transform.localRotation = Quaternion.Euler(90, 0, 90);
		placedObject.GetComponent<Collider> ().enabled = true;
		hammeringTimer = 0;
	}

	public void Hammer () {

		if (hammeringTimer < maxHammeringTime) {
			
			hammeringBar.SetActive (true);
			hammeringTimer += Time.deltaTime;
			hammeringBarFill.fillAmount = hammeringTimer / maxHammeringTime;
		}
		else if(hammeringTimer >= maxHammeringTime) {
			
			if(placedObject.name.Contains("Axe Metal")) {

				GameObject newHammeredAxe = Instantiate(hammeredAxe, Vector3.zero, Quaternion.identity) as GameObject;
				playerHammering.GetComponent<PlayerController>().ReceiveItem(newHammeredAxe);
				Destroy(placedObject.gameObject);
				ResetHammeringBar();
			}
			else if(placedObject.name.Contains("Sword Metal")) {

				GameObject newHammeredSword = Instantiate(hammeredAxe, Vector3.zero, Quaternion.identity) as GameObject;
				playerHammering.GetComponent<PlayerController>().ReceiveItem(newHammeredSword);
				Destroy(placedObject.gameObject);
				ResetHammeringBar();
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
