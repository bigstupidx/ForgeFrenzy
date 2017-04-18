using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[DisallowMultipleComponent]
public class Woodworks : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float maxWoodCuttingTime = 2;

	[Header("References")]
	[SerializeField] GameObject woodCuttingBar;
	[SerializeField] Image woodBarFill;
	[SerializeField] GameObject choppedWoodPrefab;
	[SerializeField] GameObject xButtonGameobject;

	float woodCuttingTimer = 0;
	GameObject playerCutting;


	void Awake () {

		ResetWoodCuttingBar ();
	}

	void OnTriggerEnter (Collider other) {

		if (other.CompareTag ("Player")) {

			xButtonGameobject.SetActive (true);
		}
	}

	void OnTriggerExit(Collider other) {

		if (other.CompareTag ("Player")) {

			xButtonGameobject.SetActive (false);
		}
	}

	public void SetPlayerCutting (GameObject player) {

		playerCutting = player;
	}

	public void RemovePlayerCutting () {

		playerCutting = null;
	}

	public void ChopWood () {

		if (woodCuttingTimer == 0) {

			woodCuttingBar.SetActive (true);
			//xButtonGameobject.SetActive (false);
			woodBarFill.fillAmount = 0;
		}

		if (woodCuttingTimer < maxWoodCuttingTime) {

			woodCuttingTimer += Time.deltaTime;
			woodBarFill.fillAmount = (woodCuttingTimer / maxWoodCuttingTime);
		}
		else if (woodCuttingTimer >= maxWoodCuttingTime) {
			
			ResetWoodCuttingBar ();
			GameObject choppedWood = Instantiate (choppedWoodPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			playerCutting.GetComponent<PlayerController> ().ReceiveItem (choppedWood);
		}

		if(this.GetComponent<AudioSource>().isPlaying == false) {

			this.GetComponent<AudioSource>().Play();
		}
	}

	public void ResetWoodCuttingBar () {

		woodCuttingTimer = 0;
		woodBarFill.fillAmount = 0;
		woodCuttingBar.SetActive (false);
		xButtonGameobject.SetActive(false);
	}
}
