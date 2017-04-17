using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// TODO:
// - Implement fail state

[DisallowMultipleComponent]
public class Tannery : MonoBehaviour {

	[Header("Parameters")]
	[SerializeField] float maxTanningTime = 14;
	[SerializeField] float tanningSpeed = 1;

	[Header("References")]
	[SerializeField] GameObject tanningBar;
	[SerializeField] Image tanningBarFill;
	[SerializeField] GameObject tannedLeatherPrefab;
	[SerializeField] GameObject aButtonGameobject;

	// Privates
	GameObject leatherTanning;
	float tanningTimer = 0;


	void Awake () {

		ResetTanning ();
	}

	void OnTriggerEnter(Collider other) {

		if(other.CompareTag("Player") && other.GetComponent<PlayerController>().HasObject()) {

			if(other.GetComponent<PlayerController>().GetPickedUpObject().name.Contains("Broken Leather")) {

				aButtonGameobject.SetActive(true);
			}
		}
	}

	void OnTriggerExit(Collider other) {

		if(other.CompareTag("Player")) {

			aButtonGameobject.SetActive(false);
		}
	}

	void Update () {

		if (leatherTanning) {

			tanningTimer += tanningSpeed * Time.deltaTime;
			tanningBarFill.fillAmount = (tanningTimer / maxTanningTime);

			if(tanningTimer >= maxTanningTime) {

				GameObject newLeather = Instantiate(tannedLeatherPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				newLeather.transform.SetParent(this.transform);
				newLeather.transform.localPosition = leatherTanning.transform.localPosition;
				newLeather.transform.localRotation = leatherTanning.transform.localRotation;
				Destroy(leatherTanning.gameObject);
				ResetTanning();
				aButtonGameobject.SetActive(true);
			}
		}
	}

	public void PlaceLeather (GameObject brokenLeather) {

		if (leatherTanning == null) {

			leatherTanning = brokenLeather;
			leatherTanning.transform.SetParent(this.transform);
			leatherTanning.transform.localPosition = Vector3.zero;
			leatherTanning.transform.localRotation = Quaternion.identity;
			leatherTanning.GetComponent<Collider>().enabled = false;
			tanningBar.SetActive (true);
			aButtonGameobject.SetActive(false);
		}
	}

	public void ResetTanning () {

		tanningTimer = 0;
		tanningBarFill.fillAmount = 0;
		tanningBar.SetActive (false);
		aButtonGameobject.SetActive(false);
	}
}
