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

	// Privates
	GameObject leatherTanning;
	float tanningTimer = 0;


	void Awake () {

		ResetTanning ();
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
			}
		}
	}

	public void PlaceLeather (GameObject brokenLeather) {

		if (leatherTanning == null) {

			leatherTanning = brokenLeather;
			leatherTanning.transform.SetParent(this.transform);
			leatherTanning.transform.localPosition = new Vector3(0, this.transform.GetComponent<Collider>().bounds.max.y / 2f + 0.1f, 0);
			leatherTanning.transform.localRotation = Quaternion.Euler(90, 0, 0);
			leatherTanning.GetComponent<Collider>().enabled = false;
			tanningBar.SetActive (true);
		}
	}

	public void ResetTanning () {

		tanningTimer = 0;
		tanningBarFill.fillAmount = 0;
		tanningBar.SetActive (false);
	}
}
